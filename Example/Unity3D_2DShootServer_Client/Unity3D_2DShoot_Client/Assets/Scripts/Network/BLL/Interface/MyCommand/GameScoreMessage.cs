using UnityEngine;
using System.Collections;
using System;
using WeaveBase;
using MyTcpCommandLibrary.Model;

public class GameScoreMessage : ICheckServerMessage
{
    public void CheckServerMessage(string text)
    {
        // throw new NotImplementedException();
        WeaveSession ws = Newtonsoft.Json.JsonConvert.DeserializeObject<WeaveSession>(text);
        Debug.Log("收到的GameScoreMessage消息是：" + text);
        if (ws.Request == "ServerSendGameScore")
        {
            GameScoreTempModel gsModel = ws.GetRoot<GameScoreTempModel>();
            if (gsModel != null )
            {

                MainClient.Instance.CallSetGameScoreTempModelEvent(gsModel);
              
            }

            else
            {
               // MainClient.Instance.serverBackLoginEvent.Invoke(false);
            }
        }

    }
}
