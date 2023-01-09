using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SuperSocket.Common;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketEngine.Protocol;

namespace MOServer;

public class PktBinaryRequestInfo : BinaryRequestInfo
{
    // 헤더에 해당하는 데이터들 총 5바이트
    //public UInt16 TotalSize;  public UInt16 PacketID;  public Byte Type;

    public string SessionID;
    public byte[] Data; // 헤더와 보디가 합쳐진 패킷 데이터

    public const int START_POS = 1;
    public const int HEADERE_SIZE = 5 + START_POS;
            
    public PktBinaryRequestInfo(byte[] packetData)
        : base(null, packetData)
    {
        Data = packetData;
    }

}

public class ReceiveFilter : FixedHeaderReceiveFilter<PktBinaryRequestInfo>
{
    public ReceiveFilter() : base(PktBinaryRequestInfo.HEADERE_SIZE)
    {
    }

    protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
    {
        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(header, offset, 2);
        }

        var totalSize = BitConverter.ToUInt16(header, offset + PktBinaryRequestInfo.START_POS);
        return totalSize - PktBinaryRequestInfo.HEADERE_SIZE;
    }

    protected override PktBinaryRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
    {
        if (!BitConverter.IsLittleEndian)
        {
            Array.Reverse(header.Array, 0, PktBinaryRequestInfo.HEADERE_SIZE);
        }

        PktBinaryRequestInfo reqInfo = null;
        if (length > 0)
        {
            var packetStartPos = offset - PktBinaryRequestInfo.HEADERE_SIZE;
            var packetSize = length + PktBinaryRequestInfo.HEADERE_SIZE;
            reqInfo = new PktBinaryRequestInfo(bodyBuffer.CloneRange(packetStartPos, packetSize));
        }
        else
        {
            reqInfo = new PktBinaryRequestInfo(header.CloneRange(header.Offset, header.Count));
        }
        
        return reqInfo;
    }
}
