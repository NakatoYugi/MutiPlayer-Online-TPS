using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System;
using MySqlX.XDevAPI;

namespace Multiplayer_Online_Tank_War
{

    class MainClass
    {
        public static void Main(string[] args)
        {
            // See https://aka.ms/new-console-template for more information
            if (!DataBaseManager.Connect("game", "192.168.232.1", 3306, "root", "CJ3210086"))
                return;

            NetManager.StartLoop(8888);
            //DataBase Connect Fail: Host 'LAPTOP-SV70O04T' is not allowed to connect to this MySQL server

            Console.ReadLine();
        }
    }
}
