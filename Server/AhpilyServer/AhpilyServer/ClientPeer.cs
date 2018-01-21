using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace AhpilyServer
{
    public class ClientPeer
    {
        /// <summary>
        /// 客户端Socket
        /// </summary>
        public Socket ClientSocket { get; set; }

        public ClientPeer()
        {
            ReceiveArgs = new SocketAsyncEventArgs();
            sendArgs = new SocketAsyncEventArgs();
            ReceiveArgs.UserToken = this;//设定userToken
            sendArgs.Completed += send_completed;
        }

        #region 接收数据

        public delegate void ReceiveCompleted(ClientPeer client, SocketMsg value);
        /// <summary>
        /// 当一次消息解析完成的回调
        /// </summary>
        public ReceiveCompleted OnReceiveCompleted;

        /// <summary>
        /// 接收的异步套接字请求
        /// </summary>
        public SocketAsyncEventArgs ReceiveArgs { get; set; }

        /// <summary>
        /// 是否正在处理接收的数据
        /// </summary>
        private bool IsReceiveProcess = false;

        /// <summary>
        /// 数据缓存区
        /// </summary>
        private List<byte> dataCache = new List<byte>();

        /// <summary>
        /// 自身处理数据包
        /// </summary>
        /// <param name="dataPack">数据包</param>
        public void StartReceive(byte[] dataPack)
        {
            dataCache.AddRange(dataPack);
            if (!IsReceiveProcess)
                processReceive();
        }

        /// <summary>
        /// 处理正在接收的数据
        /// </summary>
        private void processReceive()
        {
            IsReceiveProcess = true;
            byte[] data = EncodingTool.DecodePacket(ref dataCache);
            if (data == null)
            {
                IsReceiveProcess = false;
                return;
            }
            //转成具体的类型
            SocketMsg msg = EncodingTool.DecodeMsg(data);
            //回调给上层
            if (OnReceiveCompleted != null)
                OnReceiveCompleted(this, msg);
            //尾递归
            processReceive();
        }

        #endregion

        #region 发送数据

        public delegate void SendClientDisconnect(ClientPeer client,string reson);
        /// <summary>
        /// 发送消息失败时的回调（客户端断开连接了）
        /// </summary>
        public SendClientDisconnect OnSendClientDisconnect;

        /// <summary>
        /// 发送流程是否正在执行
        /// </summary>
        private bool IsSendProcess = false;

        /// <summary>
        /// 发送的异步套接字操作
        /// </summary>
        private SocketAsyncEventArgs sendArgs;

        /// <summary>
        /// 发送消息队列
        /// </summary>
        private Queue<byte[]> sendQueue = new Queue<byte[]>();

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="opCode">操作码</param>
        /// <param name="subCode">子操作码</param>
        /// <param name="value">参数</param>
        public void Send(int opCode,int subCode,object value)
        {
            SocketMsg msg = new SocketMsg(opCode, subCode, value);
            byte[] data = EncodingTool.EncodeMsg(msg);
            byte[] packet = EncodingTool.EncodePacket(data);
            sendQueue.Enqueue(packet);
            if(!IsSendProcess)
                processSend();
        }

        /// <summary>
        /// 处理发送流程
        /// </summary>
        private void processSend()
        {
            IsSendProcess = true;
            if (sendQueue.Count == 0)
            {
                IsSendProcess = false;
                return;
            }
            byte[] packet = sendQueue.Dequeue();
            //设置异步套接字操作参数
            sendArgs.SetBuffer(packet, 0, packet.Length);
            bool result = ClientSocket.SendAsync(sendArgs);
            if (result == false)
            {
                //发送结束，尾递归
                sendCompleted();
            }
        }

        /// <summary>
        /// 当异步发送完成时调用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void send_completed(object sender,SocketAsyncEventArgs e)
        {
            sendCompleted();
        }

        /// <summary>
        /// 当一条消息发送完成，做统一处理
        /// </summary>
        private void sendCompleted()
        {
            if (sendArgs.SocketError != SocketError.Success)
            {
                //发送出错 客户端断开连接了
                if (OnSendClientDisconnect != null)
                    OnSendClientDisconnect(this, sendArgs.SocketError.ToString());
            }
            else
            {
                //继续发送
                processSend();
            }
        }
        #endregion

        #region 断开连接

        /// <summary>
        /// 自身的断开连接处理
        /// </summary>
        public void Disconnect()
        {
            dataCache.Clear();
            IsReceiveProcess = false;
            //TODO 清空发送消息

            ClientSocket.Shutdown(SocketShutdown.Both);
            ClientSocket.Close();
            ClientSocket = null;
        }

        #endregion
    }
}
