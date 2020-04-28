using System;
using System.Threading;
using Lidgren.Network;
using Common;

namespace CLIClient
{
    internal class Program
    {
        // TODO: replace me with real user input.
        // currently returns a dummy made-in-advance board config
        public static ICell[] GetUserBoard(Ownership player)
        {
            var ans = Board.GetDefaultPieces(player).ToArray();
            return ans;
        }


        // TODO: ATLEAST MOCK IT!
        private static Position getPositionFromUser(string title)
        {
            Console.WriteLine("Enter Position of {0}. Example: B 4", title);
            var input = Console.ReadLine();
            throw new NotImplementedException();
        }

        // TODO: foolproof, add a way to cancel, and also it's blocking and that's probably not good.
        private static void wait_for_player_start()
        {
            Console.WriteLine("Press 's' to start");
            while (Console.ReadKey().KeyChar != 's')
            {
            }
            return;
        }

        // TODO: DO ME!!!!!!!
        public static void GetMoveFromUser(AttemptMovePacket p)
        {
            p.origin.X = 3;
            p.origin.Y = 2;
            p.dest.X = 3;
            p.dest.Y = 3;
        }

        public static void Main(string[] args)
        {
            bool debug = true;
            
            var config = new NetPeerConfiguration("StrategoLAN");
            var client = new NetClient(config);
            client.Start();
            client.Connect(host: "127.0.0.1", port: 11112);
            Console.WriteLine("Connected!");
            
            var board = new ClientBoard();
            var factory = new PacketFactoryFlyWheel();
            NetIncomingMessage message;
            NetOutgoingMessage ans;
            Packet reply;
            var game_started = false;
            Ownership this_player = Ownership.Board;
            
            
            
            // TODO: while(true) Should be replaced with Application.Idle handler!
            while (true)
            {
                if ((message = client.ReadMessage()) != null)
                {
                    if (debug)
                    {
                        Console.WriteLine(String.Format("Message: {0}", message.ToString()));
                        Console.WriteLine(String.Format("Data: {0}", message.PeekString()));
                    }
                    
                    
                    if(message.MessageType == NetIncomingMessageType.Data)
                    {
                        Packet packet = factory.ReadNetMessage(message);
                        switch (packet)
                        {
                            case BoardPacket bp:
                                if (!game_started)
                                {
                                    board.SetBoardFromStringArray(bp.BoardState, force_creation: true);
                                }
                                else if (!board.SetBoardFromStringArray(bp.BoardState))
                                {
                                    Console.WriteLine("Error loading new board from server");
                                }
                                board.PrintBoard();
                                break;
                            case ServerToClientGameStatusUpdatePacket sp:
                                switch (sp.state)
                                {
                                    case ClientGameStates.InitialConnection when sp.info == Ownership.FirstPlayer.ToString() || sp.info == Ownership.SecondPlayer.ToString():
                                        if (this_player == Ownership.Board)
                                        {
                                            this_player = sp.info == Ownership.FirstPlayer.ToString()
                                                ? Ownership.FirstPlayer
                                                : Ownership.SecondPlayer;
                                            Console.WriteLine("You are connected as {0}", this_player.ToString());
                                        }
                                        else
                                        {
                                            // TODO: ADD "IF VERBOSE"
                                            Console.WriteLine("unexpected Initial Conecnction. signed as {0}, received confirm for {1}", this_player.ToString(), sp.info);
                                        }

                                        break;
                                    case ClientGameStates.YourMove when sp.info == "":
                                    case ClientGameStates.Error when sp.info == "Invalid move. try again":
                                        game_started = true;
                                        // Ask player for move and pack it in a packet to send
                                        reply = factory.GetPacketInstance(PacketHeader.AttemptMovePacket);
                                        GetMoveFromUser((AttemptMovePacket)reply);
                                        ans = client.CreateMessage();
                                        reply.PackIntoNetMessage(ans);
                                        client.SendMessage(ans, client.ServerConnection,
                                            NetDeliveryMethod.ReliableOrdered);
                                        break;
                                    case ClientGameStates.Error when sp.info == "Starting game..":
                                        game_started = true;
                                        break;
                                    case ClientGameStates.WaitForBoard
                                        when sp.info == "Waiting for players to place their pieces on the board..":
                                    case ClientGameStates.Error when sp.info == "Illegal board sent by user":
                                    case ClientGameStates.Error
                                        when sp.info == "Unexpected packet. Waiting for user to send board":
                                        // Ask player to select initial location, pack it and send it
                                        reply = factory.GetPacketInstance(PacketHeader.BoardPacket);
                                        var user_board = GetUserBoard(this_player);
                                        ((BoardPacket)reply).SetBoardState(user_board);
                                        ans = client.CreateMessage();
                                        reply.PackIntoNetMessage(ans);
                                        client.SendMessage(ans, NetDeliveryMethod.ReliableOrdered);
                                        break;
                                    case ClientGameStates.WaitForStart when sp.info != "waiting for other user..":
                                        // Ask player to START, pack it and send it
                                        wait_for_player_start();
                                        ans = client.CreateMessage();
                                        factory.GetPacketInstance(PacketHeader.PlayerReady).PackIntoNetMessage(ans);
                                        client.SendMessage(ans, NetDeliveryMethod.ReliableOrdered);
                                        break;
                                    case ClientGameStates.WaitOtherPlayerMove:
                                        game_started = true;
                                        Console.WriteLine("{0} : {1}", sp.state.ToString(), sp.info);
                                        break;
                                    default:
                                        Console.WriteLine("{0} : {1}", sp.state.ToString(), sp.info);
                                        break;
                                }
                                break;
                            default:
                                Console.WriteLine("Unexpected packet! {0}", packet.ToString());
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