using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using WeaveBase;
namespace SocketServer
{
    public class HttpProcessor
    {
            public TcpClient socket;
            public HttpServer srv;
            private Stream inputStream;
            public StreamWriter outputStream;
            public string http_method;
            public string http_url;
            public string http_protocol_versionstring;
            public Hashtable httpHeaders = new Hashtable();
        public String retrunData = "";
            private static int MAX_POST_SIZE = 10 * 1024 * 1024; // 10MB
            public HttpProcessor(TcpClient s, HttpServer srv)
            {
                this.socket = s;
                this.srv = srv;
            }
            private string streamReadLine(Stream inputStream)
            {
                int next_char;
                string data = "";
                while (true)
                {
                    next_char = inputStream.ReadByte();
                    if (next_char == '\n') { break; }
                    if (next_char == '\r') { continue; }
                    if (next_char == -1) { Thread.Sleep(1); continue; };
                    data += Convert.ToChar(next_char);
                }
                return data;
            }
            public void process(object p)
            {
                // we can't use a StreamReader for input, because it buffers up extra data on us inside it's
                // "processed" view of the world, and we want the data raw after the headers
                inputStream = new BufferedStream(socket.GetStream());
                // we probably shouldn't be using a streamwriter for all output from handlers either
                outputStream = new StreamWriter(new BufferedStream(socket.GetStream()));
                try
                {
                    parseRequest();
                    readHeaders();
                    if (http_method.Equals("GET"))
                    {
                        handleGETRequest();
                    }
                    else if (http_method.Equals("POST"))
                    {
                        handlePOSTRequest();
                    }
                }
                catch (Exception e)
                {
                    //Console.WriteLine("Exception: " + e.ToString());
                    writeFailure();
                }
                outputStream.Flush();
                // bs.Flush(); // flush any remaining output
                inputStream = null; outputStream = null; // bs = null;            
                socket.Close();
            }
            public void parseRequest()
            {
                String request = streamReadLine(inputStream);
                string[] tokens = request.Split(' ');
                if (tokens.Length != 3)
                {
                // throw new Exception("invalid http request line");
                return;
                }
                http_method = tokens[0].ToUpper();
                http_url = tokens[1];
                http_protocol_versionstring = tokens[2];
                //Console.WriteLine("starting: " + request);
            }
            public void readHeaders()
            {
                //Console.WriteLine("readHeaders()");
                String line;
                while ((line = streamReadLine(inputStream)) != null)
                {
                    if (line.Equals(""))
                    {
                        //Console.WriteLine("got headers");
                        return;
                    }
                    int separator = line.IndexOf(':');
                    if (separator == -1)
                    {
                        throw new Exception("invalid http header line: " + line);
                    }
                    String name = line.Substring(0, separator);
                    int pos = separator + 1;
                    while ((pos < line.Length) && (line[pos] == ' '))
                    {
                        pos++; // strip any spaces
                    }
                    string value = line.Substring(pos, line.Length - pos);
                    //Console.WriteLine("header: {0}:{1}", name, value);
                    httpHeaders[name] = value;
                }
                     httpHeaders.Add("Access-Control-Allow-Origin", "*") ;
        }
            public void handleGETRequest()
            {
                srv.handleGETRequest(this);
            }
            private const int BUF_SIZE = 4096;
        public void handlePOSTRequest()
            {
                // this post data processing just reads everything into a memory stream.
                // this is fine for smallish things, but for large stuff we should really
                // hand an input stream to the request processor. However, the input stream 
                // we hand him needs to let him see the "end of the stream" at this content 
                // length, because otherwise he won't know when he's seen it all! 
                ////Console.WriteLine("get post data start");
                int content_len = 0;
                MemoryStream ms = new MemoryStream();
                if (this.httpHeaders.ContainsKey("Content-Length"))
                {
                    content_len = Convert.ToInt32(this.httpHeaders["Content-Length"]);
                    if (content_len > MAX_POST_SIZE)
                    {
                        throw new Exception(
                            String.Format("POST Content-Length({0}) too big for this simple server",
                              content_len));
                    }
                    byte[] buf = new byte[BUF_SIZE];
                    int to_read = content_len;
                    while (to_read > 0)
                    {
                        //Console.WriteLine("starting Read, to_read={0}", to_read);
                        int numread = this.inputStream.Read(buf, 0, Math.Min(BUF_SIZE, to_read));
                        //Console.WriteLine("read finished, numread={0}", numread);
                        if (numread == 0)
                        {
                            if (to_read == 0)
                            {
                                break;
                            }
                            else
                            {
                                throw new Exception("client disconnected during post");
                            }
                        }
                        to_read -= numread;
                        ms.Write(buf, 0, numread);
                    }
                    ms.Seek(0, SeekOrigin.Begin);
                }
                //Console.WriteLine("get post data end");
                srv.handlePOSTRequest(this, new StreamReader(ms));
            }
            public void writeSuccess()
            {
                outputStream.WriteLine("HTTP/1.0 200 OK");
                outputStream.WriteLine("Content-Type: text/html");
                outputStream.WriteLine("Access-Control-Allow-Origin:*");
                outputStream.WriteLine("Connection: close");
                outputStream.WriteLine("");
            }
            public void writeFailure()
            {
                outputStream.WriteLine("HTTP/1.0 404 File not found");
                outputStream.WriteLine("Connection: close");
                outputStream.WriteLine("");
            }
    }
    public  class HttpServer : IWeaveTcpBase
    {
        TcpListener listener;
        bool is_active = true;
       protected List<HttpProcessor> httpProcessorList = new List<HttpProcessor>();
        public event WaveReceiveEventEvent waveReceiveEvent;
        public event WeaveReceiveBitEvent weaveReceiveBitEvent;
        public event NATthrough NATthroughevent;
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
            listener = new TcpListener(Port);
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
            httpProcessorList.Remove(p);
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
            httpProcessorList.Remove(p);
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
