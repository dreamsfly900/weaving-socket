package com.dahezhixin.haidilaomonitor.msgservice;

import android.annotation.SuppressLint;
import android.app.Service;
import android.content.Intent;
import android.os.Handler;
import android.os.IBinder;
import android.os.Message;
import android.util.Log;

import java.io.DataOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.InetSocketAddress;
import java.net.Socket;
import java.net.SocketAddress;


/**
 * Created by hp on 15-8-20.
 */
@SuppressLint("HandlerLeak")
public class MyService extends Service {

    private String TAG = "MsgService------>";
    private String Th = "Thread=========";

    //    private String ip = "192.168.1.93";//消息提醒
//  
    //    private String ip = "192.168.1.115";//火锅
    private int port = 9090;

    private Intent intentMessage = new Intent("sendMessage");
    private Intent intentState = new Intent("sendState");

    private Socket client;
    private byte num;
    private String line;
    private OutputStream outPutStream;
    private InputStream bff;
    private boolean readThreadFlag = true;
    private static boolean FLag103 = true;

    private final int STATE_CONNECT = 1;
    private final int STATE_SEND = 2;

    private boolean flagConnect = false;
    private boolean canSend = true;

    private Handler mHandler = new Handler() {
        public void handleMessage(Message msg) {
            if (msg.what == STATE_CONNECT) {
                if (((String) msg.obj).equals("success")) {
                    flagConnect = true;
                    serverRead();//如果连接成功就读取信息
                } else {
                    flagConnect = false;
                }
            } else if (msg.what == STATE_SEND) {
                canSend = true;
            }
        }
    };

    /**
     * 在service刚被创建的时候变进行耗时操作
     */
    @Override
    public void onCreate() {
        Log.i(TAG, "onCreate");
        super.onCreate();
        socketConnect(ip, port);
    }

    @Override
    public int onStartCommand(Intent intent, int flags, int startId) {

        Log.i(TAG, "onStartCommand");
        FLag103 = true;
        if (flagConnect) {
            if (canSend) {

                String str1 = intent.getStringExtra("msg1");//获取Activity传来的msg
                byte bt = intent.getByteExtra("command", (byte) 0x01);//获取msg的标志
                sendMsg(bt, str1);
            } else {
                intentState.putExtra("send", "wait pro send");
                sendBroadcast(intentState);
            }
        } else {
            intentState.putExtra("send", "wait connect");
            sendBroadcast(intentState);
        }

        return super.onStartCommand(intent, flags, startId);
    }

    @Override
    public IBinder onBind(Intent arg0) {
        return null;
    }

    @Override
    public void onDestroy() {
        Log.i(TAG, "onDestroy");
        super.onDestroy();
        readThreadFlag = false;
    }

    private void socketConnect(final String ip, final int port) {
        client = new Socket();
        new Thread() {
            @Override
            public void run() {
                try {
                    SocketAddress socketAddress = new InetSocketAddress(ip, port);
                    client.connect(socketAddress, 5000);

                    Message m = Message.obtain(mHandler);
                    m.what = STATE_CONNECT;
                    m.obj = "success";
                    m.sendToTarget();

                    intentState.putExtra("connect", "connect success");
                    sendBroadcast(intentState);

                } catch (IOException e) {
                    e.printStackTrace();

                    Message m = Message.obtain(mHandler);
                    m.what = STATE_CONNECT;
                    m.obj = "error";
                    m.sendToTarget();
                    intentState.putExtra("connect", "connect error");
                    sendBroadcast(intentState);
                }

            }
        }.start();
    }


    private void serverRead() {
        new Thread() {
            @Override
            public void run() {

                byte[] tempb = null;
                lableread:
                while (readThreadFlag) {
                    try {
                        line = null;
                        bff = client.getInputStream(); //获得网络连接输入，同时返回一个InputStream实例
                        int len = bff.available();//读取输入流的长度

                        if (len <= 0) {
                            sleep(1000);//线程休息0.5秒继续执行
                        } else {
                            byte[] tempbyte = new byte[len];
                            bff.read(tempbyte, 0, len);//读取输入流，将数据读取到tempbyte数组中

                            if (tempb != null) {
                                Log.i("-----进入循环----", "----tempb-------");
                                Log.i("tempb.len----->", "————上一次的长度——————"+String.valueOf(tempb.length));
                                Log.i("tempbyte.len--->", "————本次读到的长度——————"+String.valueOf(tempbyte.length));
                                byte[] tempbytes = new byte[tempbyte.length + tempb.length];
                                System.arraycopy(tempb, 0, tempbytes, 0, tempb.length);
                                System.arraycopy(tempbyte, 0, tempbytes, tempb.length, tempbyte.length);
                                tempbyte = tempbytes;
                                Log.i("tempbyte.length--->", "————合并后的长度————"+String.valueOf(tempbyte.length));
                            }
                            lable103:
                            while (FLag103) {
                                if (tempbyte[0] == -103) {
                                    Log.i("--读到-103心跳包--->", new String(tempbyte,"utf-8"));
                                    if (len > 1) {
                                        byte[] b = new byte[len - 1];
                                        byte[] t = tempbyte;
                                        System.arraycopy(t, 1, b, 0, b.length);
                                        tempbyte = b;
                                        len = len - 1;
                                    }
                                    continue lableread;
//                                byte[] bytesTEMP = new byte[len - 1];
//                                Array.set(bytesTEMP, 1, tempbyte);//将指定数组对象中索引组件的值设置为指定的新值。
//                                line = "没有读到数据";
                                }
                                //*读到数据开始解码*//*
                                Log.i("command--->", String.valueOf(tempbyte[0]));
                                byte command = tempbyte[0];//标识位
                                num = command;
                                int lenlen = tempbyte[1];//字符串长度的长度
                                String temp = new String(tempbyte, 2, lenlen, "utf-8");//字符串长度
                                int lens = Integer.parseInt(temp);//十六进制转化为十进制

                                Log.i("11111111", "11-------------数据比对----------------11");
                                //如果数据的长度大于读到的长度(没有读完)
                                if (2 + lenlen + lens > tempbyte.length) {
                                    Log.i("没有读完--->", "a——————读取到的放到tempb中再重头读——————a");
                                    tempb = new byte[tempbyte.length];
                                    System.arraycopy(tempbyte, 0, tempb, 0, tempbyte.length);
                                    Log.i("---总长度lens---", String.valueOf(lens));
                                    Log.i("tempb.len--->", "第一次读到的长度-->"+String.valueOf(tempb.length));
                                    continue lableread;
                                }

                                //刚好读够
                                line = new String(tempbyte, 2 + lenlen, lens, "utf-8");
                                Log.i("----刚好够----", line);
                                FLag103 = false;

                                if ((2 + lenlen + lens) < len) {//读多了
                                    Log.i("读多了--->", "***********完整的数据截取后再封装**************");
                                    byte[] tempover = new byte[len - (2 + lenlen + lens)];
                                    byte[] t = tempbyte;
                                    System.arraycopy(t, 2 + lenlen + lens, tempover, 0, tempover.length);//多读的部分从头分出来
                                    tempbyte = tempover;
                                    len = len - (2 + lenlen + lens);
                                    Log.i("封装后的len--->", String.valueOf(len));
                                    FLag103 = true;
                                    continue lable103;
                                }


                            }

                        }


                        if (num != -103 && line != null) {
                            //ok line
                            intentMessage.putExtra("msg", line);
                            sendBroadcast(intentMessage);

                        }
                    } catch (IOException e) {
                        intentState.putExtra("read", "read error");
                        sendBroadcast(intentState);

                    } catch (InterruptedException e) {
                        intentState.putExtra("read", "read error");
                        sendBroadcast(intentState);
                    }

                }
            }
        }.start();
    }


    private void sendMsg(final byte command, final String msg) {
        canSend = false;
        new Thread() {
            public void run() {
                try {
                    byte[] sendb = msg.getBytes("UTF-8");//系统默认的编码格式的字节数组
                    //这里有个组合方法
                    byte[] a = Integer.toString(sendb.length).getBytes("UTF-8");//把sendb的长度转化成byte型
                    byte[] b = new byte[2 + a.length + sendb.length];
                    b[0] = command;
                    b[1] = (byte) a.length;
                    System.arraycopy(a, 0, b, 2, a.length);
                    System.arraycopy(sendb, 0, b, 2 + a.length, sendb.length);
                    outPutStream = client.getOutputStream();//生成输出流
                    DataOutputStream dout = new DataOutputStream(outPutStream);
                    dout.write(b);////写入
                    dout.flush(); //强制请求清空缓冲区，让i/o系统立马完成它应该完成的输入、输出动作。

                    Message m = Message.obtain(mHandler);
                    m.what = STATE_SEND;
                    m.obj = "success";
                    m.sendToTarget();

                    intentState.putExtra("send", "send to handler success");
                    sendBroadcast(intentState);
                } catch (IOException e) {
                    e.printStackTrace();
                    Log.v("A", e.toString());

                    Message m = Message.obtain(mHandler);
                    m.what = STATE_SEND;
                    m.obj = "error";
                    m.sendToTarget();

                    intentState.putExtra("send", "send error");
                    sendBroadcast(intentState);
                }
            }
        }.start();
    }

}
