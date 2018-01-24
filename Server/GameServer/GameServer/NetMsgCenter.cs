using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AhpilyServer;
using GameServer.Logic;
using Protocol.Code;
using Protocol;

namespace GameServer
{
    /// <summary>
    /// 网络消息中西
    /// </summary>
    public class NetMsgCenter : IApplicationBase
    {
        IHandler account = new AccountHandler();

        public void OnDisconnect(ClientPeer client)
        {
            account.OnDisconnect(client);
        }

        public void OnReceive(ClientPeer client, SocketMsg msg)
        {
            switch (msg.OpCode)
            {
                case OpCode.ACCOUNT:
                    //帐号
                    account.OnReceiver(client, msg.SubCode, msg.Value);
                    break;
                default:
                    break;
            }
        }
    }
}
