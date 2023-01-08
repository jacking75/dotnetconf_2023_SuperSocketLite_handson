using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SuperSocket.Common;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketEngine.Protocol;

namespace EchoServer
{
    public class PktBinaryRequestInfo : BinaryRequestInfo
    {
		// 패킷 헤더용 변수
        public UInt16 TotalSize { get; private set; }
        public UInt16 PacketID { get; private set; }
        public SByte Value1 { get; private set; }

        public const int HEADERE_SIZE = 5;

        
        public PktBinaryRequestInfo(UInt16 totalSize, UInt16 packetID, SByte value1, byte[] body)
            : base(null, body)
        {
            this.TotalSize = totalSize;
            this.PacketID = packetID;
            this.Value1 = value1;
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
                Array.Reverse(header, offset, 2);

            var packetTotalSize = BitConverter.ToUInt16(header, offset);
            return packetTotalSize - PktBinaryRequestInfo.HEADERE_SIZE;
        }

        protected override PktBinaryRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(header.Array, 0, PktBinaryRequestInfo.HEADERE_SIZE);

            return new PktBinaryRequestInfo(BitConverter.ToUInt16(header.Array, 0),
                                           BitConverter.ToUInt16(header.Array, 0 + 2),
                                           (SByte)header.Array[4], 
                                           bodyBuffer.CloneRange(offset, length));
        }
    }
}
