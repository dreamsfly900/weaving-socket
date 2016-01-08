using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace P2P
{
   public class UDP
    {
       Socket listener;
       public delegate void myreceive(byte command, String data, NETcollectionUdp iep);
       public event myreceive receiveevent;
       List<NETcollectionUdp> listconn = new List<NETcollectionUdp>();
       public void start(int port)
       {
           try
           {
               listener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
               listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
               IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
               listener.Bind(localEndPoint);
              // listener.Listen(10000);
               System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(receive));
               System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(xintiao));
              // System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(receives));
           }
           catch (Exception e) { throw e; }
       }

       //private void receives(object state)
       //{
       //    while (true)
       //    {
       //        try
       //        {
       //            NETcollectionUdp[] netclist = getUdpList();
       //            foreach (NETcollectionUdp tempnetc in netclist)
       //            {
       //                IPEndPoint anyEndPoint = new IPEndPoint(IPAddress.Any, 0);
       //                EndPoint remoteEndPoint = anyEndPoint;
       //                if (tempnetc.Soc != null)
       //                {
       //                    byte[] data;
       //                    if (tempnetc.Soc.Available > 1)
       //                    {
       //                        data = new byte[tempnetc.Soc.Available];
       //                        int recv = tempnetc.Soc.Receive(data, 0, data.Length, SocketFlags.None);
       //                        tempnetc.Soc.SendTo(data, tempnetc.Iep);
       //                    }
       //                }
       //            }
       //            System.Threading.Thread.Sleep(100);
       //        }
       //        catch (Exception e)
       //        {
       //            System.IO.StreamWriter sw = new System.IO.StreamWriter("send.txt", true);
       //            sw.WriteLine(e.Message);
       //            sw.Close();
       //        }
       //    }
       //}

       //public bool mapping(int port,NETcollectionUdp  netc)
       //{
       //    try
       //       {   
       //            NETcollectionUdp[] netclist = getUdpList();
       //            foreach (NETcollectionUdp tempnetc in netclist)
       //            {
       //                if (tempnetc.Port == port)
       //                {
       //                    return false;
       //                }
       //            }
       //        Socket listeners = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
       //        listeners.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
       //        IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, port);
       //        listeners.Blocking = false;
       //        listeners.Bind(localEndPoint);
       //        netc.Soc= listeners;
       //        return true;
       //    }
       //    catch  { return false; }
            
       //}
       int minport = 33333;
       int max = 99999;
       private string loaclip;

       public UDP(string _loaclip)
       {
           // TODO: Complete member initialization
           this.loaclip = _loaclip;
       }
       void receiveeventto(object obj)
       {
           modelevent me = (modelevent)obj;
           int command = me.Command;
           if (command == 0x9c)
           {
           //    if (minport > max)
           //        minport = 33333;
           //lable999:
           //    int temp = minport++;
           //    if (mapping(temp, me.Iep))
           //    {
           //        me.Iep.Port = temp;
           //        me.Iep.Localiep = new IPEndPoint(IPAddress.Parse(loaclip), temp);
                   send(0x98, me.Iep.Iep.ToString()+ me.Iep.Iep.Port+"---"+ me.Iep.Iep.Address.ToString(), (IPEndPoint)me.Iep.Iep);
           //    }
           //    else goto lable999;
           }
         
               if (receiveevent != null)
                   receiveevent(me.Command, me.Data, me.Iep);
        
           
       }
       public NETcollectionUdp[] getUdpList()
       {
           try
           {
               NETcollectionUdp[] netlist = new NETcollectionUdp[listconn.Count];
               listconn.CopyTo(netlist);
               return netlist;
           }
           catch { return null; }
       }
       class modelevent
       {
           byte command;

           public byte Command
           {
               get { return command; }
               set { command = value; }
           }
           string data;

           public string Data
           {
               get { return data; }
               set { data = value; }
           }
           NETcollectionUdp iep;

           public NETcollectionUdp Iep
           {
               get { return iep; }
               set { iep = value; }
           }
       }
       public void xintiao(object obj)
       {
           while (true)
           {
               try
               {
                   System.Threading.Thread.Sleep(2000);
                    //  ArrayList al = new ArrayList();
                    // al.Clone()
                    NETcollectionUdp[] netclist = getUdpList();
                    
                    NETcollectionUdp[] netlist = getUdpList();
                   foreach(NETcollectionUdp nudp in netlist)
                   {
                        IPEndPoint anyEndPoint = new IPEndPoint(IPAddress.Any, 0);
                        EndPoint remoteEndPoint = anyEndPoint;
                        send(0x99, "", remoteEndPoint);
                        TimeSpan ts = DateTime.Now - nudp.Timeout;
                       if (ts.Seconds > 30)
                       {
                           nudp.Soc.Close();
                           listconn.Remove(nudp);
                       }
                   }
               }
               catch { }
           }
       }
       public bool send(byte command, string text, EndPoint ep)
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

               listener.SendTo(b, ep);
           }
           catch (Exception e){
               throw e;
               //return false; 
           }
           // tcpc.Close();
           return true;
       }
       void receive(object ias)
       {
           while (true)
           {
               try
               {
                   IPEndPoint anyEndPoint = new IPEndPoint(IPAddress.Any, 0);
                   EndPoint remoteEndPoint = anyEndPoint;
                  
                  byte[] data = new byte[1024];
                   int recv = listener.ReceiveFrom(data, 0, data.Length, SocketFlags.None, ref remoteEndPoint);
                  int bytesRead = data.Length;
                   byte[] tempbtye = new byte[data.Length];
                   data.CopyTo(tempbtye, 0);
                   if (tempbtye[0] == 0x99)
                   {
                       NETcollectionUdp nudpa = new NETcollectionUdp();
                       nudpa.Timeout = DateTime.Now;
                       nudpa.Iep = (IPEndPoint)remoteEndPoint;
                       System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(udpup), nudpa);
                       continue;
                   }
                   NETcollectionUdp nudp = new NETcollectionUdp();
                   nudp.Timeout = DateTime.Now;
                   nudp.Iep = (IPEndPoint)remoteEndPoint;
                   System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(udpadd), nudp);
               labe881:
                  
                   int a = tempbtye[1];
               if (a == 0)
                   continue;
                   String temp = System.Text.Encoding.UTF8.GetString(tempbtye, 2, a);
                   int len = int.Parse(temp);
                   //int b = netc.Buffer[2 + a+1];
                   //temp = System.Text.ASCIIEncoding.ASCII.GetString(netc.Buffer, 2 + a + 1, b);
                   //len = int.Parse(temp);
                   temp = System.Text.Encoding.UTF8.GetString(tempbtye, 2 + a, len);
                   modelevent me = new modelevent();
                   me.Command = tempbtye[0];
                   me.Data = temp;
                   me.Iep = nudp;
                   System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(receiveeventto), me);
                 
                   if (bytesRead > (2 + a + len))
                   {
                       byte[] b = new byte[bytesRead - (2 + a + len)];
                       byte[] t = tempbtye;
                       Array.Copy(t, (2 + a + len), b, 0, b.Length);

                       tempbtye = b;
                       bytesRead = bytesRead - (2 + a + len);
                       goto labe881;
                   }
               }
               catch { }
               System.Threading.Thread.Sleep(100);
           }
       }

       private void udpup(object state)
       {
           NETcollectionUdp nudp = (NETcollectionUdp)state;
           try
           {

            
               foreach (NETcollectionUdp temnudp in listconn)
               {
                   if (temnudp != null)
                       if (temnudp.Iep.ToString() == nudp.Iep.ToString())
                           temnudp.Timeout = DateTime.Now;
               }
              
           }
           catch { }
       }

       private void udpadd(object state)
       {
           NETcollectionUdp nudp = (NETcollectionUdp)state;
           try
           {

               NETcollectionUdp[] netlist = getUdpList();
               foreach (NETcollectionUdp temnudp in netlist)
               {
                   if (temnudp!=null)
                   if (temnudp.Iep == nudp.Iep)
                       return;
               }
               listconn.Add(nudp);
           }
           catch { }
          
       }
    }
}
