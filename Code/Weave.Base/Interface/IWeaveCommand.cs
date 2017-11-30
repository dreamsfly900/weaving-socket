using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Weave.Base.Interface
{
    /// <summary>
    /// 含有一个 RunCommand<T>(T DataSer, WeaveSockets mysoc)可传递一个泛型类的接口
    /// </summary>
    public interface IWeaveCommand
    {
        void RunCommand<T>(T DataSer, WeaveSockets mysoc);
    }
}
