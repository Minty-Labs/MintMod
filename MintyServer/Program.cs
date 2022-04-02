using System.Management;
using MintyServer.Logger;
using MintyServer.SocketServer;

namespace MintyServer;

class Program
{
    public static void Main(String[] args)
    {
        MintyLogger.Init();
        Server.Start();
    }
}