using System;
using System.Net.NetworkInformation;
using System.Security.AccessControl;
using Common;
using NUnit.Framework;

namespace UnitTestConflictHandler
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void TestMinerBomb()
        {
            Piece att = new Miner(Ownership.FirstPlayer);
            Piece def = new Bomb(Ownership.SecondPlayer);
            ICell ans = ConflictHandler.Handle(att, def);
            Assert.True(att == ans);
        }

        [Test]
        public void TestMinerScout()
        {
            Piece att = new Miner(Ownership.FirstPlayer);
            Piece def = new Scout(Ownership.SecondPlayer);
            Assert.True(att == ConflictHandler.Handle(att, def));
        }

        [Test]
        public void TestMinerLieutenant()
        {
            Piece att = new Miner(Ownership.FirstPlayer);
            Piece def = new Lieutenant(Ownership.SecondPlayer);
            Assert.True(def == ConflictHandler.Handle(att, def));
        }

        [Test]
        public void TestMinerMiner()
        {
            Piece att = new Miner(Ownership.FirstPlayer);
            Piece def = new Miner(Ownership.SecondPlayer);
            Assert.True(ConflictHandler.Handle(att, def) is EmptyCell);
        }

        [Test]
        public void TestMinerEmpty()
        {
            Piece att = new Miner(Ownership.FirstPlayer);
            ICell def = new EmptyCell();
            Assert.True(att == ConflictHandler.Handle(att, def));
        }

        public void TestMinerFlag()
        {
            Piece att = new Miner(Ownership.FirstPlayer);
            ICell def = new Flag(Ownership.SecondPlayer);
            Assert.True(att == ConflictHandler.Handle(att, def));
        }
    }
}