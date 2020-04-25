using System;
using System.Linq;
using Common;

namespace CLIClient
{
    public class ClientBoard : Board 
    {
        public ClientBoard() 
        {
        }

        public bool SetBoardFromStringArray(string[] data, bool force_creation = false)
        {
            if (data.Length != this.State.Length || data.Any(x => ! ICellTools.IsStringICell(x)))
            {
                return false;
            }

            if (force_creation)
            {
                this.State = data.Select(id => ICellTools.ICellFromString(id)).ToArray();
                return true;
            }

            this.State = data.Zip(this.State,
                (new_id, exsisting_cell) => (exsisting_cell.ToString().CompareTo(new_id) == 0
                    ? exsisting_cell
                    : ICellTools.ICellFromString(new_id))).ToArray();
            return true;
        }

        // TODO: print board a little bit more nicely
        public void PrintBoard()
        {
            Console.WriteLine(State.ToString());
        }
    }
}