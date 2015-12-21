package com.dahezhixin.haidilaomonitor.msgservice;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;

/**
 * Created by hp on 15-8-20.
 */
public class MyBroadcastReceiverHandleState extends BroadcastReceiver {

    /**
     * 定义一个接口用来处理接收到广播数据
     */
    ReadStateInterface readStateInterface;
    SendStateInterface sendStateInterface;
    ConnectStateInterface connectStateInterface;
    @Override
    public void onReceive(Context context, Intent intent) {
        //拿到进度，更新UI
        String read = "";
        String send = "";
        String connect = "";
        read = intent.getStringExtra("read");
        send = intent.getStringExtra("send");
        connect = intent.getStringExtra("connect");
        if(read == null || read.equals("")){
        }else{
            readStateInterface.getReadState(read);
        }

        if(send == null || send.equals("")){
        }else{
            sendStateInterface.getSendState(send);
        }

        if(connect == null || connect.equals("")){
        }else{
            connectStateInterface.getConnectState(connect);
        }

    }

    public interface ReadStateInterface {
        public void getReadState(String state);
    }
    public interface SendStateInterface {
        public void getSendState(String state);
    }
    public interface ConnectStateInterface {
        public void getConnectState(String state);
    }

    public void setOnReadStateInterfaceListener(ReadStateInterface readStateInterface) {
        this.readStateInterface = readStateInterface;
    }

    public void setOnSendStateInterfaceListener(SendStateInterface sendStateInterface) {
        this.sendStateInterface = sendStateInterface;
    }

    public void setOnConnectStateInterfaceListener(ConnectStateInterface connectStateInterface) {
        this.connectStateInterface = connectStateInterface;
    }
}
