using MyTcpClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WeaveBase;

public class LoginHandler : MonoBehaviour {


    public InputField input_Message;
  
    public InputField input_ServerIP;
    public InputField input_ServerPort;


    public Text server_msg_text;
    public Button connect_button;

    public Button send_button;


    public WeaveSocketGameClientUseEZThread weaveSocketGameClient;

    // public LoginModel userModel;
    // Use this for initialization
    void Start() {
        connect_button.onClick.AddListener(() => ConnectToServer());

        send_button.onClick.AddListener(() => SendMessageToServer());
    }


    public void ConnectToServer()
    {
        string serverIp = input_ServerIP.text;
        int port = int.Parse( input_ServerPort.text ); 


        if (string.IsNullOrEmpty(serverIp))
            return;

        try
        {
            //  weaveSocketGameClient = new WeaveSocketGameClient(SocketDataType.Json);
            weaveSocketGameClient = new WeaveSocketGameClientUseEZThread(SocketDataType.Json);
            weaveSocketGameClient.ConnectOkEvent += OnConnectOkEvent;
            weaveSocketGameClient.ReceiveMessageEvent += OnReceiveMessageEvent;
            weaveSocketGameClient.ErrorMessageEvent += OnErrorMessageEvent;
            weaveSocketGameClient.ReceiveBitEvent += OnReceiveBitEvent;
            weaveSocketGameClient.TimeOutEvent += OnTimeOutEvent;

            //pcp2.AddListenClass(new MyClientFunction());
            Debug.Log("初始化OK");
            //bool bb = pcp2.start("61.184.86.126", 10155, false);
            // bool bb = weaveSocketGameClient.StartConnect("61.184.86.126", 10155, 30, false);
            bool bb = weaveSocketGameClient.StartConnect(serverIp, port, 30, false);
            Debug.Log("链接OK");
           
        }
        catch
        {
            Debug.Log("无法连接服务器，发生错误");
        }

    }

 
    private void OnTimeOutEvent()
    {
        Debug.Log("连接超时");
        //throw new NotImplementedException();
    }

    private void OnReceiveBitEvent(byte command, byte[] data)
    {
        Debug.Log("收到了Bit数据");
        // throw new NotImplementedException();
    }

    private void OnErrorMessageEvent(int type, string error)
    {
        Debug.Log("发生了错误");
        //throw new NotImplementedException();
    }

    private void OnReceiveMessageEvent(byte command, string text)
    {
        // throw new NotImplementedException();
        Debug.Log("收到了新数据");

        //throw new NotImplementedException();
        server_msg = "指令:" + command + ".内容:" + text;
        Debug.Log("原始数据是：" + server_msg);
        try
        {
            WeaveSession ws = Newtonsoft.Json.JsonConvert.DeserializeObject<WeaveSession>(text);
            Debug.Log("接受到的WeaveSession数据是：" + ws.Request + " " + ws.Root);
        }
        catch
        {
            Debug.Log("Json转换对象出错了");
        }
        // receiveMessage = "指令:" + command + ".内容:" + text;
      
       

    }

    public void SendMessageToServer()
    {
        if (weaveSocketGameClient == null)
            return;

        string inputMsg = input_Message.text;

        if( string.IsNullOrEmpty(inputMsg) ==false)
        // pcp2.SendRoot<int>(0x01, "login", 11111, 0);
        weaveSocketGameClient.SendRoot<string>(0x01, "UnityChat", inputMsg, 0);
    }


    private void OnConnectOkEvent()
    {
        Debug.Log("已经连接成功");
    }



    private void StopConnect()
    {
        if (weaveSocketGameClient != null)
        {

            weaveSocketGameClient.CloseConnect();
            weaveSocketGameClient.ConnectOkEvent -= OnConnectOkEvent;
            weaveSocketGameClient.ReceiveMessageEvent -= OnReceiveMessageEvent;
            weaveSocketGameClient.ErrorMessageEvent -= OnErrorMessageEvent;
            weaveSocketGameClient.ReceiveBitEvent -= OnReceiveBitEvent;
            weaveSocketGameClient.TimeOutEvent -= OnTimeOutEvent;

             weaveSocketGameClient = null;

        }

    }


    private string server_msg = "";

    public void SetServerMsgShow(string serverMsg)
    {
        server_msg_text.text = serverMsg;
    }

    void OnDestroy()
    {
        StopConnect();
    }





    // Update is called once per frame
    void Update () {
		if( string.IsNullOrEmpty( server_msg) ==false || server_msg.Length >2)
        {
            SetServerMsgShow(server_msg);
           // login_button.gameObject.SetActive(true);
            server_msg = "";



        }

       

	}
}


