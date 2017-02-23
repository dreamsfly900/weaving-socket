using Newtonsoft.Json;
using StandardModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace MyInterface
{
     class table
    {
        string key;

        public string Key
        {
            get
            {
                return key;
            }

            set
            {
                key = value;
            }
        }

        public object Value
        {
            get
            {
                return value;
            }

            set
            {
                this.value = value;
            }
        }

        object value;
       
        public object Clone()
        {
            return Copy(value);
        }

        private object Copy(object obj)
        {

            Object targetDeepCopyObj;
            Type targetType = obj.GetType();
            //值类型  
            if (targetType.IsValueType == true)
            {
                targetDeepCopyObj = obj;
            }
            //引用类型   
            else
            {
                targetDeepCopyObj = System.Activator.CreateInstance(targetType);   //创建引用对象   
                System.Reflection.MemberInfo[] memberCollection = obj.GetType().GetMembers();

                foreach (System.Reflection.MemberInfo member in memberCollection)
                {
                    if (member.MemberType == System.Reflection.MemberTypes.Field)
                    {
                        System.Reflection.FieldInfo field = (System.Reflection.FieldInfo)member;
                        Object fieldValue = field.GetValue(obj);
                        if (fieldValue is ICloneable)
                        {
                            field.SetValue(targetDeepCopyObj, (fieldValue as ICloneable).Clone());
                        }
                        else
                        {
                            field.SetValue(targetDeepCopyObj, Copy(fieldValue));
                            //field.SetValue(targetDeepCopyObj, (fieldValue));
                        }

                    }
                    else if (member.MemberType == System.Reflection.MemberTypes.Property)
                    {
                        System.Reflection.PropertyInfo myProperty = (System.Reflection.PropertyInfo)member;
                        MethodInfo info = myProperty.GetSetMethod(false);
                        if (info != null)
                        {
                            object propertyValue = myProperty.GetValue(obj, null);
                            if (propertyValue is ICloneable)
                            {
                                myProperty.SetValue(targetDeepCopyObj, (propertyValue as ICloneable).Clone(), null);
                            }
                            else
                            {
                                myProperty.SetValue(targetDeepCopyObj, Copy(propertyValue), null);
                            }
                        }

                    }
                }
            }
            return targetDeepCopyObj;
        }

    }
    public class QueueTable
    {
      
        private List<table> temps = new List<table>();
        // To enable client code to validate input  
        // when accessing your indexer. 

        public bool Add(String key, object value)
        {
            try
            {
                table t = new table();
                t.Key = key;
                t.Value = value;
                temps.Add(t);

                return true;
            }
            catch { return false; }

        }
        public bool Remove(String key)
        {
            table temp = temps.Find(x => x.Key == key);
            temps.Remove(temp);
            return true;
        }
        public int Length
        {
            get { return temps.Count; }
        }
        public object this[int index]
        {
            get
            {
                return temps[index].Value;
            }

            set
            {
               
                temps[index].Value = value;
            }
        }

        public object GetValueClonebyIndex(int index)
        {
            return temps[index];
        }
        public object GetValueClonebyKey(string key)
        {
            return temps.Find(x => x.Key == key);
        }
        public object this[string key]
        {
            get
            {
                return temps.Find(x => x.Key == key).Value;
            }

            set
            {
                temps.Find(x => x.Key == key).Value = value;
            }
        }
    }
    public class InstallFun : System.Attribute
    {
        private string type;
        bool dtu = false;
        /// <summary>
        /// 标识这个方法是执行一次即卸载，还是长期执行
        /// </summary>
        /// <param name="type">forever,或noce</param>
        public InstallFun(string type)
        {
            Type = type;
        }
        public InstallFun(string type,bool _Dtu)
        {
            Type = type;
            Dtu = _Dtu;
        }
        public string Type
        {
            get
            {
                return type;
            }

            set
            {
                type = value;
            }
        }

        public bool Dtu
        {
            get
            {
                return dtu;
            }

            set
            {
                dtu = value;
            }
        }
    }
    public class MyInterface
    {
        String[] parameter;

        public String[] Parameter
        {
            get { return parameter; }
            set { parameter = value; }
        }
    }
    public interface Universal
    {
         bool Run(MyInterface myI);
    }
    public class online
    {
        string token;

        public string Token
        {
            get
            {
                return token;
            }

            set
            {
                token = value;
            }
        }
        [JsonIgnore]
        object obj;
        string name;
        [JsonIgnore]
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

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }
        [JsonIgnore]
        public object Obj
        {
            get
            {
                return obj;
            }

            set
            {
                obj = value;
            }
        }

        System.Net.Sockets.Socket soc;
    }
    public abstract class TCPCommand
    {
        _base_manage bm = new _base_manage();
        private QueueTable globalQueueTable;
        public TCPCommand()
        {
            Bm.errorMessageEvent += Bm_errorMessageEvent; ;
        }
        public bool Runbase(String data, System.Net.Sockets.Socket soc)
        {
           
            Bm.init(data, soc);
            return true;
        }
        public abstract byte Getcommand();
        public abstract bool Run(String data, System.Net.Sockets.Socket soc);
        public abstract void TCPCommand_EventUpdataConnSoc(Socket soc);
        public abstract void TCPCommand_EventDeleteConnSoc(Socket soc);
        public abstract void Bm_errorMessageEvent(Socket soc, _baseModel _0x01, string message);

        public QueueTable GlobalQueueTable
        {
            get
            {
                return globalQueueTable;
            }

        }

        public _base_manage Bm
        {
            get
            {
                return bm;
            }

            set
            {
                bm = value;
            }
        }

        public void SetGlobalQueueTable(QueueTable qt)
        {

            globalQueueTable = qt;
        }
        public online[] GetOnline()
        {
            online[] ols = new online[0];
            try
            {
                List<online> ol = GlobalQueueTable["onlinetoken"] as List<online>;
                int i = ol.Count;
                ols = new online[i];
                ol.CopyTo(0, ols, 0, i);

            }
            catch { }
            return ols;
        }
        /// <summary>
        /// 根据TOKEN 获取online对象
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public online GetonlineByToken(String token)
        {
            online[] ols = GetOnline();
            foreach (online o in ols)
            {
                if (o.Token == token)
                {
                    return o;
                }
            }
            return null;
        }
        /// <summary>
        /// 根据TOKEN 设置 name与OBJ属性
        /// </summary>
        /// <param name="token"></param>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        public void SetonlineByToken(String token,string name,object obj)
        {
            online[] ols = GetOnline();
            foreach (online o in ols)
            {
                if (o.Token == token)
                {
                    o.Name = name;
                    o.Obj = obj;
                }
            }
        }
        /// <summary>
        /// 根据TOKEN 设置 OBJ属性
        /// </summary>
        /// <param name="token"></param>
        /// <param name="obj"></param>
        public void SetonlineByToken(String token, object obj)
        {
            online[] ols = GetOnline();
            foreach (online o in ols)
            {
                if (o.Token == token)
                {
                    o.Obj = obj;
                }
            }
        }
        /// <summary>
        /// 新增online
        /// </summary>
        /// <param name="token"></param>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        public void Addonline(online o)
        {
            List<online> ol = GlobalQueueTable["onlinetoken"] as List<online>;
            ol.Add(o);
        }
        public bool SendParameter<T>(Socket soc, byte command, String Request, T Parameter, int Querycount,String Tokan)
        {
            _baseModel b = new _baseModel();
            b.Request = Request;
            b.Token = Tokan;
            b.SetParameter<T>(Parameter);
            b.Querycount = Querycount;

            return send(soc, command, b.Getjson());
        }
        public bool SendRoot<T>(Socket soc, byte command, String Request, T Root, int Querycount, String Tokan)
        {
            _baseModel b = new _baseModel();
            b.Request = Request;
            b.Token = Tokan;
            b.SetRoot<T>(Root);
            b.Querycount = Querycount;

            return send(soc, command, b.Getjson());
        }
        public bool SendDtu(Socket soc, byte[] Root, String ip,int port)
        {
            _baseModel b = new _baseModel();
            b.Request = "dtu";
            b.Token = ip + "|" + port;
            b.SetRoot<byte[]>(Root);
            b.Querycount = 0;

            return send(soc, 0x00, b.Getjson());
        }
        public bool send(Socket soc, byte command, string text)
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
              soc.Send(b); 
          } 
          catch { return false; }
          // tcpc.Close();
          return true;
      }
        /// <summary>
        /// 这个方法会被重写
        /// </summary>
        /// <param name="command"></param>
        /// <param name="data"></param>
        /// <param name="soc"></param>
        public virtual void Runcommand(byte command, string data, Socket soc)
        {
            
        }
        public virtual void Tokenout(online ol)
        {

        }
        public virtual void Tokenin(online ol)
        {

        }
    }
    public class MySockets
    {
        Socket _sck;

        public Socket Sck
        {
            get { return _sck; }
            set { _sck = value; }
        }
        //string context;

        //public string Context
        //{
        //    get { return context; }
        //    set { context = value; }
        //}
        scheduling sch = new scheduling();

        public scheduling Sch
        {
            get { return sch; }
            set { sch = value; }
        }
    }
    public class scheduling
    {
        string from = "";

        public string From
        {
            get { return from; }
            set { from = value; }
        }
        string sgin = "";

        public string Sgin
        {
            get { return sgin; }
            set { sgin = value; }
        }
        string to = "";

        public string To
        {
            get { return to; }
            set { to = value; }
        }
        string type = "";

        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        string lat = "";

        public string Lat
        {
            get { return lat; }
            set { lat = value; }
        }
        string lng = "";

        public string Lng
        {
            get { return lng; }
            set { lng = value; }
        }
        String phone = "";

        public String Phone
        {
            get { return phone; }
            set { phone = value; }
        }
        String context = "";

        public String Context
        {
            get { return context; }
            set { context = value; }
        }
        String dt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        public String Dt
        {
            get { return dt; }
            set { dt = value; }
        }
        String dts = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        public String Dts
        {
            get { return dts; }
            set { dts = value; }
        }
        int err = 0;

        public int Err
        {
            get { return err; }
            set { err = value; }
        }
        bool islock = false;

        public bool Islock
        {
            get { return islock; }
            set { islock = value; }
        }

    }
    public interface IMyCommand
    {
        void runcommand<T>(T DataSer, MySockets mysoc);

    }
    
}
