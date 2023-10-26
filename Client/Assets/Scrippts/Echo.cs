using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using UnityEngine.UI;
using TMPro;
using System;

public class Echo : MonoBehaviour
{
    Socket socket;

    public TMP_InputField inputField;
    public TextMeshProUGUI textMeshPro;
    string recStr;
    byte[] readBuff = new byte[1024];
    public void Connection()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.BeginConnect("192.168.232.1", 8888, ConnectCallback, socket);
    }

    private void ConnectCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = ar.AsyncState as Socket;
            socket.EndConnect(ar);
            socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallback, socket);
        }
        catch (SocketException ex)
        {
            Debug.LogError("SOCKET::CONNECT::ERROR" + ex.ToString());
        }
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = ar.AsyncState as Socket;
            int count = socket.EndReceive(ar);
            recStr = System.Text.Encoding.Default.GetString(readBuff, 0, count);
            Debug.Log(recStr);

            socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallback, socket);
        }
        catch(SocketException ex) 
        {
            Debug.LogError("SOCKET::RECEIVE::ERROR" + ex.ToString());
        }

    }

    public void Send()
    {
        string sendStr = inputField.text;
        byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
        socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCallback, socket);
    }

    private void SendCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = ar.AsyncState as Socket;
            socket.EndSend(ar);
        }
        catch (SocketException ex)
        {
            Debug.LogError("SOCKET::SEND::ERROR" + ex.ToString());
        }
    }

    private void Update()
    {
        textMeshPro.text = recStr;
    }
}
