package com.dahezhixin.haidilaomonitor.msgservice;

import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.Intent;

/**
 * Created by hp on 15-8-20.
 */
public class MyBroadcastReceiverHandleMessage extends BroadcastReceiver {

    MessageInterface messageInterface;
    @Override
    public void onReceive(Context context, Intent intent) {
        String msg = intent.getStringExtra("msg");
        messageInterface.getMessage(msg);
    }

    public interface MessageInterface {
        public void getMessage(String msg);
    }

    public void setOnMessageInterfaceListener(MessageInterface msgInterface) {
        this.messageInterface = msgInterface;
    }

}

