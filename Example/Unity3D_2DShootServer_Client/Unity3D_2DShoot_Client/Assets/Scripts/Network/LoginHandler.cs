using MyTcpCommandLibrary.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class LoginHandler : MonoBehaviour {


    public InputField input_Username;
    public InputField input_Password;
    
    public InputField input_ServerIP;
    public InputField input_ServerPort;


    public Text server_msg_text;
    public Button login_button;

    // public LoginModel userModel;
    // Use this for initialization
    void Start() {
        MainClient.Instance.serverBackLoginEvent.AddListener(GetServerBackLoginEvent);

        MainClient.Instance.firstCheckServerEvent.AddListener(firstCheckServerConfigEvent);
    }

    private void firstCheckServerConfigEvent(bool connectToServerResult)
    {
        // throw new NotImplementedException();
        connectedServerOK = connectToServerResult;
    }

    public void SetServerIP_Connected()
    {
        string ip = input_ServerIP.text;
        int port = int.Parse(input_ServerPort.text);

        if(connectedServerOK ==false)
        MainClient.Instance.ConnectToServer(ip, port);
    }


    private void GetServerBackLoginEvent(bool arg0)
    {
        //throw new NotImplementedException();
        server_msg = "登陆失败，账号密码错误...";
       

    }

    public bool connectedServerOK = false;

    public void Login()
    {

        LoginTempModel model = new LoginTempModel()
        {
            userName = input_Username.text,
            password = input_Password.text,
            //userName = "ssss",
            //password = "yyyyyy",
            logintime = System.DateTime.Now.ToString("yyyyMMddHHmmssfff")
        };
       // userModel = model;
        MainClient.Instance.InvokeSetLoginTempModelEvent(model);
        MainClient.Instance.SendLogin(model);

    }

    private string server_msg = "";

    public void SetServerMsgShow(string serverMsg)
    {
        server_msg_text.text = serverMsg;
    }

    void OnDestroy()
    {
        MainClient.Instance.serverBackLoginEvent.RemoveListener(GetServerBackLoginEvent);

        MainClient.Instance.firstCheckServerEvent.RemoveListener(firstCheckServerConfigEvent);

    }





    // Update is called once per frame
    void Update () {
		if( string.IsNullOrEmpty( server_msg) ==false || server_msg.Length >2)
        {
            SetServerMsgShow(server_msg);
            login_button.gameObject.SetActive(true);
            server_msg = "";
        }
	}
}


