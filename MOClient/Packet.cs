
using MemoryPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csharp_test_client;


public struct PacketHeadReadWrite
{
    const int StartPos = 1;
    public const int HeadSize = 6;

    public UInt16 TotalSize;
    public UInt16 Id;
    public byte Type;

    public static UInt16 GetTotalSize(byte[] data, int startPos)
    {
        return FastBinaryRead.UInt16(data, startPos + StartPos);
    }

    public static void WritePacketId(byte[] data, UInt16 packetId)
    {
        FastBinaryWrite.UInt16(data, StartPos + 2, packetId);
    }

    public void Read(byte[] headerData)
    {
        var pos = StartPos;

        TotalSize = FastBinaryRead.UInt16(headerData, pos);
        pos += 2;

        Id = FastBinaryRead.UInt16(headerData, pos);
        pos += 2;

        Type = headerData[pos];
        pos += 1;
    }

    public void Write(byte[] pktData)
    {
        var pos = StartPos;

        FastBinaryWrite.UInt16(pktData, pos, TotalSize);
        pos += 2;

        FastBinaryWrite.UInt16(pktData, pos, Id);
        pos += 2;

        pktData[pos] = Type;
        pos += 1;
    }

    public static void Write(PACKET_ID packetId, byte[] pktData)
    {
        var pos = StartPos;

        FastBinaryWrite.UInt16(pktData, pos, (UInt16)pktData.Length);
        pos += 2;

        FastBinaryWrite.UInt16(pktData, pos, (UInt16)packetId);
        pos += 2;

        pktData[pos] = 0;
        pos += 1;
    }
}


//헤더 정보를 byte 배열을 사용하면 안된다. 크기가 커진다
/*
[MemoryPackable]
public partial class PacketHead
{
    public Byte[] Head = new Byte[PacketHeadReadWrite.HeadSize];
}*/
[MemoryPackable]
public partial class PacketHead
{
    public UInt16 TotalSize { get; set; } = 0;
    public UInt16 Id { get; set; } = 0;
    public byte Type { get; set; } = 0;
}


// 로그인 요청
[MemoryPackable]
public partial class PKTReqLogin : PacketHead
{
    public string UserID;
    public string AuthToken;
}

[MemoryPackable]
public partial class PKTResLogin : PacketHead
{
    public short Result;
}



[MemoryPackable]
public partial class PKNtfMustClose : PacketHead
{
    public short Result;
}



[MemoryPackable]
public partial class PKTReqRoomEnter : PacketHead
{
    public int RoomNumber;
}

[MemoryPackable]
public partial class PKTResRoomEnter : PacketHead
{
    public short Result;
}

[MemoryPackable]
public partial class PKTNtfRoomUserList : PacketHead
{
    public List<string> UserIDList = new List<string>();
}

[MemoryPackable]
public partial class PKTNtfRoomNewUser : PacketHead
{
    public string UserID;
}


[MemoryPackable]
public partial class PKTReqRoomLeave : PacketHead
{
}

[MemoryPackable]
public partial class PKTResRoomLeave : PacketHead
{
    public short Result;
}

[MemoryPackable]
public partial class PKTNtfRoomLeaveUser : PacketHead
{
    public string UserID;
}


[MemoryPackable]
public partial class PKTReqRoomChat : PacketHead
{
    public string ChatMessage;
}


[MemoryPackable]
public partial class PKTNtfRoomChat : PacketHead
{
    public string UserID;
    public string ChatMessage;
}
