using System;
using System.Collections.Generic;
using System.Linq;

using Common;

namespace GameServer
{
    public class ServerBoard : Board
    {
        public void Reset()
        {
            this.State = new ICell[DefaultBoardSize, DefaultBoardSize];
            InitBoardSystemCells();
            return;
        }


        public Ownership CheckGameOver()
        {
            bool[] flags = new bool[2] {false, false};
            bool[] has_moveable = new bool[2] {false, false};

            foreach (var cell in State)
            {
                switch (cell)
                {
                    case Flag f:
                        flags[(int) f.GetOwnership() - 1] = true;
                        break;
                    case MovablePiece p when p.GetValidMoves(this, PositionOfCell(p)).Length > 0:
                        has_moveable[(int) p.GetOwnership() - 1] = true;
                        break;
                }

                // lazy stop when possible..
                if (flags.All(x => x) && has_moveable.All(x => x))
                {
                    return Ownership.Board;
                }
            }

            if (flags[(int) Ownership.FirstPlayer - 1] && has_moveable[(int) Ownership.FirstPlayer - 1])
            {
                return Ownership.FirstPlayer;
            }
            else return Ownership.SecondPlayer;

        }
        
        // TODO: Change signature?
        // TODO: TEST
        public Boolean ApplyValidMove(Position start, Position end)
        {
            // Hold both pieces
            // try:
            //     ask conflict handler for result
            // try:
            //     put the result at end position
            //    put an empty at start position
            // Notify whoever needs to know

            Piece attacker;
            ICell defender;
            try
            {
                attacker = (Piece) CellAtPos(start);
                defender = CellAtPos(end);
            }
            catch
            {
                return false;
            }
            
            if (attacker == null || defender == null) return false;

            ICell winner = ConflictHandler.Handle(attacker, defender);
            State[end.X, end.Y] = winner;
            State[start.X, start.Y] = new EmptyCell();
            
            // TODO: NOTIFY RESULT
            //     if(winner == attacker) : 
            //     if(winner == defender) :
            //     if(winner == empty_cell) : DRAW

            return true;
        }

        public ICell[,] BoardFilterToUser(Ownership player)
        {
            // icell.sample() the whole board according to player
            // flip positions for player2 if needed
            var ans = new ICell[DefaultBoardSize, DefaultBoardSize];

            for (int col = 0; col < DefaultBoardSize; col++)
            {
                for (int row = 0; row < DefaultBoardSize; row++)
                {
                    switch (player)
                    {
                        case Ownership.FirstPlayer:
                            ans[col, row] = State[col, row].Sample(player);
                            break;
                        case Ownership.SecondPlayer:
                            ans[DefaultBoardSize - 1 - col, DefaultBoardSize - 1 - row] = State[col, row].Sample(player
                            );
                            break;
                        default:
                            throw new ArgumentException("BoardFilterToUser for board user");
                    }
                }
            }

            return ans;
        }

        public Boolean InitBoardFromUser(ICell[,] input, Ownership player)
        {
            // make sure size is correct
            if (input.Length != DefaultPiecesRanks.Length)
            {
                return false;
            }

            // make sure all pieces are of the right type and Ownership
            var list = new List<Common.Rank>();
            var enumerator = input.GetEnumerator();
            enumerator.Reset();
            ICell item;
            while ((item = (ICell) enumerator.Current) != null)
            {
                switch (item)
                {
                    case Piece p when p.GetOwnership() == player:
                        list.Add(p.GetRank());
                        break;
                    default:
                        return false;
                }

                enumerator.MoveNext();
            }

            // make sure all pieces that should be there - are there
            var intersect = list.Intersect(DefaultPiecesRanks);
            if (intersect.Count() != DefaultPiecesRanks.Length)
            {
                return false;
            }

            // TODO: TEST BOARDFILPPING FOR PLAYER2

            for (int row = 0; row < 4; row++)
            {
                for (int col = 0; col < DefaultBoardSize; col++)
                {
                    switch(player)
                    {
                        case Ownership.FirstPlayer:
                            this.State[col, row] = input[col, row];
                            break;
                        case Ownership.SecondPlayer:
                            this.State[DefaultBoardSize - 1 - col, DefaultBoardSize - 1 - row] = input[col, row];
                            break;
                        default:
                            throw new ArgumentException("InitBoardFromUser unexpected player");
                    }
                }
            }
            
            return true;
        }

        private Boolean InitBoardSystemCells()
        {
            // Fills the middle two rows with the defaults ICells:
            // two Empty, two Water, two Empty, two Water, two Empty per row
            // Assumes classic board
            for (int row = 4; row < 6; row++)
            {
                for (int col = 0; col < DefaultBoardSize; col++)
                {
                    if (col / 2 % 2 == 0)
                    {
                        this.State[col, row] = new EmptyCell();
                    }
                    else
                    {
                        this.State[col, row] = new WaterCell();
                    }
                }
            }

            return true;
        }
    }
}