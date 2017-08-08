using client;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaveBase;
public class test
{
    [InstallFunAttribute("forever")]//客户端也支持像服务端那样写，刚才看懂返回的内容也是testaabb，所以客户端也要把方法命名testaabb
    public void login(System.Net.Sockets.Socket soc, WeaveBase.WeaveSession _0x01)
    {
        Debug.Log("login方法：" + _0x01.GetRoot<int>().ToString());
        //  Gw_EventMylog("",_0x01.Getjson());
    }
}
public class Test : MonoBehaviour {
    public static P2Pclient pcp2;
    void Awake() {
        pcp2 = new P2Pclient(false);
        pcp2.receiveServerEvent += Pcp2_receiveServerEvent;
        pcp2.timeoutevent += Pcp2_timeoutevent;
        pcp2.ErrorMge += Pcp2_ErrorMge;
        pcp2.AddListenClass(new test());
        bool bb=pcp2.start("127.0.0.1", 8989, false);
        Debug.Log(bb.ToString());
        try
        {
            pcp2.SendRoot<int>(0x02, "login", 11111, 0);
        }
        catch (Exception e)
        { Debug.Log(e.ToString()); }
    }

    private void Pcp2_ErrorMge(int type, string error)
    {
        Debug.Log(error);
    }
    void Start () {
       
    }
    /// <summary>
    /// 连接超时
    /// </summary>
    private void Pcp2_timeoutevent()
    {
        
    }
   
    private void Pcp2_receiveServerEvent(byte command, string text)
    {
        if (command == 0x01)
        {
            Debug.Log("0x01");
        }
        if (command == 0x02) {
            Debug.Log("0x02");
        }
       // Debug.Log(text);
    }
    void Update () {
		
	}
}
