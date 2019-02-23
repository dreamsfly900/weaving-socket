using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Weave.Base
{
    public class WeaveBaseManager
    {
        public List<WeaveModelData> listmode = new List<WeaveModelData>();
        public event WeaveErrorMessageDelegate WeaveErrorMessageEvent = null;

        /// <summary>
        /// 请求数据集事件
        /// </summary>
        public void AddListen(string Request, WeaveRequestDataDelegate rd, string type)
        {
            WeaveModelData md = new WeaveModelData
            {
                Request = Request,
                Rd = rd,
                Type = type,
                Dtu = false
            };
            listmode.Add(md);
        }

        public void AddListen(string Request, WeaveRequestDataDtuDelegate rd, string type, bool dtu)
        {
            WeaveModelData md = new WeaveModelData
            {
                Request = Request,
                Rd2 = rd,
                Type = type,
                Dtu = dtu
            };
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
        public void Init(string data, Socket soc)
        {
            WeaveSession _0x01 = Newtonsoft.Json.JsonConvert.DeserializeObject<WeaveSession>(data);
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
                            if (md.Rd2 != null && md.Dtu == true)
                            {
                                WeaveRequestDataDtuDelegate rdh = new WeaveRequestDataDtuDelegate(md.Rd2);
                                byte[] b = Newtonsoft.Json.JsonConvert.DeserializeObject<byte[]>(_0x01.Root);
                                rdh(soc, b, _0x01.Token.Split('|')[0], Convert.ToInt32(_0x01.Token.Split('|')[1]));
                                if (md.Type == "once")
                                {
                                    listmode.Remove(md);
                                }
                            }
                            if (md.Rd != null && md.Dtu == false)
                            {
                                WeaveRequestDataDelegate rdh = new WeaveRequestDataDelegate(md.Rd);
                                rdh(soc, _0x01);
                                if (md.Type == "once")
                                {
                                    listmode.Remove(md);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _0x01.Parameter = ex.Message;
                message = Newtonsoft.Json.JsonConvert.SerializeObject(_0x01);
                WeaveErrorMessageEvent(soc, _0x01, ex.Message);
            }
        }
    }
}
