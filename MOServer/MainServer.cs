using System;

using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Logging;
using SuperSocket.SocketBase.Protocol;




namespace MOServer;

public class MainServer : AppServer<ClientSession, PktBinaryRequestInfo>
{
    public const int Conf_MaxUserCount = 256;
    public const int Conf_RoomMaxCount = 56;
    public const int Conf_RoomMaxUserCount = 2;

    public static SuperSocket.SocketBase.Logging.ILog MainLogger;

    SuperSocket.SocketBase.Config.IServerConfig m_Config;

    PacketProcessor MainPacketProcessor = new PacketProcessor();
    
    
    
    public MainServer()
        : base(new DefaultReceiveFilterFactory<ReceiveFilter, PktBinaryRequestInfo>())
    {
        NewSessionConnected += new SessionHandler<ClientSession>(OnConnected);
        SessionClosed += new SessionHandler<ClientSession, CloseReason>(OnClosed);
        NewRequestReceived += new RequestHandler<ClientSession, PktBinaryRequestInfo>(OnPacketReceived);
    }

    public void InitConfig()
    {
        m_Config = new SuperSocket.SocketBase.Config.ServerConfig
        {
            Name = "MO Server",
            Ip = "Any",
            Port = 32452,
            Mode = SocketMode.Tcp,
            MaxConnectionNumber = Conf_MaxUserCount,
            MaxRequestLength = 1024,
            ReceiveBufferSize = 8012,
            SendBufferSize = 8012
        };
    }
    
    public void CreateStartServer()
    {
        try
        {
            bool bResult = Setup(new SuperSocket.SocketBase.Config.RootConfig(), m_Config, logFactory: new ConsoleLogFactory());

            if (bResult == false)
            {
                Console.WriteLine("[ERROR] 서버 네트워크 설정 실패 ㅠㅠ");
                return;
            } 
            else 
            {
                MainLogger = base.Logger;
                MainLogger.Info("서버 초기화 성공");
            }


            CreateComponent();

            Start();

            MainLogger.Info("서버 생성 성공");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] 서버 생성 실패: {ex.ToString()}");
        }          
    }

    
    public void StopServer()
    {            
        Stop();

        MainPacketProcessor.Destory();
    }

    public ERROR_CODE CreateComponent()
    {
        Room.NetSendFunc = this.SendData;
        

        MainPacketProcessor = new PacketProcessor();
        MainPacketProcessor.NetSendFunc = this.SendData;
        MainPacketProcessor.DistributePacket = this.Distribute;
        MainPacketProcessor.CreateAndStart();

        MainLogger.Info("CreateComponent - Success");
        return ERROR_CODE.NONE;
    }

    public bool SendData(string sessionID, byte[] sendData)
    {
        var session = GetSessionByID(sessionID);

        try
        {
            if (session == null)
            {
                return false;
            }

            session.Send(sendData, 0, sendData.Length);
        }
        catch(Exception ex)
        {
            // TimeoutException 예외가 발생할 수 있다
            MainServer.MainLogger.Error($"{ex.ToString()},  {ex.StackTrace}");

            session.SendEndWhenSendingTimeOut(); 
            session.Close();
        }
        return true;
    }

    public void Distribute(PktBinaryRequestInfo requestPacket)
    {
        MainPacketProcessor.InsertPacket(requestPacket);
    }
                    
    void OnConnected(ClientSession session)
    {
        MainLogger.Info(string.Format("세션 번호 {0} 접속", session.SessionID));
    }

    void OnClosed(ClientSession session, CloseReason reason)
    {
        MainLogger.Info(string.Format("세션 번호 {0} 접속해제: {1}", session.SessionID, reason.ToString()));

        var packet = InnerPacketManager.MakeNTFDisConnectPacket(session.SessionID);
        Distribute(packet);
    }

    void OnPacketReceived(ClientSession session, PktBinaryRequestInfo reqInfo)
    {
        MainLogger.Debug(string.Format("세션 번호 {0} 받은 데이터 크기: {1}, ThreadId: {2}", session.SessionID, reqInfo.Body.Length, System.Threading.Thread.CurrentThread.ManagedThreadId));

        reqInfo.SessionID = session.SessionID;
        Distribute(reqInfo);
    }
}

public class ClientSession : AppSession<ClientSession, PktBinaryRequestInfo>
{
}
    
