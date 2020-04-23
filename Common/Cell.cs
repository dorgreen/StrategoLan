using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace Common
{
    public enum Ownership
    {
        Board,
        FirstPlayer,
        SecondPlayer
    }

    public enum Rank
    {
        Flag,
        Spy,
        Scout,
        Miner,
        Sergeant,
        Lieutenant,
        Captain,
        Major,
        Colonel,
        General,
        Marshal,
        Bomb
    }

    public abstract class ICell
    {
        public abstract Position[] GetValidMoves(CellSampler cs, Position pos);
        public abstract Ownership GetOwnership();

        public abstract ICell Sample(Ownership asker);

        // Format as : { "ICell" : {"t":INSERT_TYPE_HERE, "o":INSERT_OWNER_HERE } }
        public string ToString()
        {
            return "{\"ICell\":{" +
                   String.Format("t:{0},o:{1}", this.GetType().ToString(), this.GetOwnership().ToString()) + "}}";
        }
    }

    class ICellTools
    {
        public static ICell ICellFromRank(Rank rank, Ownership owner)
        {
            ICell ans;
            switch (rank)
            {
                case Rank.Flag:
                    ans = new Flag(owner);
                    break;
                case Rank.Spy:
                    ans = new Spy(owner);
                    break;
                case Rank.Scout:
                    ans = new Scout(owner);
                    break;
                case Rank.Miner:
                    ans = new Miner(owner);
                    break;
                case Rank.Sergeant:
                    ans = new Sergeant(owner);
                    break;
                case Rank.Lieutenant:
                    ans = new Lieutenant(owner);
                    break;
                case Rank.Captain:
                    ans = new Captain(owner);
                    break;
                case Rank.Major:
                    ans = new Major(owner);
                    break;
                case Rank.Colonel:
                    ans = new Colonel(owner);
                    break;
                case Rank.General:
                    ans = new General(owner);
                    break;
                case Rank.Marshal:
                    ans = new Marshal(owner);
                    break;
                case Rank.Bomb:
                    ans = new Bomb(owner);
                    break;
                default:
                    ans = new EmptyCell(Ownership.Board);
                    break;
            }

            return ans;
        }
    }


    // For Water and Empty types
    public abstract class StaticPiece : ICell
    {
        public override Position[] GetValidMoves(CellSampler cs, Position pos)
        {
            // TODO: maybe return only current position 'pos'
            return new Position[0];
        }

        public override Ownership GetOwnership()
        {
            return Ownership.Board;
        }

        public override ICell Sample(Ownership asker)
        {
            return this;
        }
    }

    public class WaterCell : StaticPiece
    {
        public WaterCell(Ownership owner) : base()
        {
        }

        public WaterCell() : base()
        {
        }
    }

    public class EmptyCell : StaticPiece
    {
        public EmptyCell(Ownership owner) : base()
        {
        }

        public EmptyCell() : base()
        {
        }
    }

    // "Dummy" class represent Enemy Cells for client
    // TODO: maybe throw exception on Sample()?
    public class Enemy : StaticPiece
    {
        private Ownership owner;

        public Enemy(Ownership owner)
        {
            if (owner == Ownership.Board) throw new InvalidCastException("Can't Enemy from boardpiece");
            this.owner = owner;
        }

        public new Ownership GetOwnership()
        {
            return this.owner;
        }
    }

    public abstract class Piece : ICell
    {
        private Ownership owner;

        public abstract Rank GetRank();

        public Piece(Ownership owner)
        {
            this.owner = owner;
        }

        public override Position[] GetValidMoves(CellSampler cs, Position pos)
        {
            throw new System.NotImplementedException();
        }

        public override Ownership GetOwnership()
        {
            return this.owner;
        }

        public override ICell Sample(Ownership asker)
        {
            ICell ans;

            if (this.GetOwnership() == asker || this.GetOwnership() == Ownership.Board)
                ans = this;
            else
            {
                ans = new Enemy(this.owner);
            }

            return ans;
        }
    }

    public abstract class MovablePiece : Piece
    {
        protected MovablePiece(Ownership owner) : base(owner)
        {
        }

        // #TODO: TEST ME 
        // TO BE OVERRIDDEN BY SCOUT TYPE
        public new Position[] GetValidMoves(CellSampler cs, Position pos)
        {
            var ans = new List<Position>();
            foreach (var d in Enum.GetValues(typeof(Directions)))
            {
                Position candidate = new Position(pos, (Directions) d);
                ICell CellInPosition = cs.SampleLocation(candidate, this.GetOwnership());
                if (CellInPosition is EmptyCell || CellInPosition is Enemy) ans.Add(candidate);
            }

            return ans.ToArray();
        }
    }

    public abstract class ImmovablePiece : Piece
    {
        protected ImmovablePiece(Ownership owner) : base(owner)
        {
        }

        // TODO: maybe return only current position 'pos'
        public Position[] GetValidMoves(CellSampler cs, Position pos)
        {
            return new Position[0];
        }
    }

    public class Flag : ImmovablePiece
    {
        public Flag(Ownership owner) : base(owner)
        {
        }

        public override Rank GetRank()
        {
            return Rank.Flag;
        }
    }

    public class Spy : MovablePiece
    {
        public Spy(Ownership owner) : base(owner)
        {
        }

        public override Rank GetRank()
        {
            return Rank.Spy;
        }
    }

    public class Scout : MovablePiece
    {
        public Scout(Ownership owner) : base(owner)
        {
        }

        public override Rank GetRank()
        {
            return Rank.Scout;
        }

        // Scout walks in a different manner:
        // can walk howmany empty spots in one direction
        // cannot skip enemies or water pieces
        // can attack an enemy within X empty spaces in one direction 
        public new Position[] GetValidMoves(CellSampler cs, Position pos)
        {
            var ans = new List<Position>();
            foreach (var d in Enum.GetValues(typeof(Directions)))
            {
                Position candidate = new Position(pos, (Directions) d);
                ICell CellInPosition = cs.SampleLocation(candidate, this.GetOwnership());

                while (CellInPosition is EmptyCell)
                {
                    ans.Add(candidate);
                    candidate = new Position(candidate, (Directions) d);
                    CellInPosition = cs.SampleLocation(candidate, this.GetOwnership());
                }

                if (CellInPosition is Enemy) ans.Add(candidate);
            }

            return ans.ToArray();
        }
    }

    public class Miner : MovablePiece
    {
        public Miner(Ownership owner) : base(owner)
        {
        }

        public override Rank GetRank()
        {
            return Rank.Miner;
        }
    }

    public class Sergeant : MovablePiece
    {
        public Sergeant(Ownership owner) : base(owner)
        {
        }

        public override Rank GetRank()
        {
            return Rank.Sergeant;
        }
    }

    public class Lieutenant : MovablePiece
    {
        public Lieutenant(Ownership owner) : base(owner)
        {
        }

        public override Rank GetRank()
        {
            return Rank.Lieutenant;
        }
    }

    public class Captain : MovablePiece
    {
        public Captain(Ownership owner) : base(owner)
        {
        }

        public override Rank GetRank()
        {
            return Rank.Captain;
        }
    }

    public class Major : MovablePiece
    {
        public Major(Ownership owner) : base(owner)
        {
        }

        public override Rank GetRank()
        {
            return Rank.Major;
        }
    }

    public class Colonel : MovablePiece
    {
        public Colonel(Ownership owner) : base(owner)
        {
        }

        public override Rank GetRank()
        {
            return Rank.Colonel;
        }
    }

    public class General : MovablePiece
    {
        public General(Ownership owner) : base(owner)
        {
        }

        public override Rank GetRank()
        {
            return Rank.General;
        }
    }

    public class Marshal : MovablePiece
    {
        public Marshal(Ownership owner) : base(owner)
        {
        }

        public override Rank GetRank()
        {
            return Rank.Marshal;
        }
    }

    public class Bomb : ImmovablePiece
    {
        public Bomb(Ownership owner) : base(owner)
        {
        }

        public override Rank GetRank()
        {
            return Rank.Bomb;
        }
    }
}