using System;
using System.Xml;
using Common;
using Lidgren.Network;

namespace GameServer
{
    
    
    public abstract class GameState
    {
        
        
        
        protected Game game;
        public abstract void HandlePacket(NetIncomingMessage msg);

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
        public override void HandlePacket(NetIncomingMessage message)
        {
            if(message == null) return;
             
            switch (message.MessageType)
            {
                case NetIncomingMessageType.DebugMessage:
                case NetIncomingMessageType.ErrorMessage:
                case NetIncomingMessageType.WarningMessage:
                case NetIncomingMessageType.VerboseDebugMessage:
                    string text = message.ReadString();
                    Console.WriteLine(String.Format("Got msg:{0}", text));
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
            this.boardinit = new[] {false, false};
        }
        
        public override void HandlePacket(NetIncomingMessage msg)
        {
            throw new NotImplementedException();
        }
    }

    public class AwaitPlayerStartState : GameState
    {
        public bool[] playerstart;

        public AwaitPlayerStartState(Game game) : base(game)
        {
            // Send both players a notice to press "Start Game"
            this.playerstart = new[] {false, false};   
        }
        
       // When player i sends Start, tell player i to ack
       // When both players sent start, Move to state GameWaitFirstPlayerMoveState
        public override void HandlePacket(NetIncomingMessage msg)
        {
            throw new NotImplementedException();
        }
    }

    // Should probably be the same as wait for player2 move 
    // 
    public class GameWaitFirstPlayerMoveState : GameState
    {
        public GameWaitFirstPlayerMoveState(Game game) : base(game)
        {
        }
        
        public override void HandlePacket(NetIncomingMessage msg)
        {
            throw new NotImplementedException();
        }
    }
    
    public class GameWaitSecondPlayerMoveState : GameState
    {
        public GameWaitSecondPlayerMoveState(Game game) : base(game)
        {
        }
        
        public override void HandlePacket(NetIncomingMessage msg)
        {
            throw new NotImplementedException();
        }
    }

    public class GameOverState : GameState
    {
        public GameOverState(Game game) : base(game)
        {
        }
        
        public override void HandlePacket(NetIncomingMessage msg)
        {
            throw new NotImplementedException();
        } 
    }
    
    
    
    
    
    public class Game
    {
        public Players players;
        private ServerBoard board;
        public GameState state;
        public NetServer server;


        public Game()
        {
            this.players = new Players();
            this.board = new ServerBoard();
            this.state = new AwaitConnectionState(this);
            this.server = null;
        }

        public Game(NetServer server, Players players)
        {
            this.players = players;
            this.board = new ServerBoard();
            this.state = new AwaitConnectionState(this);
            this.server = server;
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