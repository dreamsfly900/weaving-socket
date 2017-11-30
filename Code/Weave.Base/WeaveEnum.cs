using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Weave.Base
{
    /// <summary>
    /// 有Web,     Json,   Bytes,  Http, udpBytes,jsonudp这几种类型
    /// </summary>
    public enum WeavePortTypeEnum { Web,     Json,   Bytes,  Http, udpBytes,jsonudp }
    /// <summary>
    /// 有Json, Bytes 两种类型
    /// </summary>
    public enum WeaveDataTypeEnum { Json, Bytes };
    /// <summary>
    /// 有ten=10, hundred=100, thousand=1000, ten_thousand=10000 三种
    /// </summary>
    public enum WeavePipelineTypeEnum { ten=10, hundred=100, thousand=1000, ten_thousand=10000 };
}
