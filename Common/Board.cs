using System;
using System.Linq;

namespace Common
{
    public interface CellSampler
    {
        Position PositionOfCell(ICell cell);
        ICell SampleLocation(Position pos, Ownership player);
    }
    
    public class Board : CellSampler
    {
        protected int DefaultBoardSize = 10;
        protected ICell[][] State;
        protected ICell CellAtPos(Position pos)
        {
            ICell ans;
            try
            {
                ans = State[pos.X][pos.Y];
            }
            catch
            {
                return null;
            }

            return ans;
        }

        //TODO: IMPLEMENT!
        public Position PositionOfCell(ICell cell)
        {
            throw new NotImplementedException("Board.PositionOfCell");
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