using System;
using System.ComponentModel;
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

    public struct Position
    {
        public int x;
        public int y;
    }

    public interface ICell
    {
        Position[] GetValidMoves(CellSampler cs, Position pos);
        Ownership GetOwnership();
        ICell Sample(Ownership asker);
    }

    // "Dummy" class represent Enemy Cells for client
    // TODO: maybe throw exception on Sample()?
    public class Enemy : StaticPiece
    {
        private Ownership owner;

        public Enemy(Ownership owner)
        {
            this.owner = owner;
        }

        public Enemy(Piece p)
        {
            Ownership owner = p.GetOwnership();
            if(owner == Ownership.Board) throw new InvalidCastException("Can't Enemy from boardpiece");
            this.owner = owner;
        }
        
        public new Ownership GetOwnership()
        {
            return this.owner;
        }
    }

    // For Water and Empty types
    public abstract class StaticPiece : ICell
    {
        public Position[] GetValidMoves(CellSampler cs, Position pos)
        {
            // TODO: maybe return only current position 'pos'
            return new Position[0];
        }

        public Ownership GetOwnership()
        {
            return Ownership.Board;
        }

        public ICell Sample(Ownership asker)
        {
            return this;
        }
    }

    public class WaterCell : StaticPiece
    {
    }

    public class EmptyCell : StaticPiece
    {
    }

    public abstract class Piece : ICell
    {
        private Ownership owner;

        public abstract Rank GetRank();

        public Piece(Ownership owner)
        {
            this.owner = owner;
        }

        public Position[] GetValidMoves(CellSampler cs, Position pos)
        {
            throw new System.NotImplementedException();
        }

        public Ownership GetOwnership()
        {
            return this.owner;
        }

        public ICell Sample(Ownership asker)
        {
            return asker == this.owner ? this : Enemy(this);
        }
    }

    public abstract class MovablePiece : Piece
    {
        protected MovablePiece(Ownership owner) : base(owner)
        {
        }

        // #TODO: IMPLEMENT ME 
        // TO BE OVERRIDDEN BY SCOUT TYPE
        public Position[] GetValidMoves(CellSampler cs, Position pos)
        {
            throw new System.NotImplementedException();
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

        // TODO: Scout walks in a different manner, handle that
        public Position[] GetValidMoves(CellSampler cs, Position pos)
        {
            throw new System.NotImplementedException();
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