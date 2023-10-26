using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multiplayer_Online_Tank_War
{
    public class Player
    {
        public string id = "";
        public ClientState state;

        //临时数据
        public int x;
        public int y;
        public int z;

        //数据库数据
        public PlayerData data;

        public Player(ClientState state) 
        {
            this.state = state;
        }

        public void Send(ProtoBuf.IExtensible msgBase)
        {
            NetManager.Send(state, msgBase);
        }
    }
}
