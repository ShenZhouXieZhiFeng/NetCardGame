using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketDemo
{
    class Program
    {

        static Socket serverSocket = null;

        static void Main(string[] args)
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 9999);
            serverSocket.Bind(endPoint);
            serverSocket.Listen(10);
            Thread thread = new Thread(listenClientConnectThread);
            thread.Start();
            Console.ReadLine();
        }

        /// <summary>
        /// 监听客户端连接
        /// </summary>
        static void listenClientConnectThread()
        {
            Socket client = serverSocket.Accept();
            Console.WriteLine("客户端连接成功");
            client.Send(Encoding.Default.GetBytes("客户端你好！"));
            Thread receive = new Thread(receiveClientThread);
            receive.Start(client);
        }

        /// <summary>
        /// 接收客户端消息
        /// </summary>
        static void receiveClientThread(object _client)
        {
            Socket client = (Socket)_client;
            byte[] recs = new byte[1024];
            int length = client.Receive(recs);
            Console.WriteLine(Encoding.Default.GetString(recs, 0, length));
        }
    }
}
