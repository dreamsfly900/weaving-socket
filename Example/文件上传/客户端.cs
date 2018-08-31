using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Weave.Base;

namespace fileupdata
{

    public class Class1
    {
        public delegate void insetrtext(string text);
        public event insetrtext addtext;
        public Weave.TCPClient.P2Pclient pclient = new Weave.TCPClient.P2Pclient(false);

        // int minb = 0;
        //  int index = 1;


        public Class1()
        {
            //文件发送
            //pclient = new Weave.TCPClient.P2Pclient(false);


        }


        public void stat(csdata cs)
        {
            string path = System.AppDomain.CurrentDomain.BaseDirectory;

            pclient = new Weave.TCPClient.P2Pclient(false);
            pclient.receiveServerEvent += Pclient_receiveServerEvent;
            pclient.timeoutevent += Pclient_timeoutevent;
            pclient.AddListenClass(this);
            bool b = pclient.start("116.255.241.138", 8989, false);
            // index = 1;
            // minb = filename.Count;

            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(filesend));
            t.Start(cs);

        }
        bool gg = true;
        void check(object obj)
        {
            System.Threading.Thread.Sleep(10000);
            while (gg)
            {
                csdata cs = obj as csdata;
                filestream fs = new filestream();
                fs.Filename = cs.filename;
                fs.Index = 0;
                fs.Len = 0;
                fs.Stream = null;
                fs.Maxlen = 0;
                fs.Filetype = cs.filetype;                
                pclient.Tokan = "check";
                pclient.SendRoot<filestream>(0x31, "check", fs, 0);
                System.Threading.Thread.Sleep(2000);
            }
        }

        private void Pclient_timeoutevent()
        {
            lb11223:
            if (pclient.Restart(false))
            {
                goto lb11223;
            }
        }

        private void Pclient_receiveServerEvent(byte command, string text)
        {

        }
        [InstallFun("forever")]
        public void pushdata(Socket soc, WeaveSession _0x01)
        {


            filestream fstre = _0x01.GetRoot<filestream>();
            byte[] byteData = new byte[1024 * 4];
            //从文件中读取数据写入到数组中(数组对象，从第几个开始读，读多少个)
            //返回读取的文件内容真实字节数
            FileStream fstream = File.OpenRead(fstre.Filename);
            fstream.Seek(fstre.Index * 1024 * 4, SeekOrigin.Begin);
            int length = fstream.Read(byteData, (int)0, byteData.Length);
            //如果字节数大于0，则转码
            if (length > 0)
            {
                filestream fs = new filestream();
                fs.Filename = fstre.Filename;
                fs.Index = fstre.Index;
                fs.Len = length;
                fs.Stream = byteData;
                fs.Filetype = "";
                pclient.Tokan = "fileupdata_gedian";
                pclient.SendRoot<filestream>(0x31, "SaveFile", fs, 0);
                System.Threading.Thread.Sleep(2);
            }

        }
        [InstallFun("forever")]
        public void finish(Socket soc, WeaveSession _0x01)
        {
            string filename = _0x01.GetRoot<string>();
            try
            {
                System.Threading.Thread.Sleep(1000);
                File.Delete(@filename);
                if (addtext != null)
                    addtext(filename + "已上传！-----------");
                gg = false;
                pclient.stop();
                //index++;
            }
            catch (Exception ex)
            {
                addtext(filename + ".错误信息\n" + ex.ToString());
               
            }
             
        }



        /// <summary>
        /// 格点预报文件发送
        /// </summary>
        /// <param name="obj"></param>
        public void filesend(object obj)
        {
            string fas = "";

            csdata cs = obj as csdata;

            try
            {
                string filelist = cs.filename;
                string type = cs.filetype;
              
                fas = filelist;
                string filename = filelist;
                String name = filename;
                FileStream fstream = File.OpenRead(filename);
                long count = fstream.Length % (1024 * 4) == 0 ? fstream.Length / (1024 * 4) : (fstream.Length / (1024 * 4)) + 1;
                int i = 0;
                fstream.Position = 0;
                filestream fs = new filestream();
                fs.Filename = name;
                fs.Index = 0;
                fs.Len = 0;
                fs.Stream = null;
                fs.Maxlen = fstream.Length;
                fs.Filetype = "";
                pclient.Tokan = "fileupdata_gedian";
                pclient.SendRoot<filestream>(0x31, "CreateFile", fs, 0);
                System.Threading.Thread.Sleep(1000);
                if (addtext != null)
                    addtext("马上准备上传！");
                // listBox1.Invoke(new EventHandler(delegate { listBox1.Items.Insert(0, "马上准备上传！"); }));
                //发送一段标记，说明文件要传输了。
                while (i < count)
                {
                    //创建一个容量4K的数组
                    byte[] byteData = new byte[1024 * 4];
                    //从文件中读取数据写入到数组中(数组对象，从第几个开始读，读多少个)
                    //返回读取的文件内容真实字节数
                    int length = fstream.Read(byteData, (int)0, byteData.Length);
                    //如果字节数大于0，则转码
                    if (length > 0)
                    {
                        fs = new filestream();
                        fs.Filename = name;
                        fs.Index = i;
                        fs.Len = length;
                        fs.Stream = byteData;
                        fs.Filetype = "";
                        pclient.Tokan = "fileupdata_gedian";
                        pclient.SendRoot<filestream>(0x31, "SaveFile", fs, 0);
                        System.Threading.Thread.Sleep(20);
                    }
                    else { fstream.Close(); return; }

                    if (addtext != null)
                        addtext((Double)(((Double)i / (Double)count) * 100) + "%");

                    System.Threading.Thread.Sleep(50);
                    i++;
                }
                fstream.Close();
                if (addtext != null)
                    addtext("上传完成");
                
                System.Threading.Thread t1 = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(check));
                t1.Start(cs);




            }
            catch (Exception ex)
            {
                using (StreamWriter sa = new StreamWriter(System.AppDomain.CurrentDomain.BaseDirectory + DateTime.Now.ToString("yyyymmddhhmmss") + "cuoeu.txt"))
                {
                    sa.WriteLine(ex.ToString());
                    addtext(fas + "网络波动，暂停上传，请稍等！");
                    sa.Close();
                }
            }


        }


    }



    public class csdata
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public string filename { get; set; }
        /// <summary>
        /// 文件类型
        /// </summary>
        public string filetype { get; set; }
    }
}
