using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using System.Threading.Tasks.Dataflow;


namespace MOServer;

class PacketProcessor
{
    bool IsThreadRunning = false;
    System.Threading.Thread ProcessThread = null;

    public Func<string, byte[], bool> NetSendFunc;
    public Action<PktBinaryRequestInfo> DistributePacket;

    BufferBlock<PktBinaryRequestInfo> MsgBuffer = new();

    UserManager UserMgr = new();

    List<Room> RoomList = new List<Room>();

    Dictionary<int, Action<PktBinaryRequestInfo>> PacketHandlerMap = new();
    PKHCommon CommonPacketHandler = new PKHCommon();
    PKHRoom RoomPacketHandler = new PKHRoom();
            

    public void CreateAndStart(List<Room> roomList)
    {
        UserMgr.Init(MainServer.Conf_MaxUserCount);

        RoomList = roomList;
            
        RegistPacketHandler();

        IsThreadRunning = true;
        ProcessThread = new System.Threading.Thread(this.Process);
        ProcessThread.Start();
    }
    
    public void Destory()
    {
        IsThreadRunning = false;
        MsgBuffer.Complete();
    }
          
    public void InsertPacket(PktBinaryRequestInfo data)
    {
        MsgBuffer.Post(data);
    }

    
    void RegistPacketHandler()
    {
        PKHandler.NetSendFunc = NetSendFunc;
        PKHandler.DistributePacket = DistributePacket;  

        CommonPacketHandler.Init(UserMgr);
        CommonPacketHandler.RegistPacketHandler(PacketHandlerMap);                
        
        RoomPacketHandler.Init(UserMgr);
        RoomPacketHandler.SetRooomList(RoomList);
        RoomPacketHandler.RegistPacketHandler(PacketHandlerMap);
    }

    void Process()
    {
        var header = new PacketHeadReadWrite();
        
        while (IsThreadRunning)
        {
            try
            {
                var packet = MsgBuffer.Receive();
                header.Read(packet.Data);

                if (PacketHandlerMap.ContainsKey(header.Id))
                {
                    PacketHandlerMap[header.Id](packet);
                }               
            }
            catch (Exception ex)
            {
                if (IsThreadRunning)
                {
                    MainServer.MainLogger.Error(ex.ToString());
                }
            }
        }
    }


}
