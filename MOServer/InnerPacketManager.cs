using MemoryPack;
using System;

namespace MOServer;

public class InnerPacketManager
{   
    public static PktBinaryRequestInfo MakeNTFDisConnectPacket(string sessionID)
    {
        var packet = new PktBinaryRequestInfo(null);
        packet.Data = new byte[PacketHeadReadWrite.HeadSize];

        PacketHeadReadWrite.WritePacketId(packet.Data, (UInt16)PACKETID.NTF_IN_DISCONNECT_CLIENT);
        packet.SessionID = sessionID;
        return packet;
    }

}



[MemoryPackable]
public partial class PKTInternalNtfRoomLeave : PacketHead
{
    public int RoomNumber;

    public string UserID;
}
