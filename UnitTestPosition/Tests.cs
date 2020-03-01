using System;
using Common;
using NUnit.Framework;

namespace UnitTestPosition
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Create()
        {
            int x = 1;
            int y = 2;
            Position p = new Position(x,y);
            Assert.AreEqual(x, p.X);
            Assert.AreEqual(y, p.Y);
        }
        
        [Test]
        public void CreateUninitAndSet()
        {
            int x = 8;
            int y = 11;
            Position p = new Position();
            p.X = x;
            p.Y = y;
            Assert.AreEqual(x, p.X);
            Assert.AreEqual(y, p.Y);
        }
        
        [Test]
        public void CreateAndSet()
        {
            int x = 1;
            int new_x = 44;
            int y = 2;
            Position p = new Position(x,y);
            p.X = new_x;
            Assert.AreEqual(new_x, p.X);
            Assert.AreEqual(y, p.Y);
        }
        
        [Test]
        public void PositionFromDirectionUp()
        {
            int x = 1;
            int y = 2;
            Position orig = new Position(x,y);
            Position p = new Position(orig, Directions.Up);
            Assert.AreEqual(x, orig.X);
            Assert.AreEqual(y, orig.Y);
            y++;
            Assert.AreEqual(x, p.X);
            Assert.AreEqual(y, p.Y);
        }
        
        [Test]
        public void PositionFromDirectionDown()
        {
            int x = 1;
            int y = 2;
            Position orig = new Position(x,y);
            Position p = new Position(orig, Directions.Down);
            Assert.AreEqual(x, orig.X);
            Assert.AreEqual(y, orig.Y);
            y--;
            Assert.AreEqual(x, p.X);
            Assert.AreEqual(y, p.Y);
        }
        
        [Test]
        public void PositionFromDirectionLeft()
        {
            int x = 1;
            int y = 2;
            Position orig = new Position(x,y);
            Position p = new Position(orig, Directions.Left);
            Assert.AreEqual(x, orig.X);
            Assert.AreEqual(y, orig.Y);
            x--;
            Assert.AreEqual(x, p.X);
            Assert.AreEqual(y, p.Y);
        }
        
        [Test]
        public void PositionFromDirectionRight()
        {
            int x = 1;
            int y = 2;
            Position orig = new Position(x,y);
            Position p = new Position(orig, Directions.Right);
            Assert.AreEqual(x, orig.X);
            Assert.AreEqual(y, orig.Y);
            x++;
            Assert.AreEqual(x, p.X);
            Assert.AreEqual(y, p.Y);
        }
        
        
    }
}