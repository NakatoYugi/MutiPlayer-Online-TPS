using ProtoBuf;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    public static string id = "";
    // Start is called before the first frame update
    void Start()
    {
        NetManager.AddEventListener(NetManager.NetEvent.Close, OnConnectClose);
        NetManager.AddMsgListener("proto.LoginMsg.MsgKick", OnMsgKick);
        PanelManager.Init();
        PanelManager.Open<LoginPanel>();
    }

    private void OnMsgKick(IExtensible msgBase)
    {
        PanelManager.Open<TipPanel>("异地登陆");
    }

    private void OnConnectClose(string str)
    {
        PanelManager.Open<TipPanel>("连接断开");
    }

    // Update is called once per frame
    void Update()
    {
        NetManager.Update();
    }
}
