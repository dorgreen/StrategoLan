using System;
using System.Collections.Generic;
using Lidgren.Network;

namespace Common
{
    public interface PacketFactory
    {
        Packet GetPacketInstance(PacketHeader header);
        Packet ReadNetMessage(NetIncomingMessage msg);
    }


    // To be used by async functions. these instances are guaranteed not to be messed with. 
    public class StaticPacketFactory : PacketFactory
    {
        public Packet GetPacketInstance(PacketHeader header)
        {
            Packet packet;
            switch (header)
            {
                case PacketHeader.BoardPacket:
                    packet = new BoardPacket();
                    break;
                case PacketHeader.PlayerReady:
                    packet = new PlayerReadyPacket();
                    break;
                case PacketHeader.AttemptMovePacket:
                    packet = new AttemptMovePacket();
                    break;
                case PacketHeader.ServerToClientGameStatusUpdatePacket:
                    packet = new ServerToClientGameStatusUpdatePacket();
                    break;
                default:
                    throw new NotImplementedException(String.Format("Unhandled PacketHeader @ StaticPacketFactory {0}", header.ToString()));
            }

            return packet;
        }

        // Not to be used. use something that recycles objects :)
        public Packet ReadNetMessage(NetIncomingMessage msg)
        {
            throw new NotImplementedException();
        }
    }
    
    // TODO: IMPLEMENT SINGLETON
   // NOT THREAD SAFE, ASSUMING SEQUENTIAL RUN WITHOUT INTERRUPTS (no more than one instance of each and no wait)
   // ASSUMING NEVER NEEDING TWO INSTANCES OF THE SAME PACKET TYPE IN THE SAME TIME (could be solved by having one for incomming and one for outgoing packets)
   public class PacketFactoryFlyWheel : PacketFactory
    {
        private Dictionary<PacketHeader, Packet> flywheel;


        public PacketFactoryFlyWheel()
        {
            this.flywheel = new Dictionary<PacketHeader, Packet>();
        }

        public Packet GetPacketInstance(PacketHeader header)
        {
            Packet packet = null;
            if (this.flywheel.ContainsKey(header))
            {
                packet = this.flywheel[header];
                packet.Recycle();
                return packet;
            }
            else
            {
                switch (header)
                {
                    case PacketHeader.BoardPacket:
                        packet = new BoardPacket();
                        break;
                    case PacketHeader.PlayerReady:
                        packet = new PlayerReadyPacket();
                        break;
                    case PacketHeader.AttemptMovePacket:
                        packet = new AttemptMovePacket();
                        break;
                    case PacketHeader.ServerToClientGameStatusUpdatePacket:
                        packet = new ServerToClientGameStatusUpdatePacket();
                        break;
                    default:
                        throw new NotImplementedException(String.Format("Unhandled PacketHeader @ PacketFactoryFlyWheel {0}", header.ToString()));
                }

                this.flywheel[header] = packet;
                return packet;
            }
        }

        public Packet ReadNetMessage(NetIncomingMessage msg)
        {
            if (msg.MessageType != NetIncomingMessageType.Data) return null;
            
            PacketHeader header = (PacketHeader) msg.PeekByte();
            var packet = GetPacketInstance(header);
            packet.PopulateFromNetMessage(msg);
            return packet;
        }
    }
}