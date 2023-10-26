using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using proto.LoginMsg;
using ProtoBuf;
using System;

public class LoginPanel : BasePanel
{
    public TMPro.TMP_InputField account;
    public TMPro.TMP_InputField password;
    public Button loginButton;
    public Button registerButton;

    public override void OnInit()
    {
        skinPath = "LoginPanel";
        layer = PanelManager.Layer.Panel;
    }

    public override void OnShow(params object[] para)
    {
        account = skin.transform.Find("Account").GetComponent<TMP_InputField>();
        password = skin.transform.Find("Password").GetComponent<TMP_InputField>();
        loginButton = skin.transform.Find("Login Button").GetComponent<Button>();
        registerButton = skin.transform.Find("Register Button").GetComponent<Button>();

        loginButton.onClick.AddListener(() => {
            if (account.text == "" || password.text == "")
            {
                PanelManager.Open<TipPanel>("用户名和密码不能为空");
                return;
            }
            MsgLogin msgLogin = new MsgLogin();
            msgLogin.id = account.text;
            msgLogin.pw = password.text;
            NetManager.Send(msgLogin);
        });

        registerButton.onClick.AddListener(() => {
            PanelManager.Open<RegisterPanel>();
        });

        NetManager.AddMsgListener("proto.LoginMsg.MsgLogin", OnMsgLogin);
        NetManager.AddEventListener(NetManager.NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.AddEventListener(NetManager.NetEvent.ConnectFail, OnConnectFail);
        NetManager.Connect("192.168.232.1", 8888);
    }

    public override void OnClose()
    {
        NetManager.RemoveMsgListener("proto.LoginMsg.MsgLogin", OnMsgLogin);
        NetManager.RemoveEventListener(NetManager.NetEvent.ConnectSucc, OnConnectSucc);
        NetManager.RemoveEventListener(NetManager.NetEvent.ConnectFail, OnConnectFail);
    }

    private void OnConnectFail(string str)
    {
        PanelManager.Open<TipPanel>(str);
    }

    private void OnConnectSucc(string str)
    {
        Debug.Log("OnConnectSucc");
    }

    private void OnMsgLogin(IExtensible msgBase)
    {
        MsgLogin msgLogin = msgBase as MsgLogin;
        if (msgLogin.result == 0)
        {
            Debug.Log("登陆成功");
            //TODO:进入游戏
                
            GameMain.id = msgLogin.id;
            Close();
        }
        else
        {
            PanelManager.Open<TipPanel>("登陆失败");
        }
    }
}
