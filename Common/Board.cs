using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Xml.Schema;

namespace Common
{
    public interface CellSampler
    {
        ICell SampleLocation(Position pos, Ownership player);
    }
    
    public class Board : CellSampler
    {
        
        public static readonly Rank[] DefaultPiecesRanks =
        {
            Rank.Flag, Rank.Spy, Rank.Scout, Rank.Scout, Rank.Scout, Rank.Scout, Rank.Scout, Rank.Scout, Rank.Scout,
            Rank.Scout, Rank.Miner, Rank.Scout, Rank.Scout, Rank.Scout, Rank.Scout, Rank.Sergeant, Rank.Sergeant,
            Rank.Sergeant, Rank.Sergeant, Rank.Lieutenant, Rank.Lieutenant, Rank.Lieutenant, Rank.Lieutenant,
            Rank.Captain, Rank.Captain, Rank.Captain, Rank.Captain, Rank.Major, Rank.Major, Rank.Major,
            Rank.Colonel, Rank.Colonel, Rank.Marshal, Rank.Bomb, Rank.Bomb, Rank.Bomb, Rank.Bomb, Rank.Bomb,
            Rank.Bomb
        };

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

        public const int DefaultBoardSize = 10;
        protected ICell[] State;
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
                defender.GetOwnership() == attacker.GetOwnership())
            {
                return false;
            }

            return attacker.GetValidMoves(this, start).Contains(end);
        }


    }
    
}