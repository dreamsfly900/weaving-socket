using System;
namespace Weave.Base
{
    /// <summary>
    /// 0x01基类
    /// </summary>
    /// 
    public class WeaveSession
    {
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
        public string Request
        {
            get { return request; }
            set { request = value; }
        }
        /// <summary>
        /// 服务器查询结果
        /// </summary>
        public string Root
        {
            get;set;
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
        public String Parameter
        {
            get; set;
        }
        /// <summary>
        /// 授权验证
        /// </summary>
        public string Token
        {
            get; set;
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
        public int Querycount
        {
            get; set;
        }
        public string Number
        {
            get;set;
        }
        public byte[] Type
        {
            get;set;
        }
    }
}
