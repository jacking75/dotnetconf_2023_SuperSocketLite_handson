using System;
using System.Collections.Generic;

using MemoryPack;


namespace MOServer;

public class PKHCommon : PKHandler
{
    public void RegistPacketHandler(Dictionary<int, Action<PktBinaryRequestInfo>> packetHandlerMap)
    {            
        packetHandlerMap.Add((int)PACKETID.NTF_IN_DISCONNECT_CLIENT, NotifyInDisConnectClient);

        packetHandlerMap.Add((int)PACKETID.REQ_LOGIN, RequestLogin);
                                            
    }
      
    public void NotifyInDisConnectClient(PktBinaryRequestInfo requestData)
    {
        var sessionID = requestData.SessionID;
        var user = UserMgr.GetUser(sessionID);

        if (user != null)
        {
            var roomNum = user.RoomNumber;

            if (roomNum != PacketDef.INVALID_ROOM_NUMBER)
            {
                var packet = new PKTInternalNtfRoomLeave()
                {
                    RoomNumber = roomNum,
                    UserID = user.ID(),
                };

                var pktData = MemoryPackSerializer.Serialize(packet);
                PacketHeadReadWrite.Write(PACKETID.NTF_IN_ROOM_LEAVE, pktData);

                var innerPkt = new PktBinaryRequestInfo(pktData);
                innerPkt.SessionID = sessionID;
                DistributePacket(innerPkt);
            }

            UserMgr.RemoveUser(sessionID);
        }

        MainServer.MainLogger.Debug("접속 끊어짐 처리 완료");
    }


    public void RequestLogin(PktBinaryRequestInfo packetData)
    {
        var sessionID = packetData.SessionID;
        MainServer.MainLogger.Debug("로그인 요청 받음");

        try
        {
            if(UserMgr.GetUser(sessionID) != null)
            {
                ResponseLoginToClient(ERROR_CODE.LOGIN_ALREADY_WORKING, packetData.SessionID);
                return;
            }
                            
            var reqData = MemoryPackSerializer.Deserialize<PKTReqLogin>(packetData.Data);
            var errorCode = UserMgr.AddUser(reqData.UserID, sessionID);
            if (errorCode != ERROR_CODE.NONE)
            {
                ResponseLoginToClient(errorCode, packetData.SessionID);

                if (errorCode == ERROR_CODE.LOGIN_FULL_USER_COUNT)
                {
                    NotifyMustCloseToClient(ERROR_CODE.LOGIN_FULL_USER_COUNT, packetData.SessionID);
                }
                
                return;
            }

            ResponseLoginToClient(errorCode, packetData.SessionID);

            MainServer.MainLogger.Debug("로그인 요청 답변 보냄");

        }
        catch(Exception ex)
        {
            // 패킷 해제에 의해서 로그가 남지 않도록 로그 수준을 Debug로 한다.
            MainServer.MainLogger.Error(ex.ToString());
        }
    }
            
    public void ResponseLoginToClient(ERROR_CODE errorCode, string sessionID)
    {
        var resLogin = new PKTResLogin()
        {
            Result = (short)errorCode
        };

        var pktData = MemoryPackSerializer.Serialize(resLogin);
        PacketHeadReadWrite.Write(PACKETID.RES_LOGIN, pktData);

        NetSendFunc(sessionID, pktData);
    }

    public void NotifyMustCloseToClient(ERROR_CODE errorCode, string sessionID)
    {
        var resLogin = new PKNtfMustClose()
        {
            Result = (short)errorCode
        };

        var pktData = MemoryPackSerializer.Serialize(resLogin);
        PacketHeadReadWrite.Write(PACKETID.NTF_MUST_CLOSE, pktData);

        NetSendFunc(sessionID, pktData);
    }


    
                  
}
