using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Common;

namespace GameServer
{
    public class ServerBoard : Board
    {
        public ServerBoard() : base()
        {
            InitBoardSystemCells();
        }

        public void Reset()
        {
            this.State = new ICell[DefaultBoardSize * DefaultBoardSize];
            InitBoardSystemCells();
            return;
        }


        // Retruns the winner or Board if game is ongoing
        public Ownership CheckGameOver()
        {
            bool[] flags = new bool[2] {false, false};
            bool[] has_moveable = new bool[2] {false, false};

            ICell cell;
            for (int index = 0; index < DefaultBoardSize * DefaultBoardSize; index++)
            {
                cell = State[index];
                switch (cell)
                {
                    case Flag f:
                        flags[(int) f.GetOwnership() - 1] = true;
                        break;
                    case MovablePiece p
                        when p.GetValidMoves(this, Position.PositionFromIndex(index, DefaultBoardSize)).Length > 0:
                        has_moveable[(int) p.GetOwnership() - 1] = true;
                        break;
                }

                // lazy stop when possible..
                if (flags[0] && flags[1] && has_moveable[0] && has_moveable[1])
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
            State[end.to_board_index(DefaultBoardSize)] = winner;
            State[start.to_board_index(DefaultBoardSize)] = new EmptyCell();
            return true;
        }

        public string[] BoardFilterToUser(Ownership player)
        {
            // icell.sample() the whole board according to player
            // flip positions for player2 if needed
            var ans = this.State.Select(x => x.Sample(player).ToString()).ToArray();
            if (player == Ownership.SecondPlayer) ans = ans.Reverse().ToArray();
            return ans;
        }

        public Boolean InitBoardFromUser(string[] input, Ownership player)
        {
            // make sure size is correct and all are all indeed valid
            if (input.Length != DefaultPiecesRanks.Length || input.Any(id => !ICellTools.IsStringICell(id)))
            {
                return false;
            }

            // make sure all pieces are of the right type and Ownership
            var user_icells = input.Select(x => ICellTools.ICellFromString(x)).ToArray();
            var user_icells_ranklist = new List<Rank>();
            var enumerator = user_icells.GetEnumerator();
            enumerator.Reset();
            if(!enumerator.MoveNext()) throw  new EvaluateException("InitBoardFromUser");
            ICell item;
            while ((item = (ICell) enumerator.Current) != null)
            {
                switch (item)
                {
                    case Piece p when p.GetOwnership() == player:
                        user_icells_ranklist.Add(p.GetRank());
                        break;
                    default:
                        return false;
                }

                if(!enumerator.MoveNext()) break;
            }

            // make sure all pieces that should be there - are there
            var ordered_user_ranklist = user_icells_ranklist.OrderBy(rank => (int) rank);

            if (!DefaultPiecesRanks
                .Zip(ordered_user_ranklist, (expected_rank, given_rank) => expected_rank == given_rank).All(x => x))
            {
                return false;
            }


            switch (player)
            {
                case Ownership.FirstPlayer:
                    user_icells.CopyTo(State, 0);
                    break;
                case Ownership.SecondPlayer:
                    int insert_index = Position.CoordinatesToIndex(0, DefaultBoardSize - 4);
                    user_icells.Reverse().ToArray().CopyTo(State, insert_index);
                    break;
                default:
                    return false;
            }

            return true;
        }

        private void InitBoardSystemCells()
        {
            // Fills the middle two rows with the defaults ICells:
            // two Empty, two Water, two Empty, two Water, two Empty per row
            // Assumes classic board
            for (int index = DefaultBoardSize * 4; index < DefaultBoardSize * 6 ; index++)
            {
                if ((index % DefaultBoardSize) / 2 % 2 == 0)
                {
                    State[index] = new EmptyCell();
                }
                else
                {
                    State[index] = new WaterCell();    
                }
            }
        }
    }
}