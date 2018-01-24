using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AhpilyServer;
using Protocol.Dto;
using Protocol.Code;
using GameServer.Cache;

namespace GameServer.Logic
{
    /// <summary>
    /// 帐号处理逻辑层
    /// </summary>
    public class AccountHandler : IHandler
    {
        AccountCache accountCache = Caches.Account;

        public void OnDisconnect(ClientPeer client)
        {

        }

        public void OnReceiver(ClientPeer client, int subCode, object value)
        {
            switch (subCode)
            {
                case AccountCode.REGIST_CREQ:
                    {
                        AccountDto dto = value as AccountDto;
                        //Console.WriteLine("注册:" + dto.Account + "||" + dto.Password);
                        regist(client, dto.Account, dto.Password);
                    }
                    break;
                case AccountCode.LOGIN_CREQ:
                    {
                        AccountDto dto = value as AccountDto;
                        //Console.WriteLine("登录:" + dto.Account + "||" + dto.Password);
                        login(client, dto.Account, dto.Password);
                    }
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="client"></param>
        /// <param name="acc"></param>
        /// <param name="pwd"></param>
        void regist(ClientPeer client,string acc,string pwd)
        {
            SingleExecute.Instance.Execute(() =>
            {
                if (accountCache.IsExist(acc))
                {
                    //帐号已存在
                    client.Send(OpCode.ACCOUNT, AccountCode.REGIST_SRES, "帐号已存在");
                    return;
                }
                if (string.IsNullOrEmpty(acc))
                {
                    //帐号不合法
                    client.Send(OpCode.ACCOUNT, AccountCode.REGIST_SRES, "帐号不合法");
                    return;
                }
                if (string.IsNullOrEmpty(pwd) || pwd.Length < 4 || pwd.Length > 16)
                {
                    //密码不合法
                    client.Send(OpCode.ACCOUNT, AccountCode.REGIST_SRES, "密码不合法");
                    return;
                }
                //注册
                accountCache.CreateAcount(acc, pwd);
                client.Send(OpCode.ACCOUNT, AccountCode.REGIST_SRES, "注册成功");
            });
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="client"></param>
        /// <param name="acc"></param>
        /// <param name="pwd"></param>
        void login(ClientPeer client, string acc, string pwd)
        {
            SingleExecute.Instance.Execute(() =>
            {
                if (!accountCache.IsExist(acc))
                {
                    //帐号不存在
                    client.Send(OpCode.ACCOUNT, AccountCode.LOGIN_SREQ, "帐号不存在");
                    return;
                }
                if (accountCache.IsOnLine(acc))
                {
                    //帐号已经登录
                    client.Send(OpCode.ACCOUNT, AccountCode.LOGIN_SREQ, "帐号已经登录");
                    return;
                }
                if (accountCache.GetAccount(acc).Password != pwd)
                {
                    //密码不正确
                    client.Send(OpCode.ACCOUNT, AccountCode.LOGIN_SREQ, "密码不正确");
                    return;
                }
                //登录
                accountCache.OnLine(acc, client);
                client.Send(OpCode.ACCOUNT, AccountCode.LOGIN_SREQ, "登录成功");
            });
        }
    }
}
