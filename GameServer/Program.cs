using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Xml;
using Lidgren.Network;
using Common;

namespace GameServer
{

    internal class Players
    {
        private NetConnection[] _Players;
        public Players()
        {
            this._Players = new NetConnection[2];
        }

        // Should return a message
        public NetOutgoingMessage SignUp(NetConnection user, NetOutgoingMessage msg)
        {
            lock (_Players)
            {
                if (_Players.Contains(user)){
                    msg.Write("AlreadyConnected!");
                    return msg;
                }

                int player_index = 0;
                while (player_index < _Players.Length)
                {
                    if (_Players[player_index] == null)
                    {
                        _Players[player_index] = user;
                        msg.Write(((Ownership)(player_index+1)).ToString());
                        Console.WriteLine(String.Format("Signed user {0} to Player {1}", user.ToString(), ((Ownership)(player_index+1)).ToString()));
                        break;
                    }
                    player_index++;
                }
                if(player_index == _Players.Length)
                    msg.Write("Two Players already connected!");
            }

            return msg;
        }

        public NetOutgoingMessage SignOut(NetConnection user, NetOutgoingMessage msg)
        {
            lock (_Players)
            {
                int player_index = 0;
                while (player_index < _Players.Length)
                {
                    if (_Players[player_index] == user)
                    {
                        _Players[player_index] = null;
                        msg.Write("Sign out: "+((Ownership)(player_index+1)).ToString());
                        Console.WriteLine(String.Format("Signed out user {0} to Player {1}", user.ToString(), ((Ownership)(player_index+1)).ToString()));
                        break;
                    }
                    player_index++;
                }
                if(player_index == _Players.Length)
                    msg.Write("Can't Signout, No such user!");

                return msg;
            }
        }

        public NetConnection GetConnection(Ownership player)
        {
            lock (_Players)
            {
                return _Players[(int) player - 1];   
            }
        }


    }

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
            Players players = new Players();

            server.Start();
            
            NetIncomingMessage message;
            NetOutgoingMessage response;

            // TODO: while(true) Should be replaced with Application.Idle handler!
            while (true)
            {
                if((message = server.ReadMessage()) == null) continue;
                switch (message.MessageType)
                {
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
                        string text = message.ReadString();
                        Output(String.Format("Got msg:{0}", text));
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)message.ReadByte();
                        string reason = message.ReadString();
                        Output(NetUtility.ToHexString(message.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);

                        response = server.CreateMessage();
                        if (status == NetConnectionStatus.Connected)
                            response = players.SignUp(message.SenderConnection, response);
                        else if (status == NetConnectionStatus.Disconnecting)
                            response = players.SignOut(message.SenderConnection, response);

                        server.SendMessage(response, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                        break;
                    default:
                        break;
                        
                }
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