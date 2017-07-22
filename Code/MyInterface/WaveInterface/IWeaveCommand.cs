using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace WeaveBase
{
    public interface IWeaveCommand
    {
        void RunCommand<T>(T DataSer, WeaveSockets mysoc);
    }
}
