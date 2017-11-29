using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Weave.Base.Interface
{
    public interface IWeaveCommand
    {
        void RunCommand<T>(T DataSer, WeaveSockets mysoc);
    }
}
