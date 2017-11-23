using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeaveBase.Helper
{
    public static class WeaveServerHelper
    {

        public static int ConvertToInt(byte[] list)
        {
            int ret = 0;
            int i = 0;
            foreach (byte item in list)
            {
                ret = ret + (item << i);
                i = i + 8;
            }
            return ret;
        }
        public static byte[] ConvertToByteList(int v)
        {
            List<byte> ret = new List<byte>();
            int value = v;
            while (value != 0)
            {
                ret.Add((byte)value);
                value = value >> 8;
            }
            byte[] bb = new byte[ret.Count];
            ret.CopyTo(bb);
            return bb;
        }

        /// <summary>
        /// 对数据进行编码，第1位为命名位，第2位为第三位的数据长度，第三部分为第四部分的数据长度，第四部分为实际数据
        /// </summary>
        /// <param name="command"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static byte[] CodingProtocol(byte command, string text)
        {
            byte[] sendb = System.Text.Encoding.UTF8.GetBytes(text);
            byte[] part3_length = System.Text.Encoding.UTF8.GetBytes(sendb.Length.ToString());
            byte[] b = new byte[2 + part3_length.Length + sendb.Length];
            b[0] = command;
            b[1] = (byte)part3_length.Length;
            part3_length.CopyTo(b, 2);
            //扩充 第四部分数据（待发送的数据）的长度，扩充到b数组第三位开始的后面
            sendb.CopyTo(b, 2 + part3_length.Length);
            //扩充 第四部分数据实际的数据，扩充到b数组第三部分结尾后面...

            return b;
        }

        /// <summary>
        /// 对数据进行编码，第1位为命名位，第2位为第三位的数据长度，第三部分为第四部分的数据长度，第四部分为实际数据
        /// </summary>
        /// <param name="command"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static byte[] CodingProtocol(byte command, byte[] text)
        {
            byte[] sendb = text;
            byte[] part3_length = System.Text.Encoding.UTF8.GetBytes(sendb.Length.ToString());
            byte[] b = new byte[2 + part3_length.Length + sendb.Length];
            b[0] = command;
            b[1] = (byte)part3_length.Length;
            part3_length.CopyTo(b, 2);
            //扩充 第四部分数据（待发送的数据）的长度，扩充到b数组第三位开始的后面
            sendb.CopyTo(b, 2 + part3_length.Length);
            //扩充 第四部分数据实际的数据，扩充到b数组第三部分结尾后面...

            return b;
        }


    }
}
