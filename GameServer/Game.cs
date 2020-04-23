using System;
using System.Data;
using System.Linq;
using System.Xml;
using Common;
using Lidgren.Network;

namespace GameServer
{
    
    
    public abstract class GameState
    {
        
        
        
        protected Game game;
        public abstract void HandleMessage(NetIncomingMessage message);

        public GameState(Game game)
        {
            this.game = game;
        }
        
    }

    public class AwaitConnectionState : GameState
    {
        public AwaitConnectionState(Game game) : base(game)
        {
        }

        // Register new players if appilcable
        // Deny any other request
        // Move to awaitboardinit when two players are present
        public override void HandleMessage(NetIncomingMessage message)
        {
            if(message == null) return;
             
            switch (message.MessageType)
            {
                case NetIncomingMessageType.DebugMessage:
                case NetIncomingMessageType.ErrorMessage:
                case NetIncomingMessageType.WarningMessage:
                case NetIncomingMessageType.VerboseDebugMessage:
                    string text = message.ReadString();
                    Console.WriteLine(String.Format("Got message:{0}", text));
                    break;
                case NetIncomingMessageType.StatusChanged:
                    NetConnectionStatus status = (NetConnectionStatus)message.ReadByte();
                    string reason = message.ReadString();
                    Console.WriteLine(NetUtility.ToHexString(message.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);

                    var response = game.server.CreateMessage();
                    if (status == NetConnectionStatus.Connected)
                        response = game.players.SignUp(message.SenderConnection, response);
                    else if (status == NetConnectionStatus.Disconnecting)
                        response = game.players.SignOut(message.SenderConnection, response);

                    game.server.SendMessage(response, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                    break;
                default:
                    break;
            }

            if (game.players.GetConnection(Ownership.FirstPlayer) != null &&
                game.players.GetConnection(Ownership.SecondPlayer) != null)
            {
                game.state = new AwaitBoardInitializationState(game);
            }
        }
    }

    public class AwaitBoardInitializationState : GameState
    {

        public bool[] boardinit;
        public AwaitBoardInitializationState(Game game) : base(game)
        {
            // Send both players a notice to init their boards
            NetOutgoingMessage msg = game.CreateStatusMessage(ClientGameStates.WaitForBoard,
                "Waiting for players to place their pieces on the board..");
            game.server.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);

            this.boardinit = new[] {false, false};
        }
        
        public override void HandleMessage(NetIncomingMessage message)
        {
            if(message == null) return;
            NetOutgoingMessage response = game.server.CreateMessage();
             
            switch (message.MessageType)
            {
                case NetIncomingMessageType.DebugMessage:
                case NetIncomingMessageType.ErrorMessage:
                case NetIncomingMessageType.WarningMessage:
                case NetIncomingMessageType.VerboseDebugMessage:
                    string text = message.ReadString();
                    Console.WriteLine(String.Format("Got message:{0}", text));
                    break;
                case NetIncomingMessageType.StatusChanged when (NetConnectionStatus)message.ReadByte() == NetConnectionStatus.Disconnecting:
                    NetConnectionStatus status = (NetConnectionStatus)message.ReadByte();
                    string reason = message.ReadString();
                    Console.WriteLine(NetUtility.ToHexString(message.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);
                    response = game.players.SignOut(message.SenderConnection, response);
                    game.server.SendMessage(response, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                    break;
                case NetIncomingMessageType.Data:
                    Packet incoming_packet = game.packetfactory.ReadNetMessage(message);
                    // TODO: DEAL WITH THAT WHEN ICell SERIALIZATION WORKS
                    // mark the array and when both are true, do game.SendBoardToUsers() and move to next state
                    
                    
                    // if type if is BoardPacket:
                    // Ownership player = game.players.GetPlayerFromConnection(message.SenderConnection);
                    // if (game.board.InitBoardFromUser((ICell[,]) data.data, player))
                    // {
                    //     this.boardinit[(int) player - 1] = true;
                    //     response.WriteAllFields(new DataPacket("BoardInit", "Success! wait..."));
                    // }
                    // else
                    // {
                    //     response.WriteAllFields(new DataPacket("Error", "InitBoard"));
                    // }
                    //
                    // game.server.SendMessage(response, message.SenderConnection,
                    //     NetDeliveryMethod.ReliableOrdered);
                    //         
                    // if (boardinit.All(val => val))
                    // {
                    //     game.state = new AwaitPlayerStartState(game);
                    // }
                    
                    // if other type:
                    // Console.WriteLine("Unexpected data packet: HEADER: {0}  | DATA: {1}", data.header, data.data.ToString());
                    // response.WriteAllFields(new DataPacket("Error", "Unexpected packet"));

                    break;
                default:
                    break;
            }
        }
    }

    public class AwaitPlayerStartState : GameState
    {
        public bool[] playerstart;

        public AwaitPlayerStartState(Game game) : base(game)
        {
            // Send both players a notice to press "Start Game"
            NetOutgoingMessage msg = game.CreateStatusMessage(ClientGameStates.WaitForStart, "Waiting both players to press START");
            game.server.SendToAll(msg, NetDeliveryMethod.ReliableOrdered);
            this.playerstart = new[] {false, false};   
        }
        
       // When player i sends Start, tell player i to ack
       // When both players sent start, Move to state GameWaitFirstPlayerMoveState
        public override void HandleMessage(NetIncomingMessage message)
        {
            // TODO: properly handle other types
            if (message.MessageType != NetIncomingMessageType.Data)
            {
                Console.WriteLine("Unexecpected Message on wait start: {0}", message.ToString());
                return;
            }

            Packet incoming_packet = game.packetfactory.ReadNetMessage(message);
            switch (incoming_packet)
            {
                case PlayerReadyPacket p:
                    this.playerstart[(int) game.players.GetPlayerFromConnection(message.SenderConnection) - 1] = true;
                    if (this.playerstart.All(x => x))
                    {
                        game.state = new GameWaitFirstPlayerMoveState(game);
                        return;
                    }
                    else
                    {
                        var response = game.CreateStatusMessage(ClientGameStates.WaitForStart, "waiting for other user..");
                        game.server.SendMessage(response, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);

                        var notice = game.CreateStatusMessage(ClientGameStates.WaitForStart,
                            "Waiting just for you! press START to begin");
                        game.server.SendMessage(notice, game.players.GetOtherUserConnection(message.SenderConnection),
                            NetDeliveryMethod.ReliableOrdered);
                    }
                    return;
                
                default:
                    // TODO: SEND PACKET OF ERROR
                    break;
            }
            


        }
    }

    // Generic for both players
    // FirstPlayer just calls base(game, FirstPlayer) etc
    public abstract class WaitMove : GameState
    {
        protected Ownership player;
        protected WaitMove(Game game, Ownership player) : base(game)
        {
            this.player = player;
            
            // Let player know we are waiting
            var this_player_message = game.CreateStatusMessage(ClientGameStates.YourMove, info: "");
            game.server.SendMessage(this_player_message, game.players.GetConnection(player),
                NetDeliveryMethod.ReliableOrdered);

            // Let other player know what's going on
            var other_player_message = game.CreateStatusMessage(ClientGameStates.WaitOtherPlayerMove, info: "");
            game.server.SendMessage(other_player_message, game.players.GetOtherUserConnection(player),
                NetDeliveryMethod.ReliableOrdered);
            
            game.server.FlushSendQueue();
        }

        public override void HandleMessage(NetIncomingMessage message)
        {
            // TODO: HANDLE ME!
            if(message.MessageType != NetIncomingMessageType.Data) return;

            Packet packet = game.packetfactory.ReadNetMessage(message);
            if (game.players.GetPlayerFromConnection(message.SenderConnection) != this.player)
            {
                game.server.SendMessage(game.CreateStatusMessage(ClientGameStates.Error, "Wait for other player to play"), message.SenderConnection, NetDeliveryMethod.Unknown);
                return;
            }
            // TODO: IF USER IS SecondPlayer, FLIP COORDINANTES!!

            switch (packet)
            {
                case AttemptMovePacket movePacket:
                    if (game.board.VerifyMove(movePacket.origin, movePacket.dest))
                    {
                        ICell attacker = game.board.SampleLocation(movePacket.origin, player);
                        ICell defender = game.board.SampleLocation(movePacket.dest, Players.OtherPlayer(player));
                        game.board.ApplyValidMove(movePacket.origin, movePacket.dest);
                        ICell winner_piece = game.board.SampleLocation(movePacket.dest, player);
                        
                        // Let players know of result
                        switch (defender)
                        {
                            case EmptyCell e:
                                break;
                            case WaterCell w:
                                throw new ArgumentException("HandleMessage: can't attack WaterCell");
                            case Flag f:
                                game.server.SendMessage(game.CreateStatusMessage(ClientGameStates.YourMove, string.Format("{0} attacks {1}. Game Over.",
                                    ((Piece) attacker).GetRank().ToString(), ((Piece) defender).GetRank().ToString())), message.SenderConnection, NetDeliveryMethod.ReliableOrdered) ;
                                game.server.SendMessage(game.CreateStatusMessage(ClientGameStates.WaitOtherPlayerMove,
                                        string.Format("{0} attacks {1}. Game Over.",
                                            ((Piece) attacker).GetRank().ToString(),
                                            ((Piece) defender).GetRank().ToString())),
                                    game.players.GetConnection(Players.OtherPlayer(player)),
                                    NetDeliveryMethod.ReliableOrdered);
                                game.server.FlushSendQueue();
                                game.state = new GameOverState(game);
                                return;
                            case Piece p:
                                var summary = string.Format("{0} attacks {1} .",
                                    ((Piece) attacker).GetRank().ToString(), ((Piece) defender).GetRank().ToString());
                                var result = winner_piece.GetOwnership() == Ownership.Board
                                    ? "Draw, both pieces are eliminated from the board"
                                    : String.Format("Winner is: {0}",
                                        (winner_piece.GetOwnership() == player
                                            ? ((Piece) attacker).GetRank().ToString()
                                            : ((Piece) defender).GetRank().ToString()));

                                
                                game.server.SendMessage(game.CreateStatusMessage(ClientGameStates.YourMove, summary + result), game.players.GetConnection(player),NetDeliveryMethod.ReliableOrdered);
                                game.server.SendMessage(game.CreateStatusMessage(ClientGameStates.WaitOtherPlayerMove, summary + result), game.players.GetConnection(Players.OtherPlayer(player)),NetDeliveryMethod.ReliableOrdered);
                                break;
                        }
                        
                        game.SendBoardToUsers();

                        if (game.board.CheckGameOver() != Ownership.Board)
                        {
                            game.state = new GameOverState(game);
                        }

                        else
                        {
                            var newState = (player == Ownership.FirstPlayer)
                                ? new GameWaitSecondPlayerMoveState(game)
                                : new GameWaitSecondPlayerMoveState(game);
                            game.state = newState;
                        }
                    }
                    else
                    {
                        var error_msg = game.CreateStatusMessage(ClientGameStates.Error, "Invalid move. try again");
                        game.server.SendMessage(error_msg, message.SenderConnection, NetDeliveryMethod.ReliableOrdered);
                        return;
                    }
                    break;
            }
        }
    }
    public class GameWaitFirstPlayerMoveState : WaitMove
    {
        public GameWaitFirstPlayerMoveState(Game game) : base(game, Ownership.FirstPlayer)
        {
        }
        
    }
    
    public class GameWaitSecondPlayerMoveState : WaitMove
    {
        public GameWaitSecondPlayerMoveState(Game game) : base(game, Ownership.SecondPlayer)
        {
        }
        
    }

    public class GameOverState : GameState
    {
        public GameOverState(Game game) : base(game)
        {
            var winner = game.board.CheckGameOver();
            if(winner == Ownership.Board) throw new ConstraintException("Can't GameOver from an ongoing game");
            // Greet winner, loser
            var greeting = "Game Over! You {0} replay or leave. waiting for players..";
            var winner_msg = game.CreateStatusMessage(ClientGameStates.GameOver, String.Format(greeting, "WIN!"));
            game.server.SendMessage(winner_msg, game.players.GetConnection(winner), NetDeliveryMethod.ReliableOrdered);
            var loser_msg = game.CreateStatusMessage(ClientGameStates.GameOver,
                String.Format(greeting, "LOSE! maybe next time..."));
            game.server.SendMessage(loser_msg, game.players.GetConnection(Players.OtherPlayer(winner)),
                NetDeliveryMethod.ReliableOrdered);
        }
        
        public override void HandleMessage(NetIncomingMessage message)
        {
            throw new NotImplementedException();
        } 
    }
    
    
    
    
    
    public class Game
    {
        public Players players;
        public ServerBoard board;
        public GameState state;
        public NetServer server;
        // TODO: probably shouldn't expose packet factory like that 
        public PacketFactoryFlyWheel packetfactory;

        public NetOutgoingMessage CreateStatusMessage(ClientGameStates state, string info)
        {
            Packet packet = this.packetfactory.GetPacketInstance(PacketHeader.ServerToClientGameStatusUpdatePacket);
            ((ServerToClientGameStatusUpdatePacket) packet).state = state;
            ((ServerToClientGameStatusUpdatePacket) packet).info = info;
            var message = server.CreateMessage();
            packet.PackIntoNetMessage(message);
            return message;
        }

        public void SendBoardToUsers()
        {
            for( Ownership player = Ownership.FirstPlayer ; player <= Ownership.SecondPlayer ; player++)
            {
                var message = server.CreateMessage();
                Packet board_packet = packetfactory.GetPacketInstance(PacketHeader.BoardPacket);
                ((BoardPacket) board_packet).BoardState = board.BoardFilterToUser(player);
                board_packet.PackIntoNetMessage(message);
                server.SendMessage(message, players.GetConnection(player), NetDeliveryMethod.ReliableOrdered);
            }
            server.FlushSendQueue();
            return;
        }


        public Game()
        {
            this.players = new Players();
            this.board = new ServerBoard();
            this.state = new AwaitConnectionState(this);
            this.server = null;
            this.packetfactory = new PacketFactoryFlyWheel();
        }

        public Game(NetServer server, Players players)
        {
            this.players = players;
            this.board = new ServerBoard();
            this.state = new AwaitConnectionState(this);
            this.server = server;
            this.packetfactory = new PacketFactoryFlyWheel();
        }

        public void Reset()
        {
            this.board = new ServerBoard();
            // TODO: Maybe some other state according to Client?
            this.state = new AwaitBoardInitializationState(this);
        }

        public void PrintState()
        {
            
            // Console.WriteLine("GameState: {0}", this.status.ToString());
            // // TODO: Add "if verbose"
            // switch (this.status)
            // {
            //     case GameStatus.AwaitConnection:
            //         Console.WriteLine(this.players.ToString());
            //         break;
            //     case GameStatus.AwaitBoardInitialization:
            //         Console.WriteLine("FirstPlayer: {0}, SecondPlayer: {1}", this.boardinit);
            //         break;
            //     case GameStatus.AwaitPlayerStart:
            //         Console.WriteLine("FirstPlayer: {0}, SecondPlayer: {1}", this.playerstart);
            //         break;
            //     case GameStatus.GameOver:
            //         Console.WriteLine("Winner is: NOTYETIMPLEMENTED");
            //         break;
            //     case GameStatus.GameWaitFirstPlayerMove:
            //     case GameStatus.GameWaitSecondPlayerMove:
            //         Console.WriteLine("No more info");
            //         break;
            //     default:
            //         Console.WriteLine("Unexpected status");
            //         break;
            // }
        }
        
        
        
        
        
    }
    
    
}