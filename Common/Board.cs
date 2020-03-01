using System;

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
        public ICell CellAtPos(Position pos)
        {
            return State[pos.x][pos.y];
        }
    }
    
    
    // TODO: HOW IS THE RIGHT WAY TO DESIGN AND IMPLEMENT THIS????
    public class CellSampler
    {
        private Board board;
        
        public CellSampler(Board board)
        {
            this.board = board;
        }

        // For a given pos, tell of this is Empty, Water, Friend, Enemy or Invalid (e.g not in field etc)
        // TODO: maybe create a sampler for each player?
        public CellSample sample(Ownership player, Position pos)
        {
            ICell icell = board.CellAtPos(pos);
            CellSample ans;
            switch(icell)
            {
                case EmptyCell empty:
                    
                    
            }
        }

    }
}