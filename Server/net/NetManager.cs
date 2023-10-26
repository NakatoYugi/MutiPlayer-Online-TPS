using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Multiplayer_Online_Tank_War
{
    class NetManager
    {
        public static long pingInterval = 30;
        public static Socket listenfd;
        public static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();

        //select多路复用技术，用于检查是否有可读的Socket
        static List<Socket> checkRead = new List<Socket>();

        public static void StartLoop(int listenPort)
        {
            listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAdr = IPAddress.Parse("192.168.232.1");
            IPEndPoint ipEp = new IPEndPoint(ipAdr, listenPort);
            listenfd.Bind(ipEp);
            listenfd.Listen(0);
            Console.WriteLine("SERVER START LOOP");

            while (true)
            {
                ResetCheckRead();
                Socket.Select(checkRead, null, null, 1000);
                for (int i = checkRead.Count - 1; i >= 0; i--)
                {
                    Socket socket = checkRead[i];
                    if (socket == listenfd)
                    {
                        ReadListenfd();
                    }
                    else
                    {
                        ReadClientfd(socket);
                    }
                }

                Timer();
            }
        }

        private static void ResetCheckRead()
        {
            checkRead.Clear();
            checkRead.Add(listenfd);
            foreach (var clientState in clients.Values)
            {
                checkRead.Add(clientState.socket);
            }
        }

        private static void ReadListenfd()
        {
            try
            {
                Socket clientfd = listenfd.Accept();
                Console.WriteLine("SEVER ACCEPT:" + clientfd.RemoteEndPoint.ToString());
                ClientState state = new ClientState(clientfd);
                clients.Add(clientfd, state);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("SOCKET::ACCEPT::ERROR:" + ex.ToString());
            }
        }

        private static void ReadClientfd(Socket clientfd)
        {
            Console.WriteLine("ReadClientfd");
            ClientState state = clients[clientfd];
            ByteArray readBuff = state.readBuff;
            int count = 0;
            if (readBuff.remain <= 0)
            {
                OnReceiveData(state);
                readBuff.MoveBytes();
            }
            if (readBuff.remain <= 0)
            {
                Console.WriteLine("message length > byteArray capacity");
                Close(state);
                return;
            }

            try
            {
                count = clientfd.Receive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, 0);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("SOCKET::RECEIVE::ERROR:" + ex.ToString());
                Close(state);
                return;
            }

            if (count <= 0)
            {
                Console.WriteLine("SOCKET CLOSE " + clientfd.RemoteEndPoint.ToString());
                Close(state);
                return;
            }

            readBuff.writeIdx += count;
            OnReceiveData(state);
            readBuff.CheckAndMoveBytes();
        }

        private static void OnReceiveData(ClientState state)
        {
            ByteArray readBuff = state.readBuff;
            byte[] bytes = readBuff.bytes;

            if (readBuff.length <= 2)
                return;

            int readIdx = readBuff.readIdx;
            Int16 len = (Int16)((bytes[readIdx + 1] << 8 | bytes[readIdx]));

            if (readBuff.length < len + 2)
                return;

            readBuff.readIdx += 2;

            int nameCount = 0;
            string protoName = ProtocolManager.DecodeName(readBuff.bytes, readBuff.readIdx, out nameCount);
            if (protoName == String.Empty)
            {
                Console.WriteLine("ONRECEIVEDATA::DECODENAME::ERROR");
                Close(state);
                return;
            }

            readBuff.readIdx += nameCount;

            int bodyCount = len - nameCount;
            ProtoBuf.IExtensible msgBase = ProtocolManager.DecodeBody(protoName, readBuff.bytes, readBuff.readIdx, bodyCount);

            readBuff.readIdx += bodyCount;
            readBuff.CheckAndMoveBytes();

            //提取出不包含命名空间的方法名 proto.BattleMsg.MsgMove => MsgMove
            string[] strs = protoName.Split('.');
            string methodName = strs[2];
            //Console.WriteLine("Debug: OnReceiveData methodName:" + methodName);
            MethodInfo methodInfo = typeof(Multiplayer_Online_Tank_War.MsgHandler).GetMethod(methodName);
            object[] o = { state, msgBase};
            //Console.WriteLine("Recive Protocol:" + methodName);
            if (methodInfo != null)
            {
                methodInfo.Invoke(state, o);
            }
            else
            {
                Console.WriteLine("OnReceiveData Invoke fail " + methodName);
            }

            if (readBuff.length > 2)
                OnReceiveData(state);
        }

        public static void Send(ClientState clientState, ProtoBuf.IExtensible msgBase)
        {
            if (clientState == null || !clientState.socket.Connected)
                return;

            byte[] sendBytes = ProtocolManager.Encode(msgBase);
            //Console.WriteLine(sendBytes.Length);
            try
            {
                clientState.socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, clientState);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("SERVER::SEND::ERROR:" + ex.ToString());
            }
        }

        private static void SendCallback(IAsyncResult ar)
        {
            ClientState clientState = ar.AsyncState as ClientState;
            int count = clientState.socket.EndSend(ar);
            //Console.WriteLine("Send Count:" + count);
        }

        public static void Close(ClientState state)
        {
            MethodInfo methodInfo = typeof(Multiplayer_Online_Tank_War.EventHandler).GetMethod("OnDisconnect");
            object[] objects = { state};
            methodInfo.Invoke(null, objects);

            state.socket.Close();
            clients.Remove(state.socket);
        }

        private static void Timer()
        {
            MethodInfo methodInfo = typeof(Multiplayer_Online_Tank_War.EventHandler).GetMethod("OnTimer");
            object[] objects = {};
            methodInfo.Invoke(null, objects);
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStamp()
        {
            TimeSpan timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(timeSpan.TotalMilliseconds);
        }
    }
}
