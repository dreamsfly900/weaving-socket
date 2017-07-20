using MyInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StandardModel;

namespace 智信构建结构
{
    public class Bytepackage : IDataparsing
    {
        /// <summary>
        /// 将传输的BYTE字节内容转为基础类
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public _baseModel Get_baseModel(byte[] data)
        {
            //data 的内容为，协议中的内容段，不明白的查看 客户端协议的说明，内容段是自己定义的内容
            _baseModel bm = new _baseModel();
            //bm.Request 注意这个属性必须要赋值，赋值的内容和你后端的执行处理的方法一致，代表你需要那个方法处理他。
            // bm.SetRoot<byte[]>(data); 可以用SetRoot 方法把整段内容发送给后端方法处理
            return new _baseModel();
        }
        /// <summary>
        /// 将基础类转为BYTE字节
        /// </summary>
        /// <param name="bm"></param>
        /// <returns></returns>
        public byte[] Get_Byte(_baseModel bm)
        {
           
            return new byte[0];
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
        public bool socketvalidation(_baseModel bm)
        {
            //这个方法主要是鉴权，如果内容不正确，返回false将不会继续向下执行
            return true;
        }
    }
}
