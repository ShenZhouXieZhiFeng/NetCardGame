using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetManager : ManagerBase {

    public static NetManager Instance = null;

    private void Awake()
    {
        Instance = this;
    }

    private ClientPeer client;

    public void Connetced(string ip,int port)
    {
        client = new ClientPeer(ip, port);
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
        }
    }

}
