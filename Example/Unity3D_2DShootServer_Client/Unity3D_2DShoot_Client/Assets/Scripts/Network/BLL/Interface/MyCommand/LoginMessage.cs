using UnityEngine;
using System.Collections;
using System;
using WeaveBase;
public class LoginMessage : ICheckServerMessage
{
    public void CheckServerMessage( string text)
    {
       // throw new NotImplementedException();
       WeaveSession ws = Newtonsoft.Json.JsonConvert.DeserializeObject<WeaveSession>(text);
        Debug.Log("收到的消息是："+text);
        if(ws.Request == "ServerBackLoginResult" )
        {
            if( ws.GetRoot<bool>() == true)
            {
                //如果登陆成功，，处理的逻辑......
                //先更新用户

                //跳转场景
                MainClient.Instance.SetLoadSceneFlag();


                //发送读取上次分数数据的逻辑
                MainClient.Instance.SendCheckUserScore();
            }
           
            else
            {
                MainClient.Instance.serverBackLoginEvent.Invoke(false);
            }
        }
       

    }

}
