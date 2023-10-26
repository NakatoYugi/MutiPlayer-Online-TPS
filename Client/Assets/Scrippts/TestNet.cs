using proto.BattleMsg;
using proto.SysMsg;
using ProtoBuf;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TestNet : MonoBehaviour
{
    public TMPro.TMP_InputField account;
    public TMPro.TMP_InputField password;


    // Start is called before the first frame update
    void Start()
    {
        //NetManager.AddMsgListener("proto.BattleMsg.MsgMove", OnMsgMove);
        NetManager.AddMsgListener("proto.SysMsg.MsgPing", OnMsgPing);
        NetManager.AddMsgListener("proto.SysMsg.MsgPong", OnMsgPong);
        NetManager.AddMsgListener("proto.LoginMsg.MsgRegister", OnMsgRegister);
        NetManager.AddMsgListener("proto.LoginMsg.MsgLogin", OnMsgLogin);
        NetManager.AddEventListener(NetManager.NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.AddEventListener(NetManager.NetEvent.ConnectFail, OnConnectFail);
        NetManager.AddEventListener(NetManager.NetEvent.Close, OnClose);
        NetManager.Connect("192.168.232.1", 8888);
    }

    public void OnRegisterButtonClick()
    {
        proto.LoginMsg.MsgRegister msgRegister = new proto.LoginMsg.MsgRegister();
        msgRegister.id = account.text;
        msgRegister.pw = password.text;
        NetManager.Send(msgRegister);
    }

    public void OnLoginButtonClick()
    {
        proto.LoginMsg.MsgLogin msgLogin = new proto.LoginMsg.MsgLogin();
        msgLogin.id = account.text;
        msgLogin.pw = password.text;
        NetManager.Send(msgLogin);
    }

    private void OnMsgRegister(IExtensible msgBase)
    {
        proto.LoginMsg.MsgRegister msgRegister = msgBase as proto.LoginMsg.MsgRegister;
        if (msgRegister.result == 0)
        {
            Debug.Log("×¢²á³É¹¦");
        }
        else
        {
            Debug.Log("×¢²áÊ§°Ü");
        }
    }

    private void OnMsgLogin(IExtensible msgBase)
    {
        proto.LoginMsg.MsgLogin msgLogin = msgBase as proto.LoginMsg.MsgLogin;
        if (msgLogin.result == 0)
        {
            Debug.Log("µÇÂ¼³É¹¦");
        }
        else
        {
            Debug.Log("µÇÂ¼Ê§°Ü");
        }
    }

    public void Send()
    {
        MsgMove msgMove = new MsgMove();
        msgMove.x = 9999;
        NetManager.Send(msgMove);
    }

    private void OnClose(string str)
    {
        Debug.Log("Close" + str);
    }

    private void OnConnectFail(string str)
    {
        Debug.Log("Connect Fail" + str);
    }

    private void OnConnectSucc(string str)
    {
        Debug.Log("Connect Succ" + str);
    }

    private void OnMsgPong(IExtensible msgBase)
    {
        Debug.Log("MsgPong");
    }

    private void OnMsgPing(IExtensible msgBase)
    {
        MsgPing msgPing = new MsgPing();
        Debug.Log("MsgPing");
    }

    void OnMsgMove(ProtoBuf.IExtensible msgBase)
    {
        MsgMove msgMove = msgBase as MsgMove;
        Debug.Log("MoveX:" + msgMove.x);
    }

    // Update is called once per frame
    void Update()
    {
        NetManager.Update();
    }
}
