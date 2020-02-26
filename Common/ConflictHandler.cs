using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;

namespace Common
{
    public class PieceConflictHandlerException : Exception
    {
        public PieceConflictHandlerException(string message) : base(message)
        {
        }
    }

    public static class ConflictHandler
    {
        private static ICell SimpleHandler(Piece attacker, Piece defender)
        {
            ICell ans;
            // Go by enum value
            if (attacker.GetRank() == defender.GetRank())
            {
                ans = new EmptyCell();
            }
            else if (attacker.GetRank() > defender.GetRank())
            {
                ans = attacker;
            }
            else ans = defender;

            return ans;
        }

        // Should return either attacker, defender or empty
        // Assuming that if attacker is movable its movement is legal
        public static ICell Handle(Piece attacker, ICell defender)
        {
            if (attacker.GetOwnership() == defender.GetOwnership() || attacker.GetOwnership() == Ownership.Board ||
                !(attacker is MovablePiece) || defender is WaterCell)
                throw new PieceConflictHandlerException("Illegal Attacker or Defender");

            if (defender is EmptyCell)
            {
                return attacker;
            }

            if (defender is Piece)
            {
                ICell ans;
                switch (((Piece) defender).GetRank())
                {
                    case Rank.Bomb:
                        ans = attacker.GetRank() == Rank.Miner ? attacker : defender;
                        break;
                    case Rank.Flag:
                        // TODO: alert game over?
                        // If not, could handled by default case
                        ans = attacker;
                        break;
                    case Rank.Marshal:
                        ans = attacker.GetRank() == Rank.Spy ? attacker : SimpleHandler(attacker, (Piece) defender);
                        break;

                    default:
                        ans = SimpleHandler(attacker, (Piece) defender);
                        break;
                }
                return ans;
            }

            else
            {
                throw new PieceConflictHandlerException("Unexpected Defender");
            }
        }
    }
}