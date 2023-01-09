using System;

namespace EchoServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello SuperSocketLite");
            
            var server = new MainServer();
            server.CreateServer();

            var IsResult = server.Start();

            if (IsResult)
            {
                MainServer.MainLogger.Info("서버 네트워크 시작");
            }
            else
            {
                Console.WriteLine("서버 네트워크 시작 실패");
                return;
            }
                        

            Console.WriteLine("key를 누르면 종료한다....");
            Console.ReadKey();
        }

                  

    }

    
}
