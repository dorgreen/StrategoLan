using System;
using System.Linq;
using System.Runtime.InteropServices;
using Lidgren.Network;
using Json.Net;

namespace Common
{

    public enum PacketHeader
    {
        BoardPacket,
        AttemptMovePacket,
        ServerToClientGameStatusUpdatePacket,
        PlayerReady
    }

   
    // TODO: Maybe add a "display data" packet?
    public enum ClientGameStates
    {
        Error,
        InitialConnection,
        WaitForBoard,
        WaitForStart,
        YourMove,
        WaitOtherPlayerMove,
        GameOver
    }
    
    public interface Packet
        {
             void PackIntoNetMessage(NetOutgoingMessage msg);
             Packet PopulateFromNetMessage(NetIncomingMessage msg);
             void Recycle();
        }
    

    // Used by server to update client of new board state after turns
    // Used by client to send initial position of pieces as selected by user
    public class BoardPacket : Packet
    {
        public BoardPacket(ICell[] boardState)
        {
            SetBoardState(boardState);
        }

        public string[] BoardState;

        public void SetBoardState(ICell[] ICellBoardToSet){
            this.BoardState = ICellBoardToSet.Select(x => x.ToString()).ToArray();
        } 

        public BoardPacket()
        {
            BoardState = null;
        }

        public void Recycle()
        {
            BoardState = null;
        }

        public void PackIntoNetMessage(NetOutgoingMessage msg)
        {
            msg.Write((byte)PacketHeader.BoardPacket);
            var encoded = Json.Net.JsonNet.Serialize(BoardState);
            msg.Write(encoded);
            return;
        }

        public Packet PopulateFromNetMessage(NetIncomingMessage msg)
        {
            if (msg.ReadByte() != (Byte)PacketHeader.BoardPacket)
            {
                return null;
            }
            string json_data = msg.ReadString();
            this.BoardState = Json.Net.JsonNet.Deserialize<string[]>(json_data);            
            return this;
        }
    }

    public class AttemptMovePacket : Packet
    {

        public Position origin;
        public Position dest;
        public AttemptMovePacket(Position origin, Position dest)
        {
            this.origin = origin;
            this.dest = dest;
        }

        public AttemptMovePacket()
        {
            this.Recycle();
        }

        public void PackIntoNetMessage(NetOutgoingMessage msg)
        {
            if (origin.X == -1 || origin.Y == -1 || dest.X == -1 || dest.Y == -1)
            {
                throw new Exception("Packet is not populated: AttemptMovePacket");
            }
            
            msg.Write((byte)PacketHeader.AttemptMovePacket);
            msg.Write("From ");
            msg.Write(origin.X);
            msg.Write(" ");
            msg.Write(origin.Y);
            msg.Write(" To ");
            msg.Write(dest.X);
            msg.Write(" ");
            msg.Write(dest.Y);
        }

        public Packet PopulateFromNetMessage(NetIncomingMessage msg)
        {
            if (msg.ReadByte() != (Byte)PacketHeader.AttemptMovePacket)
            {
                return null;
            }

            if (!msg.ReadString().Equals("From ")) return null;
            this.origin.X = msg.ReadInt32();
            if (!msg.ReadString().Equals(" ")) return null;
            this.origin.Y = msg.ReadInt32();
            if (!msg.ReadString().Equals(" To ")) return null;
            this.dest.X = msg.ReadInt32();
            if (!msg.ReadString().Equals(" ")) return null;
            this.dest.Y = msg.ReadInt32();
            return this;
        }

        public void Recycle()
        {
            this.origin = new Position(-1, -1);
            this.dest = new Position(-1, -1);
        }
    }

    public class ServerToClientGameStatusUpdatePacket : Packet
    {

        public ClientGameStates state;
        public string info;
        
        public ServerToClientGameStatusUpdatePacket()
        {
            this.Recycle();
        }

        public void PackIntoNetMessage(NetOutgoingMessage msg)
        {
            msg.Write((byte)PacketHeader.ServerToClientGameStatusUpdatePacket);
            msg.Write((byte) this.state);
            msg.Write(info);
        }

        public Packet PopulateFromNetMessage(NetIncomingMessage msg)
        {
            if (msg.ReadByte() != (Byte)PacketHeader.ServerToClientGameStatusUpdatePacket)
            {
                return null;
            }

            this.state = (ClientGameStates) msg.ReadByte();
            this.info = msg.ReadString();
            return this;
        }

        public void Recycle()
        {
            state = ClientGameStates.Error;
            info = "";
        }
    }

    public class PlayerReadyPacket : Packet
    {
        public PlayerReadyPacket()
        {
        }

        public void PackIntoNetMessage(NetOutgoingMessage msg)
        {
            msg.Write((byte) PacketHeader.PlayerReady);
        }

        public Packet PopulateFromNetMessage(NetIncomingMessage msg)
        {
            if (msg.ReadByte() == (byte) PacketHeader.PlayerReady)
            {
                return this;    
            }
            else
            {
                return null;
            }
            
        }

        public void Recycle()
        {
        }
    }
}
