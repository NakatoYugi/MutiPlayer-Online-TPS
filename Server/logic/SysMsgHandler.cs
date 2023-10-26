using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multiplayer_Online_Tank_War
{
    public partial class MsgHandler
    {
        public static void MsgPing(ClientState clientState, ProtoBuf.IExtensible msgBase)
        {
            Console.WriteLine("MsgPing");
            clientState.lastPingTime = NetManager.GetTimeStamp();
            proto.SysMsg.MsgPong msgPong = new proto.SysMsg.MsgPong();
            NetManager.Send(clientState, msgPong);
        }

        public static void MsgPong(ClientState clientState, ProtoBuf.IExtensible msgBase)
        {
            Console.WriteLine("MsgPing");
        }
    }
}
