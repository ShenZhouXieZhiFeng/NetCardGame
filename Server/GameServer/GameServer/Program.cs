using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AhpilyServer;

namespace GameServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerPeer server = new ServerPeer();
            //指定关联应用
            server.SetApplication(new NetMsgCenter());
            server.Start(6666, 10);

            Console.ReadLine();
        }
    }
}
