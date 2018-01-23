using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AhpilyServer
{
    public class ServerPeer
    {
        //服务端socket
        private Socket serverSocket = null;
        //限制同时连接的客户端数量,信号量
        private Semaphore acceptSemaphore;
        //客户端连接池
        private ClientPeerPool clientPeerPool;
        //应用层
        private IApplicationBase application;

        /// <summary>
        /// 设置应用层
        /// </summary>
        /// <param name="app"></param>
        public void SetApplication(IApplicationBase app)
        {
            application = app;
        }

        /// <summary>
        /// 开启服务
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="maxCount">最大连接数量</param>
        public void Start(int port,int maxCount)
        {
            try
            {
                //初始化连接池
                clientPeerPool = new ClientPeerPool(maxCount);
                //限制最大连接的信号量
                acceptSemaphore = new Semaphore(maxCount, maxCount);
                //初始化服务端Scoket
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Any, port);
                serverSocket.Bind(ipPoint);
                serverSocket.Listen(10);
                Console.WriteLine("服务器启动成功...");
                startAccept(null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        #region 接收连接
        /// <summary>
        /// 开始等待客户端的连接
        /// </summary>
        private void startAccept(SocketAsyncEventArgs e)
        {
            if (e == null)
            {
                e = new SocketAsyncEventArgs();
                e.Completed += accpetCompleted;
            }
            bool result = serverSocket.AcceptAsync(e);
            //res判断异步事件是否执行完毕
            //false表示连接执行完成,可以直接处理
            //true表示正在执行，完成时会自动执行绑定的completed事件
            if (result == false)
            {
                processAccept(e);
            }
        }

        /// <summary>
        /// 接收连接请求异步事件完成时候会触发
        /// </summary>
        private void accpetCompleted(object sender,SocketAsyncEventArgs e)
        {
            processAccept(e);
        }

        /// <summary>
        /// 处理连接请求
        /// </summary>
        /// <param name="e"></param>
        private void processAccept(SocketAsyncEventArgs e)
        {
            //限制线程的访问
            acceptSemaphore.WaitOne();
            //获得连接的客户端
            Socket clientSocket = e.AcceptSocket;
            //从连接池中取对象并设定连接的客户端
            ClientPeer client = clientPeerPool.Dequeue();
            client.ClientSocket = clientSocket; //指定客户端socket
            client.ReceiveArgs.Completed += receive_completed;//指定一次接收时完成的操作
            client.OnReceiveCompleted = receiveCompleted;//指定消息解析完毕的回调
            client.OnSendClientDisconnect = Disconnect;//指定消息发送失败的回调

            Console.WriteLine("客户端连接成功：" + client.ClientSocket.RemoteEndPoint.ToString());

            startReceive(client);
            //尾递归
            e.AcceptSocket = null;
            startAccept(e);
        }

        #endregion

        #region 接收数据

        /// <summary>
        /// 开始接收数据
        /// </summary>
        private void startReceive(ClientPeer client)
        {
            try
            {
                //这里就将socket和ReceiveArgs绑定起来了
                bool result = client.ClientSocket.ReceiveAsync(client.ReceiveArgs);
                //如果传输直接就完成了，就直接处理，否则等在completed中处理
                if (result == false)
                {
                    processReceiver(client.ReceiveArgs);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        /// <summary>
        /// 当接收完成时，触发的操作
        /// </summary>
        /// <param name="e"></param>
        private void receive_completed(object sender, SocketAsyncEventArgs e)
        {
            processReceiver(e);
        }

        /// <summary>
        /// 处理接收的请求
        /// </summary>
        /// <param name="e"></param>
        private void processReceiver(SocketAsyncEventArgs e)
        {
            //从usertoken中取出发送消息的客户端
            ClientPeer client = e.UserToken as ClientPeer;
            //判断是否接收成功
            int bufferLength = client.ReceiveArgs.BytesTransferred;
            if (client.ReceiveArgs.SocketError == SocketError.Success
                && bufferLength > 0)
            {
                //将消息拷贝到数组中
                byte[] dataPack = new byte[bufferLength];
                Buffer.BlockCopy(client.ReceiveArgs.Buffer, 0, dataPack, 0, bufferLength);
                client.StartReceive(dataPack);
                //尾递归
                startReceive(client);
            }
            //断开连接了
            else if (client.ReceiveArgs.BytesTransferred == 0)
            {
                //客户端最后一次发送消息无误
                if (client.ReceiveArgs.SocketError == SocketError.Success)
                {
                    //客户端主动断开了连接
                    Disconnect(client, "客户端自动断开连接");
                }
                else {
                    //网络异常导致断开了连接
                    Disconnect(client, client.ReceiveArgs.SocketError.ToString());
                }
            }
        }

        /// <summary>
        /// 一条数据解析完成的数据
        /// </summary>
        /// <param name="client">连接对象</param>
        /// <param name="data">消息对象</param>
        private void receiveCompleted(ClientPeer client,SocketMsg data)
        {
            application.OnReceive(client, data);
        }

        #endregion

        #region 断开连接

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="client">断开连接的对象</param>
        /// <param name="readson">原因</param>
        public void Disconnect(ClientPeer client,string readson)
        {
            try
            {
                if (client == null)
                    throw new Exception("客户端对象为空，无法断开连接!");
                Console.WriteLine("客户端断开连接：" + client.ClientSocket.RemoteEndPoint.ToString());
                application.OnDisconnect(client);
                client.Disconnect();
                clientPeerPool.Enqueue(client);
                acceptSemaphore.Release();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        #endregion

    }
}
