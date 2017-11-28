using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using WeaveBase;
namespace WeaveSocketServer
{
  
    public  class HttpServer : IWeaveTcpBase
    {
        TcpListener listener;
        bool is_active = true;
       protected List<HttpProcessor> httpProcessorList = new List<HttpProcessor>();
        public event WaveReceiveEventEvent waveReceiveEvent;
        public event WeaveReceiveBitEvent weaveReceiveBitEvent;
      
        public event WeaveUpdateSocketListEvent weaveUpdateSocketListEvent;
        public event WeaveDeleteSocketListEvent weaveDeleteSocketListEvent;
        public int Port
        { get; set; }
        public HttpServer(int port)
        {
            Port = port;
        }
        public void Start(int port)
        {
            Port = port;
            Thread thread = new Thread(new ThreadStart(listen));
            thread.Start();
            Thread thread2 = new Thread(new ThreadStart(process));
            thread2.Start();
        }
        public int GetNetworkItemCount()
        {
            return httpProcessorList.Count;
        }
        public void KeepAliveHander(object obj)
        {
        }
        void process()
        {while (true)
            {
                int i = httpProcessorList.Count;
                HttpProcessor[] hps = new HttpProcessor[i];
                httpProcessorList.CopyTo(hps);
                foreach (HttpProcessor hp in hps)
                {
                    try
                    {
                        System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(hp.process));
                        httpProcessorList.Remove(hp);
                    }
                    catch { }
                }
                Thread.Sleep(1);
            }
        }
        public bool Send(Socket soc, byte command, string text)
        {
            int i = httpProcessorList.Count;
            HttpProcessor[] hps = new HttpProcessor[i];
            httpProcessorList.CopyTo(hps);
            foreach (HttpProcessor hp in hps)
            {
                if (hp.socket.Client == soc)
                {
                    hp.retrunData = text;
                    return true;
                }
            }
            return false;
        }
        public bool Send(Socket soc, byte command, byte[] data)
        {
            int i = httpProcessorList.Count;
            HttpProcessor[] hps = new HttpProcessor[i];
            httpProcessorList.CopyTo(hps);
            foreach (HttpProcessor hp in hps)
            {
                if (hp.socket.Client == soc)
                {
                    hp.retrunData = Convert.ToBase64String(data);
                    return true;
                }
            }
            return false;
        }
        void listen()
        {
            listener = new TcpListener( IPAddress.Any, Port);
            listener.Start();
            while (is_active)
            {
                try
                {
                    TcpClient s = listener.AcceptTcpClient();
                    HttpProcessor processor = new HttpProcessor(s, this);
                    httpProcessorList.Add(processor);
                    
                    Thread.Sleep(1);
                }
                catch
                { }
            }
        }
        public virtual void handleGETRequest(HttpProcessor p)
        {
            p.http_url = p.http_url.Substring(1);
           // string fun = p.http_url.Split('?')[1].Split('=')[0];
            if (p.http_url == "")
                return;
            string fun = p.http_url.Split('?')[1].Split('=')[0];
            byte command = Convert.ToByte(p.http_url.Substring(0, 1), 16);
            string data = p.http_url.Split('&')[1];
            p.writeSuccess();
            p.outputStream.WriteLine(fun + "(");
            getdata(p, command, data);
            p.outputStream.WriteLine(")");
            
        }
        public virtual void handlePOSTRequest(HttpProcessor p, StreamReader inputData)
        {
            p.http_url = p.http_url.Substring(1);
            if (p.http_url == "")
                return;
            byte command = Convert.ToByte(p.http_url, 16);
            string data = inputData.ReadToEnd();
            p.writeSuccess();
            getdata(p, command, data);
          
        }
        public virtual bool getdata(HttpProcessor p, byte command, string data)
        {
                      waveReceiveEvent?.Invoke(command, data, p.socket.Client);
                      weaveReceiveBitEvent?.Invoke(command,  Convert.FromBase64String(data), p.socket.Client);
                        int count = 0;
                        while (p.retrunData== "")
                        {
                            System.Threading.Thread.Sleep(200);
                            if (count > 450)
                            {
                                p.outputStream.WriteLine("响应超时");
                                return false;
                            }
                            count++;
                        }
                        p.outputStream.WriteLine(p.retrunData);
               p.retrunData = "";
           
            return true;
        }
    }
        //public class TestMain
        //{
        //    public static int Main(String[] args)
        //    {
        //        HttpServer httpServer;
        //        if (args.GetLength(0) > 0)
        //        {
        //            httpServer = new MyHttpServer(Convert.ToInt16(args[0]));
        //        }
        //        else
        //        {
        //            httpServer = new MyHttpServer(8080);
        //        }
        //        Thread thread = new Thread(new ThreadStart(httpServer.listen));
        //        thread.Start();
        //        return 0;
        //    }
        //}
}
