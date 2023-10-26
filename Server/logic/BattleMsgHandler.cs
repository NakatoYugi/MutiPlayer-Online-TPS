using proto.BattleMsg;
using System;

namespace Multiplayer_Online_Tank_War
{
    public partial class MsgHandler
    {
        public static void MsgMove(ClientState clientState, ProtoBuf.IExtensible msgBase)
        {
            MsgMove msgMove = msgBase as MsgMove;
            Console.WriteLine(string.Format("MsgMove:x {0},y {1},z {2}", msgMove.x, msgMove.y, msgMove.z));

            msgMove = new proto.BattleMsg.MsgMove();
            msgMove.x = 1000;
            NetManager.Send(clientState, msgMove);
        }
    }
}
