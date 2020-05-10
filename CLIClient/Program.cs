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
            
            Console.WriteLine("Press 'Q' to send default board");
            while (Console.ReadKey().KeyChar != 'q')
            {
            }
            
            var ans = Board.GetDefaultPieces(player).ToArray();
            return ans;
        }
        
        private static Position getPositionFromUser(string title)
        {
            Console.WriteLine("Enter Position of {0}. Example: B 4", title);
            int row, column;
            var input = Console.ReadLine();
            var tokens = input.Split(' ');
            if (tokens[0].Length != 1 || tokens[0][0] < 'A' || tokens[0][0] > 'Z' )
            {
                Console.WriteLine("Invalid input. try again");
                return getPositionFromUser(title);
            }

            column = tokens[0][0] - 'A';

            if (tokens[1].Length == 0 || tokens[1].Length > 2 || !Int32.TryParse(tokens[1], out row) || row > Board.DefaultBoardSize || row < 1)
            {
                Console.WriteLine("Invalid input. try again");
                return getPositionFromUser(title);
            }

            row = row - 1;

            return new Position(column, row);
        }

        // TODO: foolproof, add a way to cancel, and also it's blocking and that's probably not good.
        private static void WaitForPlayerStart()
        {
            Console.WriteLine("Press 's' to start");
            while (Console.ReadKey().KeyChar != 's')
            {
            }
            return;
        }

        public static void GetMoveFromUser(AttemptMovePacket p)
        {
            var origin = getPositionFromUser("the Piece you would like to move");
            var destination = getPositionFromUser("Where you would like to move the piece to");
            p.origin = origin;
            p.dest = destination;
        }

        public static async void UserGetMoveAndSend(AttemptMovePacket packet, NetClient client)
        {
            GetMoveFromUser((AttemptMovePacket)packet);
            var ans = client.CreateMessage();
            packet.PackIntoNetMessage(ans);
            client.SendMessage(ans, client.ServerConnection,
                NetDeliveryMethod.ReliableOrdered);
            packet.Recycle();
        }

        public static async void UserGetInitialBoardAndSend(BoardPacket packet, NetClient client, Ownership thisPlayer)
        {
            var user_board = GetUserBoard(thisPlayer);
            ((BoardPacket)packet).SetBoardState(user_board);
            var ans = client.CreateMessage();
            packet.PackIntoNetMessage(ans);
            client.SendMessage(ans, NetDeliveryMethod.ReliableOrdered);
            packet.Recycle();
        }

        public static async void UserGetStartSignalAndSend(PlayerReadyPacket packet, NetClient client)
        {
            WaitForPlayerStart();
            var ans = client.CreateMessage();
            packet.PackIntoNetMessage(ans);
            client.SendMessage(ans, NetDeliveryMethod.ReliableOrdered);
            packet.Recycle();
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
                        
                        // TODO: ADD IF DEBUG
                        Console.WriteLine("Got data packet: {0}", packet.ToString());
                        
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
                                
                                //TODO: add "if debug
                                Console.WriteLine("Got status message: {0}, {1}", sp.state.ToString(), sp.info);
                                
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
                                        UserGetMoveAndSend((AttemptMovePacket)factory.GetPacketInstance(PacketHeader.AttemptMovePacket),
                                            client);
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
                                        UserGetInitialBoardAndSend((BoardPacket)factory.GetPacketInstance(PacketHeader.BoardPacket), client, this_player);
                                        break;
                                    case ClientGameStates.WaitForStart when sp.info != "Waiting for other player to press start..":
                                        // Ask player to START, pack it and send it
                                        UserGetStartSignalAndSend((PlayerReadyPacket)factory.GetPacketInstance(PacketHeader.PlayerReady) , client);
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
                client.FlushSendQueue();

                // Console.WriteLine("sleep..");
                Thread.Sleep(100);
            }

            Console.WriteLine("wake up, close all..");
            client.Disconnect("Timed leave");
            client.Shutdown("Leave");
        }
    }
}