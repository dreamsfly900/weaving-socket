using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Weave.Base;

namespace receivefile
{
    public class receive : WeaveTCPCommand
    {


        string basepath = System.Threading.Thread.GetDomain().BaseDirectory;
        string filepath = System.Configuration.ConfigurationSettings.AppSettings["filepath"];  // 路径
        string trsqfilepath = System.Configuration.ConfigurationSettings.AppSettings["trsq"];  // 路径
        List<filestate> listfilestate = new List<filestate>();

        public receive()
        {
            
        }

       
        public override byte Getcommand()
        {
            return 0x31;
        }
        [InstallFun("forever")]
        public void check(Socket soc, WeaveSession _0x01)
        {
            filestream fstream = _0x01.GetRoot<filestream>();
            var fstate = listfilestate.Where(f => f.Filename == fstream.Filename);
            foreach (filestate fst in fstate)
            {
                int sumlen = 0;
                foreach (byte b in fst.State)
                    sumlen += b;
                if (sumlen >= fst.State.Length)
                {
                    try
                    {
                        if (fstream.Filetype != "")
                        {

                            if (!Directory.Exists(@trsqfilepath + @"\" + fstream.Filetype))//如果不存在就创建file文件夹　　             　　              
                            {
                                Directory.CreateDirectory(@trsqfilepath + @"\" + fstream.Filetype);//创建该文件夹　　      

                                File.Move(@basepath + @"\" + fst.Path + @"\" + fst.Filename + ".tmp", @trsqfilepath + @"\" + fstream.Filetype + @"\" + fst.Filename);
                            }
                            else
                            {
                                File.Move(@basepath + @"\" + fst.Path + @"\" + fst.Filename + ".tmp", @trsqfilepath + @"\" + fstream.Filetype + @"\" + fst.Filename);

                            } 
                        }
                        else
                        {
                            File.Move(@basepath + @"\" + fst.Path + @"\" + fst.Filename + ".tmp", @filepath + @"\" + fst.Filename);
                        }
                        
                    }
                    catch(Exception ex) {
                        {
                            using (StreamWriter sw = new StreamWriter(@basepath + "\\log\\接收.txt"))
                            {
                                sw.WriteLine(ex.ToString());
                                sw.Close();
                            }

                        } }
                    try
                    {
                        File.Delete(@basepath + @"\" + fst.Path + @"\" + fst.Filename + ".tmp");
                    }
                    catch { }
                    listfilestate.Remove(fst);
                    if (!SendRoot<string>(soc, 0x31, "finish", fst.Filename, 0, "finish"))
                    {

                    }
                    return;
                }
                else if ((DateTime.Now - fst.Dt).TotalSeconds > 5)
                {
                    for (int i = 0; i < fst.State.Length; i++)
                        if (fst.State[i] == 0)
                        {
                            filestream st = new filestream();
                            st.Filename = fst.Filename;
                            st.Index = i;
                            if (!SendRoot<filestream>(soc, 0x31, "pushdata", st, 0, "pushdata"))
                            {
                                //listfilestate.Remove(fst);
                                //try
                                //{
                                //    File.Delete(@basepath + @"\" + fst.Path + @"\" + fst.Filename + ".tmp");
                                //}
                                //catch { }
                                return;
                            }
                        }
                    return;
                }
            }
            if (!SendRoot<string>(soc, 0x31, "finish", fstream.Filename, 0, "finish"))
            {

            }

        }
        [InstallFun("forever")]
        public void CreateFile(Socket soc, WeaveSession _0x01)
        {
            filestream fstream = _0x01.GetRoot<filestream>();

            if (!Directory.Exists(_0x01.Token))
                Directory.CreateDirectory(_0x01.Token);

            if (!File.Exists(_0x01.Token + "/" + fstream.Filename + ".tmp"))
            {
                // File.Create(_0x01.Token + "/" + fstream.Filename, (int)fstream.Maxlen).Close();
                lock (this)
                {
                    FileStream fss = File.Create(_0x01.Token + "/" + fstream.Filename+".tmp");

                    long offset = fss.Seek(fstream.Maxlen - 1, SeekOrigin.Begin);
                    fss.WriteByte(new byte());

                    fss.Close();
                    long count = fstream.Maxlen % (1024 * 4) == 0 ? fstream.Maxlen / (1024 * 4) : (fstream.Maxlen / (1024 * 4)) + 1;
                    filestate fs = new filestate();
                    fs.Filename = fstream.Filename;
                    fs.State = new byte[count];
                    fs.Path = _0x01.Token;
                    fs.Dt = DateTime.Now;
                    fs.Soc = soc;
                    listfilestate.Add(fs);
                }
            }

        }
        [InstallFun("forever")]
        public void SaveFile(Socket soc, WeaveSession _0x01)
        {
            filestream fstream = _0x01.GetRoot<filestream>();

           
            lb0330:
            if (File.Exists(_0x01.Token + "/" + fstream.Filename + ".tmp"))
            {
                System.IO.FileStream fi = System.IO.File.Open(_0x01.Token + "/" + fstream.Filename+".tmp", FileMode.Open, FileAccess.Write, FileShare.Write);
                fi.Seek(fstream.Index * 1024 * 4, SeekOrigin.Begin);
                fi.Write(fstream.Stream, 0, fstream.Len);
                fi.Close();
               var fstate=  listfilestate.Where(f => f.Filename == fstream.Filename) ;
                foreach (filestate fst in fstate)
                {
                    fst.Dt = DateTime.Now;
                    fst.State[fstream.Index] = 1;
                }
                
            }
            else { System.Threading.Thread.Sleep(100); goto lb0330; }

        }
        public override bool Run(string data, Socket soc)
        {
            return true;
        }

        public override void WeaveBaseErrorMessageEvent(Socket soc, WeaveSession _0x01, string message)
        {
            
        }

        public override void WeaveDeleteSocketEvent(Socket soc)
        {
            
        }

        public override void WeaveUpdateSocketEvent(Socket soc)
        {
            
        }
    }
    class filestate
    {
        Socket soc;
        byte[] state;
        String filename;
        string path;
        DateTime dt;
        public string Filename
        {
            get
            {
                return filename;
            }

            set
            {
                filename = value;
            }
        }

        public byte[] State
        {
            get
            {
                return state;
            }

            set
            {
                state = value;
            }
        }

        public DateTime Dt
        {
            get
            {
                return dt;
            }

            set
            {
                dt = value;
            }
        }

        public string Path
        {
            get
            {
                return path;
            }

            set
            {
                path = value;
            }
        }

        public Socket Soc
        {
            get
            {
                return soc;
            }

            set
            {
                soc = value;
            }
        }
    }
    class filestream
    {
        long maxlen;
        int index;
        int len;
        String filename;
        string filetype;

        public int Index
        {
            get
            {
                return index;
            }

            set
            {
                index = value;
            }
        }

        public int Len
        {
            get
            {
                return len;
            }

            set
            {
                len = value;
            }
        }

        public string Filename
        {
            get
            {
                return filename;
            }

            set
            {
                filename = value;
            }
        }

        public byte[] Stream
        {
            get
            {
                return stream;
            }

            set
            {
                stream = value;
            }
        }

        public long Maxlen
        {
            get
            {
                return maxlen;
            }

            set
            {
                maxlen = value;
            }
        }

        public string Filetype
        {
            get
            {
                return filetype;
            }

            set
            {
                filetype = value;
            }
        }

        byte[] stream;
    }
}
