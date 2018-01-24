using AhpilyServer;
using AhpilyServer.Concurrent;
using GameServer.Model;
using System.Collections.Generic;

namespace GameServer.Cache
{
    public class AccountCache
    {
        private ConcurrentInt id = new ConcurrentInt(-1);

        /// <summary>
        /// 帐号数据模型字典
        /// </summary>
        private Dictionary<string, AccountModel> accModelDict = new Dictionary<string, AccountModel>();

        public bool IsExist(string account)
        {
            return accModelDict.ContainsKey(account);
        }

        public void CreateAcount(string acc,string pwd)
        {
            AccountModel model = new AccountModel(id.Add_Get(), acc, pwd);
            accModelDict.Add(acc, model);
        }

        public AccountModel GetAccount(string acc)
        {
            return accModelDict[acc];
        }

        public bool IsMatch(string acc,string pwd)
        {
            AccountModel model = accModelDict[acc];
            return model.Password == pwd;
        }

        /// <summary>
        /// 帐号-连接对象
        /// </summary>
        private Dictionary<string, ClientPeer> accClientDict = new Dictionary<string, ClientPeer>();
        private Dictionary<ClientPeer, string> clientAccDict = new Dictionary<ClientPeer, string>();

        public bool IsOnLine(string acc)
        {
            return accClientDict.ContainsKey(acc);
        }

        public bool IsOnLine(ClientPeer client)
        {
            return clientAccDict.ContainsKey(client);
        }

        public void OnLine(string acc,ClientPeer client)
        {
            accClientDict.Add(acc, client);
            clientAccDict.Add(client, acc);
        }

        public void OffLine(ClientPeer client)
        {
            string acc = clientAccDict[client];
            clientAccDict.Remove(client);
            accClientDict.Remove(acc);
        }

        public void OffLine(string acc)
        {
            ClientPeer client = accClientDict[acc];
            clientAccDict.Remove(client);
            accClientDict.Remove(acc);
        }

        /// <summary>
        /// 获取在线玩家的ID
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public int GetId(ClientPeer client)
        {
            string acc = clientAccDict[client];
            AccountModel model = accModelDict[acc];
            return model.Id;
        }

    }
}
