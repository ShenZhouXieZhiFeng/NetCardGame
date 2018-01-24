using Protocol;
using Protocol.Code;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetManager : ManagerBase {

    public static NetManager Instance = null;

    private void Awake()
    {
        Instance = this;
        Add(OpCode.ACCOUNT, this);
    }

    public override void Execute(int eventCode, object message)
    {
        //接收事件并处理
        switch (eventCode)
        {
            case OpCode.ACCOUNT:
                client.Send(message as SocketMsg);
                break;
            default:
                break;
        }
    }

    private ClientPeer client = null;

    public void Connetced(string ip,int port)
    {
        if(client == null)
            client = new ClientPeer(ip, port);
        client.Connect();
    }

    private void Update()
    {
        if (client == null)
            return;
        //处理数据
        while (client.socketMsgQueue.Count > 0)
        {
            SocketMsg msg = client.socketMsgQueue.Dequeue();
            //TODO 根据msg做处理
            Debug.Log(msg.Value.ToString());
        }
    }

}
