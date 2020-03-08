using System;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Security.AccessControl;
using Lidgren.Network;

namespace CLIClient
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var config = new NetPeerConfiguration("StrategoLAN");
            var client = new NetClient(config);
            client.Start();
            client.Connect(host: "127.0.0.1", port: 11112);

            NetIncomingMessage message;
            int x = 0;
            while (x < 1000)
            {
                if ((message = client.ReadMessage()) != null)
                {
                    Console.WriteLine(String.Format("Message: {0}", message.ToString()));
                    Console.WriteLine(String.Format("Data: {0}", message.PeekInt32()));
                }
                
                Console.WriteLine("sleep..");
                Thread.Sleep(300);
                x++;
            }

            Console.WriteLine("wake up, close all..");
            client.Disconnect("Timed leave");
            client.Shutdown("Leave");

        }
    }
}