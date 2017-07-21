using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace StandardModel
{
    /// <summary>
    /// 0x01基类
    /// </summary>
    /// 
    
    public class _baseModel
    {

        byte[] type;
        String number;
        /// <summary>
        /// 序列化当前传输类
        /// </summary>
        /// <returns></returns>
        public String Getjson()
        {
            String str = Newtonsoft.Json.JsonConvert.SerializeObject(this);
            return str;
        }
        /// <summary>
        /// 请求查询的表
        /// </summary>
        String request = "";

        public String Request
        {
            get { return request; }
            set { request = value; }
        }
        /// <summary>
        /// 服务器查询结果
        /// </summary>
        String root = "";

        public String Root
        {
            get { return root; }
            set { root = value; }
        }
        /// <summary>
        /// 赋值查询结果
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="t"></param>
        public void SetRoot<T>(T t)
        {
            String str = Newtonsoft.Json.JsonConvert.SerializeObject(t);
            Root = str;
        }

        /// <summary>
        /// 获取查询结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetRoot<T>()
        {
            T t = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(Root);
            return t;
        }

        /// <summary>
        /// 检索条件
        /// </summary>

        String parameter = "";

        public String Parameter
        {
            get { return parameter; }
            set { parameter = value; }
        }
        /// <summary>
        /// 授权验证
        /// </summary>
        String token = "";
        public String Token
        {
            get { return token; }
            set { token = value; }
        }
        /// <summary>
        /// 设置查询参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        public void SetParameter<T>(T t)
        {
           String str= Newtonsoft.Json.JsonConvert.SerializeObject(t);
            Parameter = str;
        }

        /// <summary>
        /// 获取查询参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetParameter<T>()
        { 
            T t = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(Parameter);
            return t;
        }
        /// <summary>
        ///查询总条数
        /// </summary>
        int querycount;
        public int Querycount
        {
            get { return querycount; }
            set { querycount = value; }
        }

        public string Number
        {
            get
            {
                return number;
            }

            set
            {
                number = value;
            }
        }

        public byte[] Type
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
    }


    public class MenuModel
    {
        String id = "";

        public String Id
        {
            get { return id; }
            set { id = value; }
        }
        string name = "";

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        List<MenuModel> nodes = new List<MenuModel>();

        public List<MenuModel> Nodes
        {
            get { return nodes; }
            set { nodes = value; }
        }

        public string Menu_ID { get; set; }
        public string Menu_Name { get; set; }
    }


}
