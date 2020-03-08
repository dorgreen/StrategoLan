using System;
using System.Resources;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using Common;

namespace GameServer
{
    public class ServerBoard : Board
    {
        public void Reset()
        {
            this.State = new ICell[DefaultBoardSize,DefaultBoardSize];
            InitBoardSystemCells();
            return;
        }

        // TODO: Change signature?
        public Boolean ApplyValidMove(Position start, Position end)
        {
            // Hold both pieces
            // try:
            //     ask conflict handler for result
            // try:
            //     put the result at end position
            //    put an empty at start position
            // Notify whoever needs to know
            throw new NotImplementedException("ApplyValidMove");
        }

        public Object BoardToPacket(Ownership player)
        {
            // icell.sample() the whole board according to player
            // flip positions for player2 if needed
            // wrap in packet
            throw new NotImplementedException("BoardToPacket");
        }

        public Boolean InitBoardFromUser(ICell[][] Input, Ownership player)
        {
            // make sure size is correct
            // flip for player2 if needed
            throw new NotImplementedException("InitBoardFromUser");
        }

        private Boolean InitBoardSystemCells()
        {
            // Fills the middle two rows with the defaults ICells
            // Assumes classic board???
            throw new NotImplementedException("InitBoardSystem");
        }
        
        
    }
}