using AhpilyServer;
using System;
using System.Collections.Generic;


namespace GameServer.Logic
{
    /// <summary>
    /// 逻辑层基类
    /// </summary>
    public interface IHandler
    {
        void OnReceiver(ClientPeer client,int subCode,object value);

        void OnDisconnect(ClientPeer client);
    }
}
