using WeaveBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace WeaveBase
{
    public interface IDataparsing
    {
        /// <summary>
        /// 把自定义的协议，转化成网关理解的协议
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        WeaveSession GetBaseModel(byte[] data);
        //_0x01.Token 给_baseModel的Token对象赋值
        //_0x01.Request 给Request对象赋值，这个从你的data里面解析出来，Request代表后端逻辑的方法名，
        //你可以设定一个文件做对应，比如1==getnum方法，2==setnum方法
        // _0x01.SetRoot<byte[]> 给ROOT 对象赋值，这个是你传输的数据的内容。内容直接赋值这里就行了。
        /// <summary>
        /// 把网关理解的协议转换成自定义的协议
        /// </summary>
        /// <param name="bm"></param>
        /// <returns></returns>
        byte[] Get_Byte(WeaveBase.WeaveSession bm);
        /// <summary>
        /// 把String字符串转化成自定义协议
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        byte[] Get_ByteBystring(String str);
        /// <summary>
        /// 把网关理解的协议对象的值，进行权限验证
        /// </summary>
        /// <param name="bm"></param>
        /// <returns></returns>
        bool socketvalidation(WeaveBase.WeaveSession bm);
    }
}
