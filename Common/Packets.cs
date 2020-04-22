using System;
using System.Runtime.InteropServices;
using Lidgren.Network;
using Newtonsoft.Json;

namespace Common
{

    public enum PacketHeader
    {
        BoardPacket,
        AttamptMovePacket,
        ServerToClientGameStatusUpdatePacket,
        PlayerReady
    }

    public enum ClientGameStates
    {
        Error,
        IninitalConnection,
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
    
    // To be used by both client and server for their respective messages
    // TODO: IMPLEMENT FLYWHEEL

    // Used by server to update client of new board state after turns
    // Used by client to send initial position of pieces as selected by user
    public class BoardPacket : Packet
    {
        public BoardPacket(ICell[,] boardState)
        {
            BoardState = boardState;
        }

        public ICell[,] BoardState;

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
            var settings = new JsonSerializerSettings();
            settings.ObjectCreationHandling = ObjectCreationHandling.Replace;
            // TODO: testing only, should replace with singleton for JSONConvertICell
            settings.Converters.Add(new JSONConvertICell());
            string data = Newtonsoft.Json.JsonConvert.SerializeObject(BoardState, settings);
            msg.Write(data.Length);
            msg.Write(data);
            
            return;
        }

        public Packet PopulateFromNetMessage(NetIncomingMessage msg)
        {
            if (msg.ReadByte() != (Byte)PacketHeader.BoardPacket)
            {
                return null;
            }

            int length = msg.ReadInt32();
            string json_data = msg.ReadString();
            
            // TODO: testing only, should replace with singleton for JSONConvertICell
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new JSONConvertICell());
            settings.ObjectCreationHandling = ObjectCreationHandling.Replace;
            
            BoardState = Newtonsoft.Json.JsonConvert.DeserializeObject<ICell[,]>(json_data, settings);
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
                throw new Exception("Packet is not populated: AttamptMovePacket");
            }
            
            msg.Write((byte)PacketHeader.AttamptMovePacket);
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
            if (msg.ReadByte() != (Byte)PacketHeader.AttamptMovePacket)
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
