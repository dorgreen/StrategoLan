using System;
using System.Runtime.InteropServices;
using Lidgren.Network;

namespace Common
{

    public enum PacketHeader
    {
        BoardPacket
    }
    
    public interface Packet
        {
             void PackIntoNetMessage(NetOutgoingMessage msg);
             Packet PopulateFromNetMessage(NetIncomingMessage msg);
        }
    
    // To be used by both client and server for their respective messages
    // TODO: IMPLEMENT FLYWHEEL
    public class PacketFactory{
        public static Packet ReadNetMessage(NetIncomingMessage msg)
        {
            // Switch \ use dict over message types 
            // use the appropriate constructor PopulateFromNetMessage(NetIncomingMessage msg)
            throw new NotImplementedException("ReadNetMessage");
        }
    }

    // Used by server to update client of new board state after turns
    // Used by client to send initial position of pieces as selected by user
    public class BoardPacket : Packet
    {
        public BoardPacket(ICell[,] boardState)
        {
            BoardState = boardState;
        }

        private ICell[,] BoardState;

        public BoardPacket()
        {
            BoardState = null;
        }

        public void PackIntoNetMessage(NetOutgoingMessage msg)
        {
            msg.Write((byte)PacketHeader.BoardPacket);
            string data = Newtonsoft.Json.JsonConvert.SerializeObject(BoardState);
            msg.Write(data.Length);
            msg.Write(data);
            
            throw new NotImplementedException();
        }

        public Packet PopulateFromNetMessage(NetIncomingMessage msg)
        {
            if (msg.ReadByte() != (Byte)PacketHeader.BoardPacket)
            {
                return null;
            }

            int length = msg.ReadInt32();
            string json_data = msg.ReadString();
            BoardState = Newtonsoft.Json.JsonConvert.DeserializeObject<ICell[,]>(json_data);
            return this;
            
        }
    }
}