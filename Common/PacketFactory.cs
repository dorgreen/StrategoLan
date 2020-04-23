using System;
using System.Collections.Generic;
using Lidgren.Network;

namespace Common
{
   // TODO: IMPLEMENT SINGLETON
   // NOT THREAD SAFE, ASSUMING SEQUENTIAL RUN WITHOUT INTERRUPTS (no more than one instance of each and no wait)
   public class PacketFactoryFlyWheel
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
                    case PacketHeader.AttamptMovePacket:
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