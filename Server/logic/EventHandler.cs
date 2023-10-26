using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multiplayer_Online_Tank_War
{
    public partial class EventHandler
    {
        public static void OnDisconnect(ClientState clientState)
        {
            if (clientState.player != null)
            {
                DataBaseManager.UpdatePlayerData(clientState.player.id, clientState.player.data);
                PlayerManager.RemovePlayer(clientState.player.id);
            }
        }

        public static void OnTimer() 
        {
            CheckPing();
        }

        public static void CheckPing()
        {
            long timeNow = NetManager.GetTimeStamp();
            foreach (ClientState clientState in NetManager.clients.Values)
            {
                if (clientState.lastPingTime == 0) continue;
                if (timeNow - clientState.lastPingTime > NetManager.pingInterval * 4)
                {
                    Console.WriteLine("Ping Time Out, Close Socket:" + clientState.socket.RemoteEndPoint.ToString());
                    Console.WriteLine("lastPingtime:" + clientState.lastPingTime);
                    NetManager.Close(clientState);
                    return;
                }
            }
        }
    }
}
