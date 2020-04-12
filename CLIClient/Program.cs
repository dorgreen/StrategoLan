using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using System.Security.AccessControl;
using Lidgren.Network;
using Common;

namespace CLIClient
{
    internal class Program
    {
        // TODO: replace me with real user input.
        // currently returns a dummy made-in-advance board config
        public static ICell[,] GetUserBoard(Ownership player)
        {
            var ans = new ICell[10, 4];
            Stack<ICell> PiecesToBePlaced = new Stack<ICell>(Board.GetDefaultPieces(player));
            for (int column = 0; column < 10; column++)
            {
                for (int row = 0; row < 4; row++)
                {
                    ans[column, row] = PiecesToBePlaced.Pop();
                }
            }
            
            return ans;
        }

        public static void Main(string[] args)
        {
            var config = new NetPeerConfiguration("StrategoLAN");
            var client = new NetClient(config);
            client.Start();
            client.Connect(host: "127.0.0.1", port: 11112);

            NetIncomingMessage message;
            NetOutgoingMessage ans;
            DataPacket reply;
            
            // TODO: while(true) Should be replaced with Application.Idle handler!
            while (true)
            {
                if ((message = client.ReadMessage()) != null)
                {
                    Console.WriteLine(String.Format("Message: {0}", message.ToString()));
                    Console.WriteLine(String.Format("Data: {0}", message.PeekString()));
                    
                    if(message.MessageType == NetIncomingMessageType.Data)
                    {
                        // TODO: should use flywheel on the real client :)
                        DataPacket packet = new DataPacket();
                        message.ReadAllFields(packet);
                        switch (packet.header)
                        {
                            case "Waiting for user to place pieces on board":
                                ans = client.CreateMessage();
                                reply = new DataPacket("BoardInit", GetUserBoard(Ownership.FirstPlayer));
                                ans.WriteAllFields(reply);
                                client.SendMessage(ans, client.ServerConnection, NetDeliveryMethod.ReliableOrdered);
                                break;
                            
                            case "Waiting for user press START":
                                ans = client.CreateMessage();
                                reply = new DataPacket("start!", "");
                                client.SendMessage(ans, client.ServerConnection, NetDeliveryMethod.ReliableOrdered);
                                break;
                            default:
                                break;
                        }
                    }
                }

                // Console.WriteLine("sleep..");
                Thread.Sleep(100);
            }

            Console.WriteLine("wake up, close all..");
            client.Disconnect("Timed leave");
            client.Shutdown("Leave");
        }
    }
}