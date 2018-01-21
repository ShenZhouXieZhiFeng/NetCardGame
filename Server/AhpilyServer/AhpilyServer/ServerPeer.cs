using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AhpilyServer
{
    class ServerPeer
    {
        //服务端socket
        private Socket serverSocket = null;
        //限制同时连接的客户端数量
        private Semaphore acceptSemaphore;

        /// <summary>
        /// 开启服务
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="maxCount">最大连接数量</param>
        public void Start(int port,int maxCount)
        {
            try
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //限制连接池
                acceptSemaphore = new Semaphore(maxCount, maxCount);
                IPEndPoint ipPoint = new IPEndPoint(IPAddress.Any, port);
                serverSocket.Bind(ipPoint);
                serverSocket.Listen(10);
                Console.WriteLine("服务器启动成功...");
                startAccept(null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
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
            //计数
            acceptSemaphore.WaitOne();
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
            //获得连接的客户端
            Socket clientSocket = e.AcceptSocket;
            //TODO
        }
        #endregion

        #region 接收数据

        #endregion

        #region 发送数据

        #endregion

        #region 断开连接

        #endregion

    }
}
