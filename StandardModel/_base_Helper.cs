using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace StandardModel
{
    public class _base_manage
    {
        class modelData {
             public string Request;
            public RequestData rd;
        }
        List<modelData> listmode = new List<modelData>();
        public delegate void RequestData(Socket soc,_baseModel _0x01);
        /// <summary>
        /// 请求数据集事件
        /// </summary>
        //public event RequestData  RequestDataEvent = null; 
       


        public delegate void errorMessage(Socket soc,_baseModel _0x01, string message);

        public void AddListen(String Request, RequestData rd)
        {
            modelData md = new modelData();
            md.Request = Request;
            md.rd = rd;
            listmode.Add(md);
        }
        /// <summary>
        /// 错误返回事件
        /// </summary>
        public event errorMessage errorMessageEvent = null;
        public void init(String data, Socket soc)
        {
            _baseModel _0x01= Newtonsoft.Json.JsonConvert.DeserializeObject<_baseModel>(data);
            string message = "";
            try
            {
                if (_0x01 != null && _0x01.Token != "")
                {

                    foreach (modelData md in listmode)
                    {
                        if (md.Request == _0x01.Request)
                        {
                            if(md.rd!=null)
                            md.rd(soc, _0x01);
                        }
                    }
                                    //根据具体功能不同，代码不同
                                    //if (RequestDataEvent != null)
                                    //    RequestDataEvent(soc, _0x01,_0x01.Request); 
                 
                }
            }
            catch (Exception ex)
            {
                _0x01.Parameter = ex.Message;
                message = Newtonsoft.Json.JsonConvert.SerializeObject(_0x01);

                errorMessageEvent(soc,_0x01, ex.Message);
            }
        }
    }
}
