using System;
using System.Linq;
using Common;
using GameServer;
using NUnit.Framework;


namespace UnitTestServerBoard
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void BasicTestInitBoardFromFirstPlayer()
        {
            ServerBoard sb = new ServerBoard();
            Ownership player = Ownership.FirstPlayer;
            
            ICell[] default_pieces = Board.GetDefaultPieces(player).ToArray();
            string[] user_pieces_id_strings = default_pieces.Select(cell => cell.ToString()).ToArray();

            Assert.True(sb.InitBoardFromUser(user_pieces_id_strings, player));

            Position pos = new Position(0, 0); 
            int index = pos.to_board_index();
            int end_index = new Position(Board.DefaultBoardSize-1 , 1).to_board_index();

            for (; index <= end_index; index++)
            {
                pos = Position.PositionFromIndex(index);
                ICell expected = default_pieces[index];
                ICell from_board = sb.SampleLocation(pos, player);
                Assert.Zero(from_board.ToString().CompareTo(expected.ToString()));
            }
            
        }

        [Test]
        public void BasicTestInitBoardFromUserSecondPlayer()
        {
            ServerBoard sb = new ServerBoard();
            Ownership player = Ownership.SecondPlayer;
            
            ICell[] default_pieces = Board.GetDefaultPieces(player).ToArray();
            string[] user_pieces_id_strings = default_pieces.Select(cell => cell.ToString()).ToArray();

            Assert.True(sb.InitBoardFromUser(user_pieces_id_strings, player));

            Position pos = new Position(0, 0); 
            int index = pos.to_board_index();
            int end_index = new Position(Board.DefaultBoardSize-1 , 1).to_board_index();

            for (; index <= end_index; index++)
            {
                pos = Position.PositionFromIndex(index);
                pos.Flip();
                ICell expected = default_pieces[index];
                ICell from_board = sb.SampleLocation(pos, player);
                Assert.Zero(from_board.ToString().CompareTo(expected.ToString()));
            }
        }

        // [Test]
        // public void TestInitBoardFromUserFailOnMissingPiece()
        // {
        //     
        // }
        //
        // [Test]
        // public void TestInitBoardFromUserFailOnTooManyPieces()
        // {
        //     
        // }
        //
        // [Test]
        // public void TestInitBoardFromUserFailOnWrongUserPieces()
        // {
        //     
        // }
        
        
    }
}