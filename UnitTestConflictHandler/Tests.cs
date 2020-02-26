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
        public void TestMarshalMarshal()
        {
            Piece att = new Marshal(Ownership.FirstPlayer);
            Piece def = new Marshal(Ownership.SecondPlayer);
            Assert.True(ConflictHandler.Handle(att, def) is EmptyCell);
        }
        
        [Test]
        public void TestColonelColonel()
        {
            Piece att = new Colonel(Ownership.FirstPlayer);
            Piece def = new Colonel(Ownership.SecondPlayer);
            Assert.True(ConflictHandler.Handle(att, def) is EmptyCell);
        }
        

        [Test]
        public void TestMinerEmpty()
        {
            Piece att = new Miner(Ownership.FirstPlayer);
            ICell def = new EmptyCell();
            Assert.True(att == ConflictHandler.Handle(att, def));
        }

        [Test]
        public void TestMinerFlag()
        {
            Piece att = new Miner(Ownership.FirstPlayer);
            ICell def = new Flag(Ownership.SecondPlayer);
            Assert.True(att == ConflictHandler.Handle(att, def));
        }

        [Test]
        public void TestMarshalSpy()
        {
            Piece att = new Marshal(Ownership.FirstPlayer);
            ICell def = new Spy(Ownership.SecondPlayer);
            Assert.True(att == ConflictHandler.Handle(att, def));
        }

        [Test]
        public void TestSpyMarshal()
        {
            Piece att = new Spy(Ownership.FirstPlayer);
            ICell def = new Marshal(Ownership.SecondPlayer);
            Assert.True(att == ConflictHandler.Handle(att, def));
        }

        [Test]
        public void TestMarshalBomb()
        {
            Piece att = new Marshal(Ownership.FirstPlayer);
            ICell def = new Bomb(Ownership.SecondPlayer);
            Assert.True(def == ConflictHandler.Handle(att, def));
        }

        [Test]
        public void TestMarshalMajor()
        {
            Piece att = new Marshal(Ownership.SecondPlayer);
            ICell def = new Major(Ownership.FirstPlayer);
            Assert.True(att == ConflictHandler.Handle(att, def));
        }

        [Test]
        public void TestSpyEmpty()
        {
            Piece att = new Spy(Ownership.FirstPlayer);
            ICell def = new EmptyCell();
            Assert.True(att == ConflictHandler.Handle(att, def));
        }

        [Test]
        public void TestGeneralMajor()
        {
            Piece att = new General(Ownership.SecondPlayer);
            ICell def = new Major(Ownership.FirstPlayer);
            Assert.True(att == ConflictHandler.Handle(att, def));
        }

        [Test]
        public void TestMajorGeneral()
        {
            Piece att = new Major(Ownership.SecondPlayer);
            ICell def = new General(Ownership.FirstPlayer);
            Assert.True(def == ConflictHandler.Handle(att, def));
        }

        [Test]
        public void TestFailOnAttackerFlag_ThrowsPieceConflictHandlerException()
        {
            Piece att = new Flag(Ownership.SecondPlayer);
            ICell def = new EmptyCell();

            TestDelegate code = () => ConflictHandler.Handle(att, def);
            Assert.Throws(typeof(PieceConflictHandlerException), code);
        }

        [Test]
        public void TestFailOnAttackerBomb_ThrowsPieceConflictHandlerException()
        {
            Piece att = new Bomb(Ownership.FirstPlayer);
            ICell def = new Scout(Ownership.SecondPlayer);

            TestDelegate code = () => ConflictHandler.Handle(att, def);
            Assert.Throws(typeof(PieceConflictHandlerException), code);
        }
        
        [Test]
        public void TestFailOnAttackWaterMiner_ThrowsPieceConflictHandlerException()
        {
            Piece att = new Miner(Ownership.SecondPlayer);
            ICell def = new WaterCell();

            TestDelegate code = () => ConflictHandler.Handle(att, def);
            Assert.Throws(typeof(PieceConflictHandlerException) , code);
        }
        
        [Test]
        public void TestFailOnAttackWaterSergeant_ThrowsPieceConflictHandlerException()
        {
            Piece att = new Sergeant(Ownership.FirstPlayer);
            ICell def = new WaterCell();

            TestDelegate code = () => ConflictHandler.Handle(att, def);
            Assert.Throws(typeof(PieceConflictHandlerException) , code);
        }
        
        [Test]
        public void TestFailOnSameSideAttackSpySergeant__ThrowsPieceConflictHandlerException()
        {
            Piece att = new Spy(Ownership.FirstPlayer);
            ICell def = new Sergeant(Ownership.FirstPlayer);

            TestDelegate code = () => ConflictHandler.Handle(att, def);
            Assert.Throws(typeof(PieceConflictHandlerException) , code);
        }
        
        [Test]
        public void TestFailOnSameSideAttackColonalFlag__ThrowsPieceConflictHandlerException()
        {
            Piece att = new Colonel(Ownership.SecondPlayer);
            ICell def = new Flag(Ownership.SecondPlayer);

            TestDelegate code = () => ConflictHandler.Handle(att, def);
            Assert.Throws(typeof(PieceConflictHandlerException) , code);
        }
        
        
    }
}