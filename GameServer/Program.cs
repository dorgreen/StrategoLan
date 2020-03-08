using System;
using System.Threading;
using Lidgren.Network;

namespace GameServer
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            // TODO: Get port from constant or from commandline args
            var config = new NetPeerConfiguration("StrategoLAN") {Port = 11112};
            var server = new NetServer(config);

            server.Start();

            int x = 0;
            while (x < 250)
            {
                // var message = server.CreateMessage(String.Format("ServerLoop {0}", x));
                var message = server.CreateMessage();
                message.Write(x*1000);
                server.SendToAll(message, NetDeliveryMethod.ReliableOrdered);
                Console.WriteLine(String.Format("ServerLoop Sent{0}", x));
                Console.WriteLine(String.Format("Connections :{0}", server.ConnectionsCount));
                Thread.Sleep(500);
                x++;
            }

            
            
            Console.WriteLine("Closing Server..");
            server.Shutdown("Timed Server Close");

        }
    }
}