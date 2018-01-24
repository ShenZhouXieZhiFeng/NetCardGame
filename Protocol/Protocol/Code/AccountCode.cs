using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Protocol.Code
{
    //account subcode
    public class AccountCode
    {
        public const int REGIST_CREQ = 0;//client requst AccountDto
        public const int REGIST_SRES = 1;//server response  

        public const int LOGIN_CREQ = 2;//登录客户端请求
        public const int LOGIN_SREQ = 3;//登录服务器响应
    }
}
