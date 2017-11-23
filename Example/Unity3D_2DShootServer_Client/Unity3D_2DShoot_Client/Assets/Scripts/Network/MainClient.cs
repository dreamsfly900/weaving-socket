using UnityEngine;
using System.Collections;
using GDGeek;
using MyTcpClient;
using System;
using UnityEngine.SceneManagement;
using WeaveBase;
using MyTcpCommandLibrary;
using UnityEngine.Events;
using MyTcpCommandLibrary.Model;

public class MainClient : Singleton<MainClient>
{


    public WeaveSocketGameClient weaveSocketGameClient;

    public ServerBackLoginEvent serverBackLoginEvent =new ServerBackLoginEvent();

  
    public SetLoginTempModelEvent setLoginTempModelEvent = new SetLoginTempModelEvent();

    public SetGameScoreTempModelEvent setGameScoreTempModelEvent = new SetGameScoreTempModelEvent();

    public FirstCheckServerEvent firstCheckServerEvent = new FirstCheckServerEvent();

    public GameScoreTempModel GameScore;

    public LoginTempModel loginUserModel;
    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(this);
        setLoginTempModelEvent.AddListener(SetLoginModel);


    }
    public string receiveMessage;

    public void InvokeSetLoginTempModelEvent(LoginTempModel _model)
    {
        setLoginTempModelEvent.Invoke(_model);
    }
    GameScoreTempModel tempGameScore;
    public void CallSetGameScoreTempModelEvent(GameScoreTempModel gsModel)
    {
        // StartCoroutine(CallSetGameScoreEvent(gsModel));
        tempGameScore = gsModel;
    }

    IEnumerator CallSetGameScoreEvent(GameScoreTempModel gsModel)
    {
        yield return new WaitForSeconds(0.5f);
        setGameScoreTempModelEvent.Invoke(gsModel);
    }


    // Update is called once per frame
    void Update()
    {
        //if (receiveMessage.Length != 0)
        //{
        //    receiveMessage = string.Empty;
        //}
        if (canLoadSceneFlag)
        {
            LoadGameScene();
            canLoadSceneFlag = false;
        }


        if (weaveSocketGameClient != null)
            weaveSocketGameClient.OnTick();



        if(tempGameScore != null)
        {
            StartCoroutine(CallSetGameScoreEvent(tempGameScore));
            tempGameScore = null;
        }
    }

    public void ConnectToServer(string serverIp,int port)
    {
        try
        {
            weaveSocketGameClient = new WeaveSocketGameClient(SocketDataType.Json);
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
             firstCheckServerEvent.Invoke(bb);
        }
        catch
        {
            firstCheckServerEvent.Invoke(false);
        }

    }

    void CallServerFunc()
    {
        try
        {
            //weaveSocketGameClient.SendRoot<int>(0x02, "login", 11111, 0);
            //在加个发送
            weaveSocketGameClient.tokan = "UnityTokan";
            weaveSocketGameClient.SendRoot<int>(0x01, "getnum", 0, 0);
            //调用服务端方法getnum，是服务端的方法。
            //这样就可以了，我们试试
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
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
        receiveMessage = "指令:" + command + ".内容:" + text;
        Debug.Log("原始数据是：" + receiveMessage);
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
        Debug.Log("收到的信息是：" + receiveMessage);
        ICheckServerMessageFactory factory = CheckCommand.CheckCommandType(command);

        ICheckServerMessage checkSmsg = factory.CheckServerMessage();
        checkSmsg.CheckServerMessage(text);

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

            // weaveSocketGameClient = null;

        }

    }



    public  void SendLogin(LoginTempModel user )
    {
        try
        {

            // weaveSocketGameClient.SendRoot<LoginTempModel>((byte)CommandEnum.ClientSendLoginModel, "CheckLogin", user, 0);
            StartCoroutine(  WaitSendLogin(user)  );
            Debug.Log("SendLoginFunc");
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    IEnumerator WaitSendLogin(LoginTempModel user)
    {
        yield return new WaitForSeconds(0.2f);
        weaveSocketGameClient.SendRoot<LoginTempModel>((byte)CommandEnum.ClientSendLoginModel, "CheckLogin", user, 0);
    }

    public void SendCheckUserScore()
    {
        try
        {
            LoginTempModel user = loginUserModel;
            // weaveSocketGameClient.Tokan = "UnityTokan";
            // weaveSocketGameClient.SendRoot<int>(0x01, "getnum", 0, 0);

            weaveSocketGameClient.SendRoot<LoginTempModel>((byte)CommandEnum.ClientSendGameScoreModel, "GetUserScore", user, 0);
            Debug.Log("SendLoginFunc");
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }


    public void SendNewScoreUpdate(int _score,int _missed)
    {
        try
        {
            GameScoreTempModel gsModel = new GameScoreTempModel()
            {
                userName = loginUserModel.userName,
                missenemy = _missed,
                score = _score
            };
            //LoginTempModel user = loginUserModel;
            // weaveSocketGameClient.Tokan = "UnityTokan";
            // weaveSocketGameClient.SendRoot<int>(0x01, "getnum", 0, 0);

            weaveSocketGameClient.SendRoot<GameScoreTempModel>((byte)CommandEnum.ClientSendGameScoreModel, "UpdateScore", gsModel, 0);
            Debug.Log("UpdateScore");
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }




    public int sceneIndex;
    public bool canLoadSceneFlag;
     void LoadGameScene()
    {
        SceneManager.LoadScene(1);
    }


    public void SetLoadSceneFlag()
    {
        canLoadSceneFlag = true;
    }

    void OnDestroy()
    {
        //SetLoginModelEvent.RemoveListener(SetLoginModel);

    }

    private void SetLoginModel(LoginTempModel _model)
    {
        loginUserModel = _model;
    }

    private void OnApplicationQuit()
    {
        StopConnect();
    }



}


public class SetLoginTempModelEvent : UnityEvent<LoginTempModel> { }

public class SetGameScoreTempModelEvent : UnityEvent<GameScoreTempModel> { }

public class ServerBackLoginEvent : UnityEvent<bool>{}

public class FirstCheckServerEvent : UnityEvent<bool> { }