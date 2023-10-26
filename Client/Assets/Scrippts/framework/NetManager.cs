using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using proto.SysMsg;
using ProtoBuf;

public static class NetManager
{
    public enum NetEvent
    {
        ConnectSucc = 1,
        ConnectFail = 2,
        Close = 4,
    }

    static bool isConnecting = false;

    static bool isClosing = false;

    static Socket socket;
    //接收缓冲区
    static ByteArray readBuff;
    //写入队列
    static Queue<ByteArray> writeQueue;
    //消息列表
    static List<ProtoBuf.IExtensible> msgList = new List<ProtoBuf.IExtensible>();
    //消息列表长度
    static int msgCount = 0;
    //每次Update处理的消息量
    readonly static int MAX_MESSAGE_FIRE = 10;
    //事件委托类型
    public delegate void EventListener(string str);
    //事件监听列表
    private static Dictionary<NetEvent, EventListener> eventListeners = new Dictionary<NetEvent, EventListener>();
    //消息委托类型
    public delegate void MsgListener(ProtoBuf.IExtensible msgBase);
    //消息监听列表
    private static Dictionary<string, MsgListener> msgListeners = new Dictionary<string, MsgListener>();

    public static bool isUsePing = true;
    public static int pingInterval = 30;
    static float lastPingTime = 0f;
    static float lastPongTime = 0f;

    #region Add、Remove、Fire Event And Message
    public static void AddEventListener(NetEvent netEvent, EventListener listener)
    {
        if (eventListeners.ContainsKey(netEvent))
            eventListeners[netEvent] += listener;
        else
            eventListeners[netEvent] = listener;
    }

    public static void RemoveEventListener(NetEvent netEvent, EventListener listener)
    {
        if (eventListeners.ContainsKey(netEvent))
        {
            eventListeners[netEvent] -= listener;
            if (eventListeners[netEvent] == null)
            {
                eventListeners.Remove(netEvent);
            }
        }
    }

    private static void FireEvent(NetEvent netEvent, string args)
    {
        if (eventListeners.ContainsKey(netEvent))
            eventListeners[netEvent](args);
    }

    public static void AddMsgListener(string msgName, MsgListener listener)
    {
        if (msgListeners.ContainsKey(msgName))
            msgListeners[msgName] += listener;
        else
            msgListeners[msgName] = listener;
    }

    public static void RemoveMsgListener(string msgName, MsgListener listener)
    {
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName] -= listener;
            if (msgListeners[msgName] == null)
            {
                msgListeners.Remove(msgName);
            }
        }
    }

    private static void FireMsg(string msgName, ProtoBuf.IExtensible msgBase)
    {
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName](msgBase);
        }
    }
    #endregion

    public static void Connect(string ip, int port)
    {
        if (socket != null && socket.Connected) return;

        if (isConnecting) return;

        InitData();
        socket.NoDelay = true;
        isConnecting = true;
        socket.BeginConnect(ip, port, ConnectCallback, socket);
    }

    private static void InitData()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        readBuff = new ByteArray();
        writeQueue = new Queue<ByteArray>();
        isConnecting = false;
        isClosing = false;
        msgList = new List<ProtoBuf.IExtensible>();
        msgCount = 0;

        lastPingTime = Time.time;
        lastPongTime = Time.time;

        if (!msgListeners.ContainsKey("MsgPong"))
        {
            AddMsgListener("proto.SysMsg.MsgPong", OnMsgPong);
        }
    }

    private static void OnMsgPong(IExtensible msgBase)
    {
        lastPongTime = Time.time;
    }

    public static void Send(ProtoBuf.IExtensible msgBase)
    {
        if (socket == null || !socket.Connected) return;
        if (isConnecting || isClosing) return;
        if (msgBase == null) return;

        byte[] sendBytes = ProtocolManager.Encode(msgBase);

        ByteArray byteArray = new ByteArray(sendBytes);
        int count = 0;
        lock (writeQueue)
        {
            writeQueue.Enqueue(byteArray);
            count = writeQueue.Count;
        }

        if (count == 1)
        {
            socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, socket);
        }
    }

    public static void Close()
    {
        if (socket == null || !socket.Connected || isConnecting) return;

        if (writeQueue.Count > 0)
            isClosing = true;
        else
        {
            socket.Close();
            isClosing = false;
            FireEvent(NetEvent.Close, "");
        }
    }

    //获取描述
    public static string GetDesc()
    {
        if (socket == null || !socket.Connected) return "";
        return socket.LocalEndPoint.ToString();
    }

    private static void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = ar.AsyncState as Socket;
            socket.EndConnect(ar);
            FireEvent(NetEvent.ConnectSucc, "");
            isConnecting = false;
            socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0, ReceiveCallback, socket);
        }
        catch (SocketException ex)
        {
            Debug.LogError("SOCKET::CONNECT::ERROR:" + ex.ToString());
            FireEvent(NetEvent.ConnectFail, ex.ToString());
            isConnecting = false;
        }
    }
    
    private static void ReceiveCallback (IAsyncResult ar)
    {
        Debug.Log("Receive Callback");
        try
        {
            Socket socket = ar.AsyncState as Socket;
            int count = socket.EndReceive(ar);
            if (count == 0)
            {
                Close();
                return;
            }
            readBuff.writeIdx += count;
            OnReceiveData();

            readBuff.CheckAndMoveBytes();

            socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0, ReceiveCallback, socket);
        }
        catch (SocketException ex)
        {
            Debug.LogError("SOCKET::RECEIVE::ERROR:" + ex.ToString());
        }
    }

    private static void OnReceiveData()
    {
        if (readBuff.length <= 2)
            return;

        int readIdx = readBuff.readIdx;
        byte[] bytes = readBuff.bytes;
        Int16 bodyLength = (Int16)((bytes[readIdx + 1] << 8) | bytes[readIdx]);

        if (readBuff.length < bodyLength + 2)
            return;

        readBuff.readIdx += 2;

        int nameCount = 0;
        string protoName = ProtocolManager.DecodeName(readBuff.bytes, readBuff.readIdx, out nameCount);
        if (protoName == String.Empty)
        {
            Debug.LogError("ONRECEIVEDATA::DECODENAME::ERROT");
            return;
        }

        readBuff.readIdx += nameCount;

        int bodyCount = bodyLength - nameCount;
        ProtoBuf.IExtensible msgBase = ProtocolManager.DecodeBody(protoName, readBuff.bytes, readBuff.readIdx, bodyCount);
        readBuff.readIdx += bodyCount;
        readBuff.CheckAndMoveBytes();

        Debug.Log("Receive Protocol:" + msgBase.ToString());
        lock (msgList)
        {
            msgList.Add(msgBase);
        }
        msgCount++;

        if (readBuff.length > 2)
        {
            OnReceiveData();
        }
    }

    private static void SendCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = ar.AsyncState as Socket;
            if (socket == null || !socket.Connected) return;

            int count = socket.EndSend(ar);
            ByteArray byteArray;
            lock (writeQueue)
            {
                byteArray = writeQueue.First();
            }

            byteArray.readIdx += count;
            if (byteArray.length == 0)
            {
                lock (writeQueue)
                {
                    writeQueue.Dequeue();
                    byteArray = writeQueue.First();
                }
            }

            if (byteArray != null)
            {
                socket.BeginSend(byteArray.bytes, byteArray.readIdx, byteArray.length, 0, SendCallback, socket);
            }
            else if(isClosing)
            {
                socket.Close();
            }
        }
        catch(SocketException ex) 
        {
            Debug.LogError("SOCKET::SEND::ERROR:" + ex.ToString());
        }
    }

    public static void Update()
    {
        MsgUpdate();
        PingUpdate();
    }

    public static void MsgUpdate()
    {
        if (msgCount == 0)
            return;

        for (int i = 0; i < MAX_MESSAGE_FIRE; i++)
        {
            ProtoBuf.IExtensible msgBase = null;
            lock (msgList)
            {
                if (msgList.Count > 0)
                {
                    msgBase = msgList[0];
                    msgList.RemoveAt(0);
                    msgCount--; 
                }
            }

            if (msgBase != null)
            {
                FireMsg(msgBase.ToString(), msgBase);
            }
            else
                break;
        }
    }

    private static void PingUpdate()
    {
        if (!isUsePing)
            return;

        
        if (Time.time - lastPingTime > pingInterval)
        {
            MsgPing msgPing = new MsgPing();
            Send(msgPing);
            lastPingTime = Time.time;
        }

        if(Time.time - lastPongTime > pingInterval * 4)
        {
            Close();
        }
    }
}