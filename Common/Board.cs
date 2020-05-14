using System;
using System.Collections.Generic;
using System.Linq;
using Console = Colorful.Console;
using System.Drawing;

namespace Common
{
    public interface CellSampler
    {
        ICell SampleLocation(Position pos, Ownership player);
    }
    
    public class Board : CellSampler
    {
        public const int DefaultBoardSize = 10;
        protected ICell[] State;
        
        public static readonly Rank[] DefaultPiecesRanks =
        {
            Rank.Flag, Rank.Spy, Rank.Scout, Rank.Scout, Rank.Scout, Rank.Scout, Rank.Scout, Rank.Scout, Rank.Scout,
            Rank.Scout, Rank.Miner, Rank.Miner, Rank.Miner, Rank.Miner, Rank.Miner, Rank.Sergeant, Rank.Sergeant,
            Rank.Sergeant, Rank.Sergeant, Rank.Lieutenant, Rank.Lieutenant, Rank.Lieutenant, Rank.Lieutenant,
            Rank.Captain, Rank.Captain, Rank.Captain, Rank.Captain, Rank.Major, Rank.Major, Rank.Major,
            Rank.Colonel, Rank.Colonel, Rank.General, Rank.Marshal, Rank.Bomb, Rank.Bomb, Rank.Bomb, Rank.Bomb, Rank.Bomb,
            Rank.Bomb
        };

        public Board(ICell[] state)
        {
            State = state;
        }
        
        public Board(int board_size=DefaultBoardSize*DefaultBoardSize)
        {
            this.State = new ICell[board_size];
        }

        public static List<ICell> GetDefaultPieces(Ownership player)
        {
            if (player == Ownership.Board)
            {
                // TODO: Maybe copy here the code that does that
                //  and replace it with fun. call 
                return new List<ICell>();
            }

            var ans = DefaultPiecesRanks.Select(piece_rank => ICellTools.ICellFromRank(piece_rank, player));
            return new List<ICell>(ans);
            
        }
        
        protected ICell CellAtPos(Position pos)
        {
            ICell ans;
            try
            {
                ans = State[pos.to_board_index(DefaultBoardSize)];
            }
            catch
            {
                return null;
            }

            return ans;
        }

        public ICell SampleLocation(Position pos, Ownership player)
        {
            ICell cell = CellAtPos(pos);
            return cell == null ? null : cell.Sample(player);
        }

        public Boolean VerifyMove(Position start, Position end)
        {
            ICell attacker = CellAtPos(start);
            ICell defender = CellAtPos(end);
            if (attacker is null || attacker.GetOwnership() == Ownership.Board || defender is null ||
                defender.GetOwnership() == attacker.GetOwnership() || defender is WaterCell)
            {
                return false;
            }

            return attacker.GetValidMoves(this, start).Contains(end);
        }

        public static Color OwnershipToColor(Ownership owner)
        {
            switch (owner)
            {
                case Ownership.Board:
                    return Color.Gray;
                case Ownership.FirstPlayer:
                    return Color.Tomato;
                case Ownership.SecondPlayer:
                    return Color.DodgerBlue;
                default:
                    return Color.White;
            }
        }

        public static void CellFormaterPrinter(ICell cell)
        {
            //Console.Write("| ",);
            //Drawing.Color slateBlue = Color.FromName("SlateBlue");
            Console.Write("| ");
            Console.Write("{0}", cell.ToDisplayString(), OwnershipToColor(cell.GetOwnership()));
        }
        
        public void PrintBoard()
        {
            // Rows from 1 to 10
            // Columns from A to K
            // Each item is 3 chars, surrounded by "|" on either side
            // Color is given by Ownership: Gray for Board, Red for FirstPlayer, Blue for SecondPlayer
            // Due to print order, we first print the LAST rows going down
            Console.BackgroundColor = Color.Black;
            Console.ForegroundColor = Color.Bisque;
            var reverse_list = this.State.Reverse();
            for (int RowIndex = 10; RowIndex > 0; RowIndex--)
            {
                Console.WriteLine();
                // Print row number
                Console.Write(String.Format("{0}  ", RowIndex).Substring(0, 3));
                // Print the whole row, color-coded
                var row_enum = reverse_list.Skip(DefaultBoardSize-RowIndex).Take(DefaultBoardSize).Reverse();
                row_enum.Select(cell =>
                {
                    CellFormaterPrinter(cell);
                    return true;
                });
                Console.Write("|");
            }
            
            // Print the lower Column designations
            Console.Write("   ");
            for (char col_index = 'A'; col_index < 'K'; col_index++)
            {
                Console.Write("  {0}  ", col_index);
            }
            Console.WriteLine();
            
            Console.WriteLine("- - - - - - - - - - ");
            
        }


    }
    
}