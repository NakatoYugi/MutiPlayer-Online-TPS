using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Multiplayer_Online_Tank_War
{
    public class ClientState
    {
        public Socket socket;
        public ByteArray readBuff;
        public long lastPingTime;
        public Player player;

        public ClientState(Socket socket)
        {
            this.socket=socket;
            readBuff = new ByteArray();
            lastPingTime = 0;
        }
    }
}
