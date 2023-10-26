using proto.LoginMsg;
using ProtoBuf;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RegisterPanel : BasePanel
{
    private TMP_InputField account;
    private TMP_InputField password;
    private Button registerButton;
    private Button closeButton;

    public override void OnInit()
    {
        skinPath = "RegisterPanel";
        layer = PanelManager.Layer.Panel;
    }

    public override void OnShow(params object[] para)
    {
        account = skin.transform.Find("Account").GetComponent<TMP_InputField>();
        password = skin.transform.Find("Password").GetComponent<TMP_InputField>();
        registerButton = skin.transform.Find("Register Button").GetComponent<Button>();
        closeButton = skin.transform.Find("Close Button").GetComponent<Button>();

        registerButton.onClick.AddListener(() => {
            if (account.text == "" || password.text == "")
            {
                PanelManager.Open<TipPanel>("用户名和密码不能为空");
                return;
            }
            MsgRegister msgRegister = new MsgRegister();
            msgRegister.id = account.text;
            msgRegister.pw = password.text;
            NetManager.Send(msgRegister);
        });

        closeButton.onClick.AddListener(() => {
            Close();
        });

        NetManager.AddMsgListener("proto.LoginMsg.MsgRegister", OnMsgRegister);
    }

    public override void OnClose()
    {
        NetManager.RemoveMsgListener("proto.LoginMsg.MsgRegister", OnMsgRegister);
    }

    private void OnMsgRegister(IExtensible msgBase)
    {
        MsgRegister msgRegister = msgBase as MsgRegister;
        if (msgRegister.result == 0)
        {
            PanelManager.Open<TipPanel>("注册成功");
            Close();
        }
        else
        {
            PanelManager.Open<TipPanel>("注册失败");
        }
    }
}