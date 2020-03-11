using System;
using System.Xml;
using Common;
using Lidgren.Network;

namespace GameServer
{
    public enum GameStatus
    {
        AwaitConnection,
        AwaitBoardInitialization,
        AwaitPlayerStart,
        GameWaitFirstPlayerMove,
        GameWaitSecondPlayerMove,
        GameOver
    }
    
    public class Game
    {
        public Players players;
        private ServerBoard board;
        public Ownership turn;
        public GameStatus status;
        public NetServer server;
        public bool[] boardinit;
        public bool[] playerstart;
        


        public Game()
        {
            this.players = new Players();
            this.board = new ServerBoard();
            this.turn = Ownership.FirstPlayer;
            this.status = GameStatus.AwaitConnection;
            this.server = null;
            this.boardinit = new[] {false, false};
            this.playerstart = new[] {false, false};
        }

        public Game(NetServer server)
        {
            this.players = new Players();
            this.board = new ServerBoard();
            this.turn = Ownership.FirstPlayer;
            this.status = GameStatus.AwaitConnection;
            this.server = server;
            this.boardinit = new[] {false, false};
            this.playerstart = new[] {false, false};
        }

        public void PrintState()
        {
            Console.WriteLine("GameState: {0}", this.status.ToString());
            // TODO: Add "if verbose"
            switch (this.status)
            {
                case GameStatus.AwaitConnection:
                    Console.WriteLine(this.players.ToString());
                    break;
                case GameStatus.AwaitBoardInitialization:
                    Console.WriteLine("FirstPlayer: {0}, SecondPlayer: {1}", this.boardinit);
                    break;
                case GameStatus.AwaitPlayerStart:
                    Console.WriteLine("FirstPlayer: {0}, SecondPlayer: {1}", this.playerstart);
                    break;
                case GameStatus.GameOver:
                    Console.WriteLine("Winner is: NOTYETIMPLEMENTED");
                    break;
                case GameStatus.GameWaitFirstPlayerMove:
                case GameStatus.GameWaitSecondPlayerMove:
                    Console.WriteLine("No more info");
                    break;
                default:
                    Console.WriteLine("Unexpected status");
                    break;
            }
        }
        
        
        
        
        
    }
    
    
}