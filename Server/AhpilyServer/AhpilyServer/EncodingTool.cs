using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace AhpilyServer
{
    /// <summary>
    /// 编码工具类
    /// </summary>
    public static class EncodingTool
    {
        #region  粘包拆包

        /// <summary>
        /// 构造消息：消息头 + 消息体
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] EncodePacket(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(data.Length);
                    bw.Write(data);
                    byte[] res = new byte[(int)ms.Length];
                    Buffer.BlockCopy(ms.GetBuffer(), 0, res, 0, (int)ms.Length);
                    return res;
                }
            }
        }

        /// <summary>
        /// 解析数据包，从原始数据中解析出数据主体
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] DecodePacket(ref List<byte> dataCache)
        {
            //4个数据表示一个int长度
            if (dataCache.Count < 4)
                return null;
                //throw new Exception("数据缓存不足4");
            using (MemoryStream ms = new MemoryStream(dataCache.ToArray()))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    //获取数据主体的长度
                    int length = br.ReadInt32();
                    //剩余数据长度
                    int dataRemainLength = (int)(ms.Length - ms.Position);
                    //判断剩余数据长度是否满足一个数据主体
                    if (length > dataRemainLength)
                        return null;
                        //throw new Exception("数据长度不足包头约定长度，不构成一个完整的消息");
                    byte[] data = br.ReadBytes(length);
                    //更新缓存
                    dataCache.Clear();
                    dataCache.AddRange(br.ReadBytes(dataRemainLength));
                    return data;
                }
            }
         }


        #endregion

        #region 构造网络SocketMsg类

        /// <summary>
        /// 将socketMsg类转换成字节数组 便于发送
        /// </summary>
        /// <param name="msg">消息实例</param>
        /// <returns></returns>
        public static byte[] EncodeMsg(SocketMsg msg)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write(msg.OpCode);
                    bw.Write(msg.SubCode);
                    if (msg.Value != null)
                    {
                        byte[] valueBytes = EncodeObj(msg.Value);
                        bw.Write(valueBytes);
                    }
                    byte[] data = new byte[ms.Length];
                    Buffer.BlockCopy(ms.GetBuffer(), 0, data, 0, (int)ms.Length);
                    return data;
                }
            }
        }

        /// <summary>
        /// 将字节数组还原成SocketMsg实例
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static SocketMsg DecodeMsg(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    SocketMsg msg = new SocketMsg();
                    msg.OpCode = br.ReadInt32();
                    msg.SubCode = br.ReadInt32();
                    if (ms.Length > ms.Position)
                    {
                        //还有剩余的数据
                        byte[] valueBytes = br.ReadBytes((int)(ms.Length - ms.Position));
                        object value = DecodeObj(valueBytes);
                        msg.Value = value;
                    }
                    return msg;
                }
            }
        }

        #endregion

        #region object2bytes

        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] EncodeObj(object value)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, value);
                byte[] valueBytes = new byte[ms.Length];
                Buffer.BlockCopy(ms.GetBuffer(), 0, valueBytes, 0, (int)ms.Length);
                return valueBytes;
            }
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <returns></returns>
        public static object DecodeObj(byte[] valueBytes)
        {
            using (MemoryStream ms = new MemoryStream(valueBytes))
            {
                BinaryFormatter bf = new BinaryFormatter();
                object obj = bf.Deserialize(ms);
                return obj;
            }
        }

        #endregion
    }
}
