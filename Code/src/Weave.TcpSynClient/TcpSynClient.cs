using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Weave.Base;

namespace Weave.Client
{
    public enum DataType { bytes, custom };
    public class command { public byte comand; public byte[] data; }
    public delegate void myreceivebitobj(byte command, byte[] data, TcpSynClient soc);
    public class TcpSynClient
    {
        DataType DT;
        string IP;
        int PORT;
        TcpClient tcpc;
        bool Isline = false;
        bool isok = false;
        DateTime timeout;
        public int mintimeout = 90;
        public TcpSynClient(DataType _DT, string ip, int port)
        {
            IP = ip;
            PORT = port;
            DT = _DT;
        }
       public  bool Start()
        {
            try
            {
               
               
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(IP), PORT);
                tcpc = new TcpClient
                {
                    // ExclusiveAddressUse = false,
                    ReceiveBufferSize = int.MaxValue
                };

                tcpc.Connect(IP, PORT);
                //localprot = ((IPEndPoint)tcpc.Client.LocalEndPoint).Port.ToString();
                Isline = true;
                isok = true;

                timeout = DateTime.Now;

                return true;
            }
            catch (Exception e)
            {
                Isline = false;
                throw e;
               
            }
        }
        public void Stop()
        {
            isok = false;
            tcpc.Close();
        }
        byte[] alldata = new byte[0];
        public  command Receives(myreceivebitobj funobj)
        {
            //var task = Task.Run(() =>
            //{
            //    var w = new SpinWait();
            DateTime dt=new DateTime();
            while (isok)
                {
               
                try
                    {
                        if (tcpc.Client == null)
                        {

                            return null;

                        }
                        int bytesRead = tcpc.Client.Available;
                        if (bytesRead > 0)
                        {
                            byte[] tempbtye = new byte[bytesRead];
                            try
                            {
                                timeout = DateTime.Now;
                            dt = DateTime.Now;
                            tcpc.Client.Receive(tempbtye);
                           
                            byte[] tempp = new byte[alldata.Length];
                                alldata.CopyTo(tempp, 0);
                                int lle = alldata.Length;
                                bytesRead = tempbtye.Length;
                                byte[] temp = new byte[lle + bytesRead];
                                Array.Copy(alldata, 0, temp, 0, lle);
                                Array.Copy(tempbtye, 0, temp, lle, bytesRead);
                                alldata = temp;
                                 
                            }
                            catch (Exception ee)
                            {
                                throw ee;
                                // ErrorMge(22, ee.Message);
                            }

                        }


                    if (alldata.Length > 3)
                    {
                        var outcommand = Unup(funobj);

                        if (outcommand != null)
                        {
                            DateTime dt2 = DateTime.Now;
                            //  Console.WriteLine("TcpSynClient:" + (dt2 - dt).TotalMilliseconds);
                            return outcommand;
                        }
                        else
                        { }
                    }
                    else
                    { System.Threading.Thread.Yield(); }
                        

                        try
                        {
                            TimeSpan ts = DateTime.Now - timeout;
                            if (ts.TotalSeconds > mintimeout)
                            {
                                Isline = false;

                                break;
                            }
                        }
                        catch (Exception ee)
                        {
                            throw ee;
                        }

                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                tcpc.Close();
                Isline = false;
                return null;
           // });
            //return null;
        }
        command Unup(myreceivebitobj funobj)
        {
            if (DT == DataType.custom)
            {
                int bytesRead = alldata.Length;

                if (bytesRead == 0)
                {
                    return null;
                }
                byte[] tempbtye = new byte[bytesRead];

                Array.Copy(alldata, tempbtye, tempbtye.Length);
                funobj?.Invoke(0x0, tempbtye, this);
             
                alldata = new byte[0];
                command command = new command();
                command.comand = 0;
                command.data = tempbtye;
                return command;
            }
            
            else if (DT == DataType.bytes)
            {
                return Unupbyte( funobj);
             
            }
            return null;
        }


        command Unupbyte(myreceivebitobj funobj)
        {
            try
            {
                
                lb0x99:
                    int bytesRead = alldata.Length;

                    if (bytesRead == 0)
                    {
                        return null;
                    }

                    byte[] tempbtye = new byte[bytesRead];

                    Array.Copy(alldata, tempbtye, tempbtye.Length);
                   

                    if (bytesRead > 2)
                    {
                        int a = tempbtye[1];
                    if (a == 0)
                    { }
                        if (bytesRead > 4 + a)
                        {
                            int len = 0;

                            byte[] bbcrc = new byte[4 + a];
                            Array.Copy(tempbtye, 0, bbcrc, 0, 4 + a);
                            if (CRC.DataCRC(ref bbcrc, 4 + a))
                            {
                                byte[] bb = new byte[a];
                                Array.Copy(tempbtye, 2, bb, 0, a);
                                len = ConvertToInt(bb);
                            }
                            else
                            {
                                byte[] temps = new byte[tempbtye.Length - 1];
                                Array.Copy(tempbtye, 1, temps, 0, temps.Length);
                                alldata = temps;
                                //return;
                                goto lb0x99;
                            }
                            try
                            {
                                if ((len + 4 + a) > tempbtye.Length)
                                {
                                    return null;
                                }
                                else if (tempbtye.Length > (len + 4 + a))
                                {
                                    byte[] temps = new byte[tempbtye.Length - (len + 4 + a)];
                                    Array.Copy(tempbtye, (len + 4 + a), temps, 0, temps.Length);
                                    alldata = temps;
                                    //return;
                                    //  goto lb0x99;
                                }
                                else if (tempbtye.Length == (len + 4 + a))
                                { alldata = new byte[0]; }
                            }
                            catch (Exception e)
                            {
                               // ErrorMge?.Invoke(3, e.StackTrace + "unup001:" + e.Message + "2 + a" + 2 + a + "---len" + len + "--tempbtye" + tempbtye.Length);
                                alldata = new byte[0];
                            }
                        try
                        {
                            byte[] bs = new byte[len];
                            Array.Copy(tempbtye, (4 + a), bs, 0, bs.Length);
                            if (tempbtye[0] == 0x99)
                                return null;

                            funobj?.Invoke(tempbtye[0], bs, this);
                            command command = new command();
                            command.comand = tempbtye[0];
                            command.data = bs;
                            if (command.comand == 0)
                            {


                            }
                            return command;
                        }
                        catch (Exception e)
                        {
                            //  ErrorMge?.Invoke(3, e.StackTrace + "unup122:" + e.Message);
                            alldata = new byte[0];
                        }

                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        return null;
                    }
                
            }
            catch (Exception e)
            {
              
                alldata = new byte[0];
                return null;
            }
            return null;
        }
        public bool Send(byte command, string text)
        {
            try
            {
               if (DT == DataType.bytes)
                {
                    return Send(command, System.Text.Encoding.UTF8.GetBytes(text));
                }
                else
                    return Send(System.Text.Encoding.UTF8.GetBytes(text));

            }
            catch (Exception ee)
            {
               
                Isline = false;
                return false;
            }
           
        }

        public bool Send(byte command, byte[] text)
        {
            bool bb = false;
            try
            {
                if (DT == DataType.bytes)
                {

                    byte[] sendb = text;
                    byte[] lens = ConvertToByteList(sendb.Length);
                    byte[] b = new byte[2 + 2 + lens.Length + sendb.Length];
                    b[0] = command;
                    b[1] = (byte)lens.Length;
                    lens.CopyTo(b, 2);
                    CRC.ConCRC(ref b, 2 + lens.Length);
                    sendb.CopyTo(b, 2 + 2 + lens.Length);
                    bb = Send(b);

                }
                else
                    return Send(text);

            }
            catch (Exception ee)
            {
                Isline = false;
                return false;

            }
            return bb;
        }
        public bool Send(byte[] b)
        {
            try
            {
                tcpc.Client.Send(b);
                timeout = DateTime.Now;
                return true;
            }
            catch
            {
                return false;
            }
        }
        public int ConvertToInt(byte[] list)
        {
            int ret = 0;
            int i = 0;
            foreach (byte item in list)
            {
                ret += (item << i);
                i += 8;
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
                value >>= 8;
            }
            byte[] bb = new byte[ret.Count];
            ret.CopyTo(bb);
            return bb;
        }

    }
}
