using MyInterface;
using P2P;
using StandardModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace cloud
{
    public class TCPcloud : Universal
    {
        p2psever p2psev;
        XmlDocument xml = new XmlDocument();
        List<CommandItem> listcomm = new List<CommandItem>();
        QueueTable qt = new QueueTable();
        public delegate void Mylog(string type,string log);
        public event Mylog EventMylog;
        List<online> onlines = new List<online>();
        public bool Run(MyInterface.MyInterface myI)
        {
            // Mycommand comm = new Mycommand(, connectionString);



            ReloadFlies();

            p2psev = new p2psever(myI.Parameter[3]);

            p2psev.receiveevent += p2psev_receiveevent;
            p2psev.EventUpdataConnSoc += p2psev_EventUpdataConnSoc;
            p2psev.EventDeleteConnSoc += p2psev_EventDeleteConnSoc;
         //   p2psev.NATthroughevent += tcp_NATthroughevent;//p2p事件，不需要使用
            p2psev.start(Convert.ToInt32(myI.Parameter[4]));
            qt.Add("onlinetoken", onlines);//初始化一个队列，记录在线人员的token
            if (EventMylog != null)
                EventMylog("连接","连接启动成功");
               return true; 
        }
       public void ReloadFlies()
        {
            try
            {
                listcomm = new List<CommandItem>();
                String[] strfilelist = System.IO.Directory.GetFiles(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "command");
                foreach (string file in strfilelist)
                {
                    Assembly ab = Assembly.LoadFile(file);
                    Type[] types = ab.GetExportedTypes();
                    foreach (Type t in types)
                    {

                        if (t.IsSubclassOf(typeof(TCPCommand)))
                        {
                            CommandItem ci = new CommandItem();
                            object obj = ab.CreateInstance(t.FullName);
                            TCPCommand Ic = obj as TCPCommand;
                            ci.MyICommand = Ic;
                            ci.CommName = Ic.Getcommand();
                            Ic.SetGlobalQueueTable(qt);
                            GetAttributeInfo( Ic,obj.GetType(), obj);
                            listcomm.Add(ci);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (EventMylog != null)
                    EventMylog("加载异常", ex.Message);
            }
        }
        public  void GetAttributeInfo(TCPCommand Ic,Type t,object obj)
        {

            foreach (MethodInfo mi in t.GetMethods())
            {

                InstallFun myattribute = (InstallFun)Attribute.GetCustomAttribute(mi, typeof(InstallFun));
                if (myattribute == null)
                {

                }
                else
                {

                   
                    Delegate del = Delegate.CreateDelegate(typeof(RequestData), obj, mi,true);
                    Ic.Bm.AddListen(mi.Name, del as RequestData, myattribute.Type);


                }
            }
        }
        void p2psev_EventDeleteConnSoc(System.Net.Sockets.Socket soc)
        {
            foreach (CommandItem CI in listcomm)
            {
                try
                {
                    CI.MyICommand.TCPCommand_EventDeleteConnSoc(soc);
                }
                catch (Exception ex){
                    if (EventMylog != null)
                        EventMylog("EventDeleteConnSoc", ex.Message);
                }
            }
        }

        void p2psev_EventUpdataConnSoc(System.Net.Sockets.Socket soc)
        {

            foreach (CommandItem CI in listcomm)
            {
                try
                {
                    CI.MyICommand.TCPCommand_EventUpdataConnSoc(soc);
                }
                catch (Exception ex){
                    if (EventMylog != null)
                        EventMylog("EventUpdataConnSoc", ex.Message);
                }
            }

        }
 
        
        void p2psev_receiveevent(byte command, string data, System.Net.Sockets.Socket soc)
        {
            try
            {
                if (command == 0xff)
                {
                    string[] temp = data.Split('|');
                    if (temp[0] == "in")
                    {
                        //加入onlinetoken
                        online ol = new online();
                        ol.Token = temp[1];
                        onlines.Add(ol);
                    }
                    else if (temp[0] == "out")
                    {
                        //移出onlinetoken
                        int count = onlines.Count;
                        online[] ols = new online[count];
                        onlines.CopyTo(0,ols,0,count);
                        foreach (online ol in ols)
                        {
                            if (ol.Token == temp[1])
                            {
                                onlines.Remove(ol);
                                return;
                            }
                        }
                    }
                    return;
                }
            }
            catch { return; }
            exec(command,data,soc);
            //System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(exec));
        }

        public void exec(byte command, string data, System.Net.Sockets.Socket soc)
        {
            foreach (CommandItem CI in listcomm)
            {
                if (CI.CommName == command)
                {
                    try
                    {
                        CI.MyICommand.Run(data, soc);
                        CI.MyICommand.Runbase(data, soc); 
                    }
                    catch (Exception ex)
                    {
                        if (EventMylog != null)
                            EventMylog("receiveevent", ex.Message);
                    }
                }
            }
        }
        public class CommandItem
        {
            byte commName;

            public byte CommName
            {
                get { return commName; }
                set { commName = value; }
            }


            TCPCommand _MyICommand;

            public TCPCommand MyICommand
            {
                get { return _MyICommand; }
                set { _MyICommand = value; }
            }

        }
    }


}
