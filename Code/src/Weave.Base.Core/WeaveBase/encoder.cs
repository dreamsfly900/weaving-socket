using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Weave.Base.WeaveBase
{
    public  class encoder
    {
        public byte[] sendBitencoder(byte command, byte[] text)
        {
            byte[] sendb = text;
            byte[] lens = ConvertToByteList(sendb.Length);
            byte[] b = new byte[2 + 2 + lens.Length + sendb.Length];
            b[0] = command;
            b[1] = (byte)lens.Length;
            lens.CopyTo(b, 2);
            CRC.ConCRC(ref b, 2 + lens.Length);
            sendb.CopyTo(b, 2 + 2 + lens.Length);
            return b;
        }
        public byte[] sendJsonencoder(byte command, string text)
        {
            byte[] sendb = System.Text.Encoding.UTF8.GetBytes(text);
            byte[] lens = System.Text.Encoding.UTF8.GetBytes(sendb.Length.ToString());
            byte[] b = new byte[2 + lens.Length + sendb.Length];
            b[0] = command;
            b[1] = (byte)lens.Length;
            lens.CopyTo(b, 2);
            sendb.CopyTo(b, 2 + lens.Length);
            return b;
        }
        public int ConvertToInt(byte[] list)
        {
            int ret = 0;
            int i = 0;
            foreach (byte item in list)
            {
                ret = ret + (item << i);
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
        public byte[] packageDatabtye(byte[] alldata, Socket soc, WaitCallback ReceiveBitEventHandercback,
            SslStream ssl, WaitCallback ReceiveBitEventHandercbackssl)
        {
            try
            {
                //int i = 0;
                //int count = ListData.Count;
                //  if (count > 0)
                {
              
                    int bytesRead = alldata.Length;
                    if (bytesRead == 0)
                    {
                        //  if (ListData.Count > 0) ListData.RemoveAt(0);
                        return alldata;
                    };

                    byte[] tempbtye = new byte[bytesRead];
                    Buffer.BlockCopy(alldata,0, tempbtye,0, tempbtye.Length);
                    if (bytesRead > 2)
                    {
                        int a = tempbtye[1];
                        if (a == 0)
                        {
                            byte[] temps = new byte[tempbtye.Length - 2];
                            Buffer.BlockCopy(tempbtye, 2, temps, 0, temps.Length);
                            alldata = temps;
                            Console.WriteLine("a == 0");
                            return alldata;
                        }
                        else if (bytesRead > 4 + a)
                        {
                            int len = 0;

                            byte[] bbcrc = new byte[4 + a];
                            Buffer.BlockCopy(tempbtye, 0, bbcrc, 0, 4 + a);
                            if (CRC.DataCRC(ref bbcrc, 4 + a))
                            {
                                byte[] bb = new byte[a];
                                Buffer.BlockCopy(tempbtye, 2, bb, 0, a);
                                len = ConvertToInt(bb);
                            }
                            else
                            {
                                byte[] temps = new byte[tempbtye.Length - 1];
                                Buffer.BlockCopy(tempbtye, 1, temps, 0, temps.Length);
                                alldata = temps;
                                Console.WriteLine("DataCRC == false");
                                return alldata;
                            }



                            if ((len + 4 + a) > tempbtye.Length)
                            {

                                return alldata;
                            }
                            else if (tempbtye.Length > (len + 4 + a))
                            {
                                try
                                {
                                    byte[] temps = new byte[tempbtye.Length - (len + 4 + a)];
                                    Buffer.BlockCopy(tempbtye, (len + 4 + a), temps, 0, temps.Length);
                                    alldata = temps;
                                }
                                catch(Exception e)
                                {
                                    Console.WriteLine("tempbtye.Length > (len + 4 + a):" + e.Message);
                                    return alldata;
                                }
                                //netc.IsPage = false; return;
                            }
                            else if (tempbtye.Length == (len + 4 + a))
                            {
                                alldata = new byte[0];
                            }
                            try
                            {

                                //  temp = System.Text.Encoding.UTF8.GetString(tempbtye, 2 + a, len);
                                byte[] bs = new byte[len];
                                Buffer.BlockCopy(tempbtye, (4 + a), bs, 0, bs.Length);
                                WeaveEvent me = new WeaveEvent();
                                me.Command = tempbtye[0];
                                me.Data = "";
                                me.Databit = bs;
                                me.Soc = soc;
                                me.Ssl = ssl;
                                if (ssl == null)
                                {
                                    //  Task.Run(() => { ReceiveBitEventHandercback(me); });
                                    System.Threading.ThreadPool.UnsafeQueueUserWorkItem(ReceiveBitEventHandercback, me);
                                }
                                else
                                    System.Threading.ThreadPool.UnsafeQueueUserWorkItem(ReceiveBitEventHandercbackssl, me);
                                // if (weaveReceiveBitEvent != null)
                               // System.Threading.ThreadPool.QueueUserWorkItem(ReceiveBitEventHandercback, me);
                                //weaveReceiveBitEvent?.Invoke(tempbtye[0], bs, soc);
                                // weaveReceiveBitEvent?.BeginInvoke(tempbtye[0], bs, soc,null,null);

                                return alldata;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("ee:" + e.Message);
                                // netc.IsPage = false;
                                return new byte[0];
                            }
                        }
                        else
                        {

                            return alldata;
                        }
                    }
                    else
                    {

                        return alldata;
                    }
                }
            }
            catch(Exception ee)
            {
                Console.WriteLine("all:" + ee.Message);

                return new byte[0];
            }

        }

        /// <summary>
        /// 对粘包，分包的处理方法
        /// </summary>
        /// <param name="alldata"></param>
        /// <param name="soc"></param>
        /// <param name="ReceiveBitEventHandercback"></param>
        /// <param name="ssl"></param>
        /// <param name="ReceiveBitEventHandercbackssl"></param>
        
        public byte[] packageDatajson(byte[] alldata, Socket soc, WaitCallback ReceiveBitEventHandercback,
            SslStream ssl, WaitCallback ReceiveBitEventHandercbackssl)
        {

            try
            {

                {
                lb1122:
                    int bytesRead = alldata.Length;
                    if (bytesRead == 0)
                    {

                        return alldata;
                    };

                    byte[] tempbtye = new byte[bytesRead];
                     Buffer.BlockCopy(alldata,0, tempbtye,0, tempbtye.Length);
                    if (bytesRead > 2)
                    {
                        int a = tempbtye[1];
                        if (a == 0)
                        {
                            byte[] temps = new byte[tempbtye.Length - 2];
                            Buffer.BlockCopy(tempbtye, 2, temps, 0, temps.Length);
                            alldata = temps;
                            goto lb1122;
                        }
                        else if (bytesRead > 2 + a)
                        {
                            int len = 0;

                            String temp = System.Text.Encoding.UTF8.GetString(tempbtye, 2, a);
                            len = int.Parse(temp);


                            if ((len + 2 + a) > tempbtye.Length)
                            {

                                return alldata;
                            }
                            else if (tempbtye.Length > (len + 2 + a))
                            {
                                try
                                {
                                    byte[] temps = new byte[tempbtye.Length - (len + 2 + a)];
                                    Buffer.BlockCopy(tempbtye, (len + 2 + a), temps, 0, temps.Length);
                                    alldata = temps;
                                }
                                catch
                                {

                                    return alldata;
                                }
                                //netc.IsPage = false; return;
                            }
                            else if (tempbtye.Length == (len + 2 + a))
                            {
                                alldata = new byte[0];
                            }
                            try
                            {

                                String temp2 = System.Text.Encoding.UTF8.GetString(tempbtye, 2 + a, len);
                                WeaveEvent me = new WeaveEvent();
                                me.Command = tempbtye[0];
                                me.Data = temp2;
                                me.Soc = soc;
                                me.Ssl = ssl;
                                //if (waveReceiveEvent != null)
                                //  waveReceiveEvent?.Invoke(tempbtye[0], temp, soc);
                                if (ssl==null)
                                    System.Threading.ThreadPool.QueueUserWorkItem(ReceiveBitEventHandercback, me);
                                else
                                    System.Threading.ThreadPool.QueueUserWorkItem(ReceiveBitEventHandercbackssl, me);

                                //receiveeventto(me);
                                //if (receiveevent != null)
                                //    waveReceiveEvent.BeginInvoke(tempbtye[0], temp, soc, null, null);
                                //if (ListData.Count > 0) ListData.RemoveAt(i);


                                return alldata;
                            }
                            catch //(Exception e)
                            {
                                // netc.IsPage = false;
                                return new byte[0];
                            }
                        }
                        else
                        {

                            return alldata;
                        }
                    }
                    else
                    {

                        return alldata;
                    }
                }
            }
            catch
            {

                return new byte[0];
            }


        }




    }
}
