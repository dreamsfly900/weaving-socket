using System;
using System.Collections.Generic;
using System.Net.Sockets;
namespace Weave.Base
{
    public class WeaveBaseManager
    {
      public  List<WeaveModelData> listmode = new List<WeaveModelData>();
        public event WeaveErrorMessageDelegate WeaveErrorMessageEvent = null;
        /// <summary>
        /// 请求数据集事件
        /// </summary>
        //public event RequestData  RequestDataEvent = null; 
        public void AddListen(String Request, WeaveRequestDataDelegate rd,String type)
        {
            WeaveModelData md = new WeaveModelData();
            md.Request = Request;
            md.rd = rd;
            md.type = type;
            md.dtu = false;
            listmode.Add(md);
        }
        public void AddListen(String Request, WeaveRequestDataDtuDelegate rd, String type,bool dtu)
        {
            WeaveModelData md = new WeaveModelData();
            md.Request = Request;
            md.rd2 = rd;
            md.type = type;
            md.dtu = dtu;
            listmode.Add(md);
        }
        public void DeleteListen(String Reques)
        {
            int count = listmode.Count;
            WeaveModelData[] mds = new WeaveModelData[count];
            listmode.CopyTo(mds);
            foreach (WeaveModelData md in mds)
            {
                listmode.Remove(md);
            }
        }
        /// <summary>
        /// 错误返回事件
        /// </summary>
        public void Init(String data, Socket soc)
        {
            WeaveSession _0x01= Newtonsoft.Json.JsonConvert.DeserializeObject<WeaveSession>(data);
            string message = "";
            try
            {
                if (_0x01 != null && _0x01.Token != "")
                {
                    int count = listmode.Count;
                    WeaveModelData[] mds = new WeaveModelData[count];
                    listmode.CopyTo(mds);
                    foreach (WeaveModelData md in mds)
                    {
                        if (md.Request == _0x01.Request)
                        {
                            if (md.rd2 != null && md.dtu==true)
                            {
                                WeaveRequestDataDtuDelegate rdh = new WeaveRequestDataDtuDelegate(md.rd2);
                                byte[] b= Newtonsoft.Json.JsonConvert.DeserializeObject<byte[]>(_0x01.Root); 
                                rdh(soc, b, _0x01.Token.Split('|')[0],Convert.ToInt32(_0x01.Token.Split('|')[1]));
                                if (md.type == "once")
                                {
                                    listmode.Remove(md);
                                }
                            }
                            if (md.rd != null && md.dtu == false)
                            {
                                //md.rd(soc, _0x01);
                                WeaveRequestDataDelegate rdh = new WeaveRequestDataDelegate(md.rd);
                                rdh(soc, _0x01);
                                if (md.type == "once")
                                {
                                    listmode.Remove(md); 
                                }
                            }
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
                WeaveErrorMessageEvent(soc,_0x01, ex.Message);
            }
        }
    }
}
