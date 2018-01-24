using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

/// <summary>
/// 客户端Socket的封装
/// </summary>
public class ClientPeer{

    Socket socket;
    string ip;
    int port;

    public ClientPeer(string ip,int port)
    {
        try
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.ip = ip;
            this.port = port;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void Connect()
    {
        try
        {
            socket.Connect(ip, port);
            Debug.Log("连接服务器！");
            //开始异步接收数据
            startReceive();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            throw;
        }
    }

    #region 接收数据
    private bool isReceiveProcess = false;
    private byte[] receiveBuffer = new byte[1024];
    private List<byte> dataCache = new List<byte>();
    public Queue<SocketMsg> socketMsgQueue = new Queue<SocketMsg>();

    void startReceive()
    {
        if (socket == null || !socket.Connected)
        {
            Debug.LogError("无连接");
            return;
        }

        socket.BeginReceive(receiveBuffer, 0, 1024, SocketFlags.None, receiveCallBack, socket);
    }

    /// <summary>
    /// 收到消息的回调
    /// </summary>
    /// <param name="ar"></param>
    void receiveCallBack(IAsyncResult ar)
    {
        try
        {
            Debug.Log("收到消息");
            int length = socket.EndReceive(ar);
            byte[] tmpByteArray = new byte[length];
            Buffer.BlockCopy(receiveBuffer, 0, tmpByteArray, 0, length);
            dataCache.AddRange(tmpByteArray);
            //处理收到的消息
            if (isReceiveProcess == false)
            {
                processReceive();
            }
            startReceive();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    void processReceive()
    {
        isReceiveProcess = true;

        byte[] data = EncodingTool.DecodePacket(ref dataCache);
        if (data == null)
        {
            isReceiveProcess = false;
            return;
        }
        SocketMsg msg = EncodingTool.DecodeMsg(data);
        //存储数据，等待处理
        socketMsgQueue.Enqueue(msg);

        //尾递归
        processReceive();
    }
    #endregion

    #region 发送数据

    public void Send(int opCode,int subCode,object value)
    {
        SocketMsg msg = new SocketMsg(opCode, subCode, value);
        Send(msg);
    }

    public void Send(SocketMsg msg)
    {
        byte[] data = EncodingTool.EncodeMsg(msg);
        byte[] packet = EncodingTool.EncodePacket(data);

        try
        {
            socket.Send(packet);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    #endregion
}
