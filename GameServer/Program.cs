using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Xml;
using Lidgren.Network;

namespace GameServer
{
    internal class Program
    {

        public static void Output(String s)
        {
            Console.WriteLine(s);
        }

        public static void Main(string[] args)
        {
            // TODO: Get port from constant or from commandline args
            var config = new NetPeerConfiguration("StrategoLAN") {Port = 11112};
            var server = new NetServer(config);
            var players = new Players();
            var game = new Game(server, players);
            
            game.server.Start();
            
            NetIncomingMessage message;
            
            // TODO: while(true) Should be replaced with Application.Idle handler!
            // TOOD: Test logging out
            while (true)
            {
                if((message = server.ReadMessage()) == null) continue;
                else game.state.HandleMessage(message);

            }
            
            // int x = 0;
            // while (x < 250)
            // {
            //     // var message = server.CreateMessage(String.Format("ServerLoop {0}", x));
            //     var message = server.CreateMessage();
            //     message.Write(x*1000);
            //     server.SendToAll(message, NetDeliveryMethod.ReliableOrdered);
            //     Console.WriteLine(String.Format("ServerLoop Sent{0}", x));
            //     Console.WriteLine(String.Format("Connections :{0}", server.ConnectionsCount));
            //     Thread.Sleep(500);
            //     x++;
            // }

            
            
            Console.WriteLine("Closing Server..");
            server.Shutdown("Timed Server Close");

        }
    }
}