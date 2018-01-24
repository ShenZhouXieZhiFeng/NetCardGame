using System;


namespace Protocol
{
    /// <summary>
    /// 网络消息
    /// </summary>
    [Serializable]
    public class SocketMsg
    {
        /// <summary>
        /// 操作码
        /// </summary>
        public int OpCode { get; set; }
        /// <summary>
        /// 子操作
        /// </summary>
        public int SubCode { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        public object Value { get; set; }

        public SocketMsg() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="opCode">操作码</param>
        /// <param name="subCode">子操作</param>
        /// <param name="value">参数</param>
        public SocketMsg(int opCode,int subCode,object value)
        {
            this.OpCode = opCode;
            this.SubCode = subCode;
            this.Value = value;
        }
    }
}
