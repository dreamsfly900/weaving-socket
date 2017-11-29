using System;
using System.Collections.Generic;
using Weave.Base.Interface;
using Weave.Base;
namespace 智信构建结构
{
    public class Bytepackage : IDataparsing
    {
        /// <summary>
        /// 将传输的BYTE字节内容转为基础类
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public WeaveSession GetBaseModel(byte[] data)
        {
            //data 的内容为，协议中的内容段，不明白的查看 客户端协议的说明，内容段是自己定义的内容
            WeaveSession bm = new WeaveSession();
            byte[] bs = new byte[2];
            Array.Copy(data, 0, bs, 0, bs.Length);
            int req= ConvertToInt(bs);
            if (req == 1)
            {
                bm.Request = "getdata";
            }
            //bm.Request 注意这个属性必须要赋值，赋值的内容和你后端的执行处理的方法一致，代表你需要那个方法处理他。
            byte[] b = new byte[data.Length-2];
            Array.Copy(data, 2, b, 0, b.Length);
             bm.SetRoot<byte[]>(b); //可以用SetRoot 方法把整段内容发送给后端方法处理
            bm.Token = "这里不能是空的，如果没有，就自己随便加一个内容";
            return bm;
        }
        /// <summary>
        /// 将基础类转为BYTE字节
        /// </summary>
        /// <param name="bm"></param>
        /// <returns></returns>
        public byte[] Get_Byte(WeaveSession bm)
        {
            byte[] b = bm.GetRoot<byte[]>();
            byte[] data = new byte[2 + b.Length];
            byte[] req=new byte[0];
            if (bm.Request == "getdata")
            {
              req=   ConvertToByteList(256);
            }
            if (req.Length == 2)
            {
                data[0] = req[0];
                data[1] = req[1];
            }
            if (req.Length < 2)
            {
                data[0] =0;
                data[1] = req[1];
            }
            Array.Copy(b, 0, data, 2, b.Length);
                return data;
        }
        /// <summary>
        /// 将String转成 byte字节，最常见内部调同 "token|" + Token + "
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public byte[] Get_ByteBystring(string str)
        {
            if(str.Split('|')[0]== "token")
            { 
                //str.Split('|')[1]把token 转成byte自己，如果你不写，那返回的内容就是空的token。
            }
               return new byte[0];
        }
        /// <summary>
        /// 协议鉴权
        /// </summary>
        /// <param name="bm"></param>
        /// <returns></returns>
        public bool socketvalidation(WeaveSession bm)
        {
            //这个方法主要是鉴权，如果内容不正确，返回false将不会继续向下执行
            return true;
        }
        public int ConvertToInt(byte[] list)
        {
            
            int ret = 0;
            int i = 0;
            foreach (byte item in list)
            {
              

                ret = ret + (item << i);
                if(ret!=0)
                i = i + 8;
            }
            return ret;
        }
        public byte[] ConvertToByteList(int v)
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
    }
}
