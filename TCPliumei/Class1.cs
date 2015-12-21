using DHNetSDK;
using DHPlaySDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TCPliumei
{
    public class TCPliuclient
    {
        int _tongdao = 0;
        DHPlay dp = new DHPlay();
        public TCPliuclient(string ip, int port, int tongdao, IntPtr Handle)
        {
            _tongdao = tongdao;
            receiveServerEvent += Tcp_receiveServerEvent; ;
            start(ip, port);
            DHPlay.PLAY_OpenStream(tongdao, IntPtr.Zero, 0, (UInt32)900 * 1024);
            //播放的部分
            bool b2 = dp.DHPlayControl(PLAY_COMMAND.OpenStream, tongdao, IntPtr.Zero, 0, 900 * 1024);
            dp.DHSetStreamOpenMode(tongdao, PLAY_MODE.STREAME_FILE);
            dp.DHPlayControl(0, tongdao, Handle);
        }

        private void Tcp_receiveServerEvent(byte[] data)
        {

            IntPtr pName = Marshal.AllocHGlobal((IntPtr)data.Length);
            Marshal.Copy(data, 0, pName, (int)data.Length);
            DHPlay.PLAY_InputData(_tongdao, pName, (uint)data.Length);

            Marshal.FreeHGlobal(pName);
        }

        TcpClient tcpc;
        public delegate void receive(byte[] data);
        public event receive receiveServerEvent;
        public delegate void istimeout();
        public event istimeout timeoutevent;
        public delegate void errormessage(int type, string error);
        public event errormessage ErrorMge;
        bool isok = false;
        bool isreceives = false;
        bool isline = false;
        DateTime timeout;
        int mytimeout = 30;
        public delegate void P2Preceive(byte command, String data, EndPoint ep);
        public event P2Preceive P2PreceiveEvent;

        bool NATUDP = false;
        String IP; int PORT;
        public bool Isline
        {
            get
            {
                return isline;
            }

            set
            {
                isline = value;
            }
        }

        public string Tokan
        {
            get
            {
                return tokan;
            }

            set
            {
                tokan = value;
            }
        }


        public bool start(string ip, int port, int _timeout)
        {
            mytimeout = _timeout;
            IP = ip;
            PORT = port;
            return start(ip, port);
        }
        public bool Restart()
        {
            return start(IP, PORT);
        }
        public bool start(string ip, int port)
        {
            try
            {
                IP = ip;
                PORT = port;
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                tcpc = new TcpClient();
                tcpc.ExclusiveAddressUse = false;
                tcpc.Connect(ip, port);
                Isline = true;
                isok = true;

                timeout = DateTime.Now;
                if (!isreceives)
                {
                    isreceives = true;
                    System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(receives));
                }
                return true;
            }
            catch (Exception e)
            {
                Isline = false;
                if (ErrorMge != null)
                    ErrorMge(1, e.Message);
                return false;
            }
        }

        void udp_receiveevent(byte command, string data, EndPoint iep)
        {
            if (P2PreceiveEvent != null)
                P2PreceiveEvent(command, data, iep);
        }


        private string tokan;
        public bool send(byte command, string text)
        {

            try
            {
                byte[] sendb = System.Text.Encoding.UTF8.GetBytes(text);
                byte[] lens = System.Text.Encoding.UTF8.GetBytes(sendb.Length.ToString());
                byte[] b = new byte[2 + lens.Length + sendb.Length];
                b[0] = command;
                b[1] = (byte)lens.Length;
                lens.CopyTo(b, 2);
                sendb.CopyTo(b, 2 + lens.Length);

                tcpc.Client.Send(b);
            }
            catch { return false; }
            // tcpc.Close();
            return true;
        }

        public void stop()
        {
            isok = false;
            Isline = false;
            tcpc.Close();
            dp.DHPlayControl(PLAY_COMMAND.CloseStream, _tongdao);
            DHClient.DHCleanup();
        }
        void thedata(object obj)
        {
            byte[] tempbtye = (byte[])obj;
            if (receiveServerEvent != null)
                receiveServerEvent(tempbtye);
        }
        void receives(object obj)
        {
            while (isok)
            {
                System.Threading.Thread.Sleep(50);
                try
                {
                    byte[] tempb = null;
                    labered:
                    int bytesRead = tcpc.Client.Available;

                    if (bytesRead > 0)
                    {


                        byte[] tempbtye = new byte[bytesRead];
                        tcpc.Client.Receive(tempbtye);
                        System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(thedata), tempbtye);



                        timeout = DateTime.Now;
                    }
                    else
                    {
                        TimeSpan ts = DateTime.Now - timeout;
                        if (ts.Seconds > mytimeout)
                        {
                            Isline = false;
                            // stop();
                            //  isreceives = false;
                            timeoutevent();

                            //   return;

                        }
                    }
                }
                catch (Exception e)
                {

                    if (ErrorMge != null)
                        ErrorMge(1, e.Message);

                }



            }
        }


    }
}
