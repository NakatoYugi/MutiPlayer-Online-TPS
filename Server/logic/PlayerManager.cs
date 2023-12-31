﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multiplayer_Online_Tank_War
{
    public class PlayerManager
    {
        static Dictionary<string, Player> players = new Dictionary<string, Player>();

        public static bool IsOnline(string id)
        {
            return players.ContainsKey(id);
        }

        public static Player GetPlayer(string id)
        {
            return players.ContainsKey(id) ? players[id] : null;
        }

        public static void AddPlayer(string id, Player player)
        {
            players.Add(id, player);
        }

        public static void RemovePlayer(string id) 
        {  
            players.Remove(id); 
        }
    }
}
