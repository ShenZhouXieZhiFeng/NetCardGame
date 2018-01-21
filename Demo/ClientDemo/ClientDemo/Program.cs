using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ClientDemo
{
    class Program
    {
        private static Socket clientSocket = null;
        static void Main(string[] args)
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint remoteIp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9999);
            clientSocket.Connect(remoteIp);
            Console.WriteLine("服务器连接成功");

            byte[] res = new byte[1024];
            int len = clientSocket.Receive(res);
            Console.WriteLine(Encoding.Default.GetString(res, 0, len));

            clientSocket.Send(Encoding.Default.GetBytes("服务端你好"));
            Console.ReadLine();
        }

    }
}
