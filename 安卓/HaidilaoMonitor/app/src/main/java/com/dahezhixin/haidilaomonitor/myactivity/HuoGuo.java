package com.dahezhixin.haidilaomonitor.myactivity;

import android.app.Activity;
import android.app.AlertDialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.Bundle;
import android.os.Message;
import android.util.Log;
import android.view.KeyEvent;
import android.view.View;
import android.view.Window;
import android.widget.ImageButton;
import android.widget.LinearLayout;
import android.widget.Toast;

import com.dahezhixin.haidilaomonitor.R;
import com.dahezhixin.haidilaomonitor.contentclass.LayoutCont;
import com.dahezhixin.haidilaomonitor.contentclass.myData;
import com.dahezhixin.haidilaomonitor.msgservice.MyBroadcastReceiverHandleMessage;
import com.dahezhixin.haidilaomonitor.msgservice.MyBroadcastReceiverHandleState;
import com.dahezhixin.haidilaomonitor.msgservice.MyService;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.util.ArrayList;

/**
 * Created by hp on 15-12-2.
 */
public class HuoGuo extends Activity {

    private Intent myIntent;
    private MyBroadcastReceiverHandleMessage mbcrhm;
    private MyBroadcastReceiverHandleState mbcrhs;
    private ArrayList<LayoutCont> mContent = new ArrayList<LayoutCont>();
    private ArrayList<LayoutCont> listTab1;
    private ArrayList<LayoutCont> listTab2;
    private String eng_name, chia_name, coordinate_offset, groupname, parameter, quetycount, number;
    private int exid, display_way;

    private LinearLayout btnTab1, btnTab2;
    //    private Button btnTab1, btnTab2;
    private ImageButton imgbtnexit, imgbtnshezhi;
    private Intent intent1, intent2;

    private static int STATE_ADDVIEW = 1;
    private static int STATE_ADDDATA = 2;
    private static int STATE_FINISH = 3;
    private boolean adddata = false;
    private boolean addview = false;
    //    private boolean isFinished = false;
    ArrayList<myData> dataList;

    private android.os.Handler mHandler = new android.os.Handler() {
        public void handleMessage(Message msg) {
            if (msg.what == STATE_ADDDATA) {
                if (((String) msg.obj).equals("canadddata")) {
                    adddata = true;
                    initData();
                }
            }
            if (msg.what == STATE_ADDVIEW) {
                if (((String) msg.obj).equals("can_add_view")) {
                    addview = true;
//                    isFinished = true;

//                    initView();
                }
            }
        }
    };

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        requestWindowFeature(Window.FEATURE_NO_TITLE);// 去掉标题栏
        setContentView(R.layout.layout_huoguo);
        initView();

        /**
         * 发送数据给广播
         */
        String str = "{\"Request\":\"Get_EX\",\"Root\":\"\",\"Parameter\":\"\"," +
                "\"Token\":\"c0cb6dbc0a8aa2a3a65a8e3030673dd5\",\"Querycount\":0,\"Number\":null}";
        myIntent = new Intent(HuoGuo.this, MyService.class);
        myIntent.putExtra("msg1", str);
        myIntent.putExtra("command", (byte) 0x08);
        startService(myIntent);

        /**
         * 接收导数据并解析
         */
        mbcrhm = new MyBroadcastReceiverHandleMessage();
        IntentFilter ifhm = new IntentFilter();
        ifhm.addAction("sendMessage");
        registerReceiver(mbcrhm, ifhm);
        mbcrhm.setOnMessageInterfaceListener(new MyBroadcastReceiverHandleMessage.MessageInterface() {
            @Override
            public void getMessage(String msg) {

                try {
                    JSONObject jsonobj = new JSONObject(msg);
                    String request = jsonobj.getString("Request");
                    if (request.equals("Get_EX")) {
                        String root = jsonobj.getString("Root");
                        JSONArray jarrayRoot = new JSONArray(root);
                        int len = jarrayRoot.length();

                        for (int i = 0; i < len; i++) {
                            JSONObject jobjRoot = jarrayRoot.getJSONObject(i);
                            exid = jobjRoot.getInt("EXID");
                            eng_name = jobjRoot.getString("ENG_NAME");
                            chia_name = jobjRoot.getString("CHIN_NAME");
                            display_way = jobjRoot.getInt("DISPLAY_WAY");
                            coordinate_offset = jobjRoot.getString("COORDINATE_OFFSET");
                            groupname = jobjRoot.getString("GROUP_NAME");
//                            LayoutCont laycon = new LayoutCont(exid, eng_name, chia_name, display_way, coordinate_offset, groupname);
                            LayoutCont laycon = new LayoutCont(chia_name, groupname);
                            mContent.add(laycon);
                        }
                        parameter = jsonobj.getString("Parameter");
                        quetycount = jsonobj.getString("Querycount");
                        number = jsonobj.getString("Number");

                    }

                    Message m = Message.obtain(mHandler);
                    m.what = STATE_ADDDATA;
                    m.obj = "canadddata";
                    m.sendToTarget();

                } catch (JSONException e) {
                    e.printStackTrace();
                }

//                ArrayList<myData> wen = wendulist2;

            }
        });


        mbcrhs = new MyBroadcastReceiverHandleState();
        IntentFilter ifhs = new IntentFilter();
        ifhs.addAction("sendState");
        registerReceiver(mbcrhs, ifhs);
        mbcrhs.setOnConnectStateInterfaceListener(new MyBroadcastReceiverHandleState.ConnectStateInterface()

                                                  {
                                                      @Override
                                                      public void getConnectState(String state) {
                                                          if (state == "connect error") {
                                                              Toast.makeText(HuoGuo.this, "服务器正在连接，请稍等......", Toast.LENGTH_SHORT).show();
                                                          } else if (state == "connect success") {
                                                              Toast.makeText(HuoGuo.this, "服务器连接成功", Toast.LENGTH_SHORT).show();
                                                          }
                                                      }
                                                  }

        );
        mbcrhs.setOnReadStateInterfaceListener(new MyBroadcastReceiverHandleState.ReadStateInterface()

                                               {
                                                   @Override
                                                   public void getReadState(String state) {
                                                       if (state == "read error") {
                                                           Toast.makeText(HuoGuo.this, "数据读取中，请稍等......", Toast.LENGTH_SHORT).show();
                                                       }
                                                   }
                                               }

        );
        mbcrhs.setOnSendStateInterfaceListener(new MyBroadcastReceiverHandleState.SendStateInterface()

                                               {
                                                   @Override
                                                   public void getSendState(String state) {
                                                       if (state == "send error") {
                                                           Toast.makeText(HuoGuo.this, "发送失败，请重新发送", Toast.LENGTH_SHORT).show();
                                                       } else if (state == "wait pro send") {
                                                           Toast.makeText(HuoGuo.this, "正在发送中，请稍后...", Toast.LENGTH_SHORT).show();
                                                       } else if (state == "wait connect") {
                                                           Toast.makeText(HuoGuo.this, "正在连接服务器", Toast.LENGTH_SHORT).show();
                                                       }

                                                   }
                                               }

        );
    }

    private void initData() {
        if (adddata) {
            int length = mContent.size();
            listTab1 = new ArrayList<LayoutCont>();
            listTab2 = new ArrayList<LayoutCont>();
            for (int i = 0; i < length; i++) {
                String strGroupname = mContent.get(i).getGroupName();
                if (strGroupname.equals("系统总览1-16")) {
                    listTab1.add(mContent.get(i));
                } else if (strGroupname.equals("系统总览16-32")) {
                    listTab2.add(mContent.get(i));
                }
            }
            Message m = Message.obtain(mHandler);
            m.what = STATE_ADDVIEW;
            m.obj = "can_add_view";
            m.sendToTarget();
        }

    }

    private void initView() {
//        if (addview) {
        imgbtnexit = (ImageButton) findViewById(R.id.imageButton);
        btnTab1 = (LinearLayout) findViewById(R.id.btn_tab1);
        btnTab2 = (LinearLayout) findViewById(R.id.btn_tab2);
//        imgbtnshezhi = (ImageButton) findViewById(R.id.img_shezhi);

        btnTab1.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if (addview) {
                    intent1 = new Intent(HuoGuo.this, huoguoTab1.class);
                    intent1.putExtra("listtab1", listTab1);
                    startActivity(intent1);
                } else {
                    Toast.makeText(HuoGuo.this, "请等待数据加载", Toast.LENGTH_SHORT).show();
                }
            }
        });
        btnTab2.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if (addview) {
                    intent2 = new Intent(HuoGuo.this, huoguoTab2.class);
                    intent2.putExtra("listtab2", listTab2);
                    startActivity(intent2);
                } else {
                    Toast.makeText(HuoGuo.this, "请等待数据加载", Toast.LENGTH_SHORT).show();
                }

            }
        });

        imgbtnexit.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                dialog();
            }
        });
//        imgbtnshezhi.setOnClickListener(new View.OnClickListener() {
//            @Override
//            public void onClick(View v) {
//
//            }
//        });


    }


    @Override
    public boolean onKeyDown(int keyCode, KeyEvent event) {
        if (keyCode == KeyEvent.KEYCODE_BACK && event.getRepeatCount() == 0) {//按下的如果是BACK，同时没有重复

            dialog();
        } else if (keyCode == KeyEvent.KEYCODE_MENU
                && event.getRepeatCount() == 0) {
            return true; // 返回true就不会弹出默认的setting菜单
        }

        return false;
    }

    protected void dialog() {
        AlertDialog.Builder builder = new AlertDialog.Builder(HuoGuo.this);
        builder.setMessage("确认退出吗？");

        builder.setTitle("提示");

        builder.setPositiveButton("确认", new DialogInterface.OnClickListener() {

            @Override
            public void onClick(DialogInterface dialog, int which) {
                dialog.dismiss();

                HuoGuo.this.finish();
            }
        });

        builder.setNegativeButton("取消", new DialogInterface.OnClickListener() {
            @Override
            public void onClick(DialogInterface dialog, int which) {
                dialog.dismiss();
            }
        });

        builder.create().show();
    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        Log.i("huoguo--->", "onDestory");
        Intent stopintent = new Intent(this, MyService.class);
        stopService(stopintent);

        unregisterReceiver(mbcrhm);
        unregisterReceiver(mbcrhs);
    }

}
