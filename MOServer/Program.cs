using System;

namespace MOServer;

class Program
{
    //dotnet ChatServer.dll --uniqueID 1 --roomMaxCount 16 --roomMaxUserCount 4 --roomStartNumber 1 --maxUserCount 100
    static void Main(string[] args)
    {
        var serverApp = new MainServer();
        serverApp.InitConfig();
        serverApp.CreateStartServer();


        MainServer.MainLogger.Info("Press q to shut down the server");

        while (true)
        {
            System.Threading.Thread.Sleep(50);

            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.KeyChar == 'q')
                {
                    Console.WriteLine("Server Terminate ~~~");
                    serverApp.StopServer();
                    break;
                }
            }
                            
        }
    }
                

} // end Class
