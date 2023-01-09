using System;
using System.Collections.Generic;
using System.Threading;

using SuperSocket.SocketBase.Logging;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketBase.Config;


namespace EchoServer
{
    class MainServer : AppServer<NetworkSession, PktBinaryRequestInfo>
    {
        public static SuperSocket.SocketBase.Logging.ILog MainLogger;
                        
        public MainServer()
            : base(new DefaultReceiveFilterFactory<ReceiveFilter, PktBinaryRequestInfo>())
        {
            NewSessionConnected += new SessionHandler<NetworkSession>(OnConnected);
            SessionClosed += new SessionHandler<NetworkSession, CloseReason>(OnClosed);
            NewRequestReceived += new RequestHandler<NetworkSession, PktBinaryRequestInfo>(RequestReceived);
        }
                
        public void CreateServer()
        {
            try
            {
                var config = new ServerConfig
                {
                    Port = 32452,
                    Ip = "Any",
                    MaxConnectionNumber = 100,
                    Mode = SocketMode.Tcp,
                    Name = "Echo Server"
                };

                bool bResult = Setup(new RootConfig(), config, logFactory: new ConsoleLogFactory());

                if (bResult == false)
                {
                    Console.WriteLine("[ERROR] 서버 네트워크 설정 실패 ㅠㅠ");
                    return;
                }

                MainLogger = base.Logger;
                
                MainLogger.Info($"[{DateTime.Now}] 서버 생성 성공");
            }
            catch(Exception ex)
            {
                MainLogger.Error($"서버 생성 실패: {ex.ToString()}");
            }
        }


        void OnConnected(NetworkSession session)
        {
            MainLogger.Debug($"[{DateTime.Now}] 세션 번호 {session.SessionID} 접속 start, ThreadId: {System.Threading.Thread.CurrentThread.ManagedThreadId}");
            //Thread.Sleep(3000);
            //MainLogger.Info($"세션 번호 {session.SessionID} 접속 end, ThreadId: {System.Threading.Thread.CurrentThread.ManagedThreadId}");
        }

        void OnClosed(NetworkSession session, CloseReason reason)
        {
            MainLogger.Info($"[{DateTime.Now}] 세션 번호 {session.SessionID},  접속해제: {reason.ToString()}");
        }

        void RequestReceived(NetworkSession session, PktBinaryRequestInfo reqInfo)
        {
            MainLogger.Debug($"[{DateTime.Now}] 세션 번호 {session.SessionID},  받은 데이터 크기: {reqInfo.Body.Length}, ThreadId: {System.Threading.Thread.CurrentThread.ManagedThreadId}");
              
            
            var totalSize = (Int16)(reqInfo.Body.Length + PktBinaryRequestInfo.HEADERE_SIZE);

            List<byte> dataSource = new List<byte>();
            dataSource.AddRange(BitConverter.GetBytes(totalSize));
            dataSource.AddRange(BitConverter.GetBytes((Int16)reqInfo.PacketID));
            dataSource.AddRange(new byte[1]);
            dataSource.AddRange(reqInfo.Body);

            session.Send(dataSource.ToArray(), 0, dataSource.Count);
        }
    }


    public class NetworkSession : AppSession<NetworkSession, PktBinaryRequestInfo>
    {
    }
}
