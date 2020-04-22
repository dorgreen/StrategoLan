using System.Data;

namespace Common
{
    public enum Directions {
        Up, Right, Down, Left
    }
    public struct Position
    {
        private int _x, _y;

        public Position(int xpos, int ypos)
        {
            _x = xpos;
            _y = ypos;
        }

        public void copy_from_other(Position other)
        {
            this._x = other.X;
            this._y = other._y;
        }

        public Position(Position p, Directions dir)
        {
            int x = p.X;
            int y = p.Y;
            switch (dir)
            {
                case Directions.Up:
                    y++;
                    break;
                case Directions.Right:
                    x++;
                    break;
                case Directions.Down:
                    y--;
                    break;
                case Directions.Left:
                    x--;
                    break;
            }
            _x = x;
            _y = y;
        }

        public int X
        {
            get { return _x; }

            set { _x = value; }
        }

        public int Y
        {
            get { return _y; }
            set { _y = value; }
        }
        
        
    }
}