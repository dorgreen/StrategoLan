public enum CellSample
{
    Empty,
    Water,
    Friend,
    Enemy,
    Invalid
}

namespace Common
{
    public class Board
    {
        private ICell[][] State;
    }

    // TODO: HOW IS THE RIGHT WAY TO DESIGN AND IMPLEMENT THIS????
    public class CellSampler
    {
        private Board board;
        
        public CellSampler(Board board)
        {
            this.board = board;
        }

    }
}