using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using proto.BattleMsg;
using proto.SysMsg;

public class TestProtobuf : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //±àÂë
        MsgMove msgMove = new MsgMove();
        msgMove.x = 250;
        byte[] bs = ProtocolManager.EncodeBody(msgMove);
        Debug.Log(System.BitConverter.ToString(bs));

        //½âÂë
        ProtoBuf.IExtensible m = ProtocolManager.DecodeBody("proto.BattleMsg.MsgMove", bs, 0, bs.Length);
        MsgMove m2 = m as MsgMove;
        Debug.Log(m2.x);

        byte[] namebytes = ProtocolManager.EncodeName(m2);

        int count;
        string name = ProtocolManager.DecodeName(namebytes, 0, out count);
        Debug.Log(name+ " " + count);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
