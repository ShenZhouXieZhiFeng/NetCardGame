using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AhpilyServer
{
    /// <summary>
    /// 客户端连接池
    ///     作用：重用客户端对象
    /// </summary>
    public class ClientPeerPool
    {
        private Queue<ClientPeer> clientQueue = null;

        /// <summary>
        ///  初始化
        /// </summary>
        /// <param name="capacity">连接池上限</param>
        public ClientPeerPool(int capacity)
        {
            clientQueue = new Queue<ClientPeer>(capacity);
            ClientPeer client = null;
            for (int i = 0; i < capacity; i++)
            {
                client = new ClientPeer();
                Enqueue(client);
            }
        }

        /// <summary>
        /// 将客户端放回池中
        /// </summary>
        /// <param name="client"></param>
        public void Enqueue(ClientPeer client)
        {
            clientQueue.Enqueue(client);
        }

        /// <summary>
        /// 从池中获取一个客户端对象
        /// </summary>
        /// <returns></returns>
        public ClientPeer Dequeue()
        {
            return clientQueue.Dequeue();
        }
    }
}
