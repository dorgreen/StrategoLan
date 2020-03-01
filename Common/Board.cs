using System;

namespace Common
{

    public interface CellSampler
    {
        Position PositionOfCell(ICell cell);
        ICell SampleLocation(Position pos, Ownership player);
    }
    
    public class Board : CellSampler
    {
        private ICell[][] State;
        private ICell CellAtPos(Position pos)
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
    }
    
}