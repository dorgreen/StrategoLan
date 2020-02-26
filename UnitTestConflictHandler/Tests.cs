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
            Assert.Equals(att, ans);
        }
        
        [Test]
        public void TestMinerScout()
        {
            Piece att = new Miner(Ownership.FirstPlayer);
            Piece def = new Scout(Ownership.SecondPlayer);
            Assert.Equals(att, ConflictHandler.Handle(att,def));
        }
        
        [Test]
        public void TestMinerLieutenant()
        {
            Piece att = new Miner(Ownership.FirstPlayer);
            Piece def = new Lieutenant(Ownership.SecondPlayer);
            Assert.Equals(def, ConflictHandler.Handle(att,def));
        }
        
        [Test]
        public void TestMinerMiner()
        {
            Piece att = new Miner(Ownership.FirstPlayer);
            Piece def = new Miner(Ownership.SecondPlayer);
            Assert.True(ConflictHandler.Handle(att,def) is EmptyCell);
        }
        
        [Test]
        public void TestMinerEmpty()
        {
            Piece att = new Miner(Ownership.FirstPlayer);
            ICell def = new EmptyCell();
            Assert.Equals(att, ConflictHandler.Handle(att,def));
        }
        
        public void TestMinerFlag()
        {
            Piece att = new Miner(Ownership.FirstPlayer);
            ICell def = new Flag(Ownership.SecondPlayer);
            Assert.Equals(att, ConflictHandler.Handle(att,def));
        }
        
        
        
        
        
        
    }
}