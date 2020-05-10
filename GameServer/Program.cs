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
        }
    }
}