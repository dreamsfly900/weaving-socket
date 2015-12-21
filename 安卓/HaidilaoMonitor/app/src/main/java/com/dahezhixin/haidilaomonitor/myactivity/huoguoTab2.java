package com.dahezhixin.haidilaomonitor.myactivity;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.Bundle;
import android.os.Message;
import android.widget.GridLayout;
import android.widget.Toast;

import com.dahezhixin.haidilaomonitor.R;
import com.dahezhixin.haidilaomonitor.contentclass.LayoutCont;
import com.dahezhixin.haidilaomonitor.contentclass.myData;
import com.dahezhixin.haidilaomonitor.msgservice.MyBroadcastReceiverHandleMessage;
import com.dahezhixin.haidilaomonitor.msgservice.MyBroadcastReceiverHandleState;
import com.dahezhixin.haidilaomonitor.msgservice.MyService;
import com.dahezhixin.haidilaomonitor.usercontroldesign.UserCtr_huoguo;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.ArrayList;
import java.util.TreeMap;

/**
 * Created by hp on 15-12-2.
 */
public class huoguoTab2 extends Activity {
    private Context context = huoguoTab2.this;
    private Intent myDataIntent;
    Thread th2;
    private MyBroadcastReceiverHandleMessage mbcrhmTabd2;
    private MyBroadcastReceiverHandleState mbcrhsTabd2;
    GridLayout gridhg;
    private int len;
    int Dlen;
    private ArrayList<myData> dataList;
    private ArrayList<TreeMap<Integer, LayoutCont>> list2;
    private static int STATE_ADDVIEW = 1;
    private static int STATE_ADDDATA = 2;
    private boolean canaddviewdata = false;
    private boolean isGetviewdata = false;

    String parameter = "datastr";
    ArrayList<LayoutCont> content2;
    String wendu = "当前温度";
    String gongyi = "执行工艺";
    String buzhou = "工艺步骤";
    String shijian = "下次执行时间";
    String zongshu = "总数";
    String kaiguan = "按钮开关";
    String qidong = "启动";
    String fengmingqi = "蜂鸣器";
    UserCtr_huoguo huoguo;

    private ArrayList<myData> zongshulist = new ArrayList<myData>();
    private ArrayList<myData> kaiguanlist = new ArrayList<myData>();
    private ArrayList<myData> qidonglist = new ArrayList<myData>();
    private ArrayList<myData> fengminglist = new ArrayList<myData>();
    private ArrayList<myData> wendulist = new ArrayList<myData>();
    private ArrayList<myData> gongyilist = new ArrayList<myData>();
    private ArrayList<myData> buzhoulist = new ArrayList<myData>();
    private ArrayList<myData> shijianlist = new ArrayList<myData>();
    private ArrayList<myData> wendulist2 = new ArrayList<myData>();
    private ArrayList<myData> gongyilist2 = new ArrayList<myData>();
    private ArrayList<myData> buzhoulist2 = new ArrayList<myData>();
    private ArrayList<myData> shijianlist2 = new ArrayList<myData>();
    private android.os.Handler mHandler = new android.os.Handler() {
        public void handleMessage(Message msg) {
            if (msg.what == STATE_ADDVIEW) {
                if (((String) msg.obj).equals("canaddview")) {
                    isGetviewdata = true;
                    noDataView();
                }
            } else if (msg.what == STATE_ADDDATA) {
                if (((String) msg.obj).equals("canadddata")) {
                    canaddviewdata = true;
                    initView();
                }
            }
        }
    };

    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.layout_huoguotab2);
        content2 = (ArrayList<LayoutCont>) getIntent().getSerializableExtra("listtab2");
        gridhg = (GridLayout) findViewById(R.id.layout_gridhuoguo2);
        getData();
//        noDataView();


        th2 = new Thread() {
            public void run() {
                try {
                    while (true) {
                        String str = "{\"Request\":\"Get_MD\",\"Root\":\"\",\"Parameter\":\"生产区\",\"Token\":" +
                                "\"c0cb6dbc0a8aa2a3a65a8e3030673dd5\",\"Querycount\":0,\"Number\":null}";
                        myDataIntent = new Intent(huoguoTab2.this, MyService.class);
                        myDataIntent.putExtra("msg1", str);
                        myDataIntent.putExtra("command", (byte) 0x08);
                        startService(myDataIntent);
                        sleep(2000);
                    }
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }
            }
        };
        th2.start();


        mbcrhmTabd2 = new MyBroadcastReceiverHandleMessage();
        IntentFilter ifhm = new IntentFilter();
        ifhm.addAction("sendMessage");
        registerReceiver(mbcrhmTabd2, ifhm);
        mbcrhmTabd2.setOnMessageInterfaceListener(new MyBroadcastReceiverHandleMessage.MessageInterface() {
            @Override
            public void getMessage(String msg) {

                try {
                    JSONObject jsonobj = new JSONObject(msg);
                    String request = jsonobj.getString("Request");
                    if (request.equals("Get_MD")) {
                        String rootData = jsonobj.getString("Root");

                        if (!rootData.equals("\"\"")) {
                            JSONObject jobjRootData = new JSONObject(rootData);
                            String strPath = jobjRootData.getString("Path");
                            String strEquipcode = jobjRootData.getString("Equipcode");
                            String strDate = jobjRootData.getString("Date_current");
                            String strMonitor = jobjRootData.getString("Monitor_item");

                            String str = strMonitor.substring(1, strMonitor.length() - 1);
                            String rootdata = str.replaceAll("\"\"", "null").replaceAll("\"", "");
                            String[] source = rootdata.split(",");
                            Dlen = source.length;//obj的个数
                            dataList = new ArrayList<myData>();
                            for (int i = 0; i < Dlen; i++) {
                                String[] data = source[i].split(":");
                                String mytitle = data[0];
                                String myvalue = data[1];
                                myData mydata = new myData(mytitle, myvalue);
                                dataList.add(mydata);
                            }
                        }
                    }
                    parameter = jsonobj.getString("Parameter");
                    String quetycount = jsonobj.getString("Querycount");
                    String number = jsonobj.getString("Number");

                } catch (JSONException e) {
                    e.printStackTrace();
                }
                ArrayList<myData> dataList22 = dataList;

                new Thread() {
                    public void run() {

                        for (int i = 0; i < Dlen; i++) {
                            if (dataList.get(i).getMytitle().contains(kaiguan)) {
                                kaiguanlist.add(dataList.get(i));
                            } else if (dataList.get(i).getMytitle().contains(qidong)) {
                                qidonglist.add(dataList.get(i));
                            } else if (dataList.get(i).getMytitle().contains(fengmingqi)) {
                                fengminglist.add(dataList.get(i));
                            } else if (dataList.get(i).getMytitle().contains(wendu)) {
                                wendulist.add(dataList.get(i));
                            } else if (dataList.get(i).getMytitle().contains(gongyi)) {
                                gongyilist.add(dataList.get(i));
                            } else if (dataList.get(i).getMytitle().contains(buzhou)) {
                                buzhoulist.add(dataList.get(i));
                            } else if (dataList.get(i).getMytitle().contains(shijian)) {
                                shijianlist.add(dataList.get(i));
                            } else if (dataList.get(i).getMytitle().contains(zongshu)) {
                                zongshulist.add(dataList.get(i));
                            }
                        }
                        int lendata1 = wendulist.size() / 2;
                        int lendata2 = wendulist.size();
                        for (int n = lendata1; n < lendata2; n++) {
                            wendulist2.add(wendulist.get(n));//数据按16-32分组 分给每个界面
                            gongyilist2.add(gongyilist.get(n));
                            buzhoulist2.add(buzhoulist.get(n));
                            shijianlist2.add(shijianlist.get(n));
                        }
                        ArrayList<myData> list = new ArrayList<myData>();
                        list = wendulist2;

                        Message m = Message.obtain(mHandler);
                        m.what = STATE_ADDDATA;
                        m.obj = "canadddata";
                        m.sendToTarget();
                    }
                }.start();
            }
        });
        mbcrhsTabd2 = new MyBroadcastReceiverHandleState();
        IntentFilter ifhs = new IntentFilter();
        ifhs.addAction("sendState");
        registerReceiver(mbcrhsTabd2, ifhs);
        mbcrhsTabd2.setOnConnectStateInterfaceListener(new MyBroadcastReceiverHandleState.ConnectStateInterface() {
            @Override
            public void getConnectState(String state) {
                if (state == "connect error") {
                    Toast.makeText(huoguoTab2.this, "服务器正在连接，请稍等......", Toast.LENGTH_SHORT).show();
                } else if (state == "connect success") {
                    Toast.makeText(huoguoTab2.this, "服务器连接成功", Toast.LENGTH_SHORT).show();
                }

            }
        });
        mbcrhsTabd2.setOnReadStateInterfaceListener(new MyBroadcastReceiverHandleState.ReadStateInterface() {
            @Override
            public void getReadState(String state) {
                if (state == "read error") {
                    Toast.makeText(huoguoTab2.this, "数据读取中，请稍等......", Toast.LENGTH_SHORT).show();
                }
            }
        });
        mbcrhsTabd2.setOnSendStateInterfaceListener(new MyBroadcastReceiverHandleState.SendStateInterface() {
            @Override
            public void getSendState(String state) {
                if (state == "send error") {
                    Toast.makeText(huoguoTab2.this, "发送失败，请重新发送", Toast.LENGTH_SHORT).show();
                } else if (state == "wait pro send") {
                    Toast.makeText(huoguoTab2.this, "正在发送中，请稍后...", Toast.LENGTH_SHORT).show();
                } else if (state == "wait connect") {
                    Toast.makeText(huoguoTab2.this, "正在连接服务器", Toast.LENGTH_SHORT).show();
                }
            }
        });

        ArrayList<myData> list2 = new ArrayList<myData>();
        list2 = shijianlist2;


    }

    private void getData() {
        new Thread() {
            public void run() {
                len = content2.size();
                list2 = new ArrayList<TreeMap<Integer, LayoutCont>>();
                int num = 0;
                for (int i = 0; i < len; i += 4) {
                    TreeMap<Integer, LayoutCont> tm = new TreeMap<Integer, LayoutCont>();
                    for (int j = 0; j < 4; j++) {
                        int index = 4 * num + j;
                        if (content2.get(index).getChinName().contains(wendu)) {
                            tm.put(0, content2.get(index));
                        } else if (content2.get(index).getChinName().contains(gongyi)) {
                            tm.put(1, content2.get(index));
                        } else if (content2.get(index).getChinName().contains(buzhou)) {
                            tm.put(2, content2.get(index));
                        } else if (content2.get(index).getChinName().contains(shijian)) {
                            tm.put(3, content2.get(index));
                        }
                    }
                    num++;
                    list2.add(tm);
                }
                Message m = Message.obtain(mHandler);
                m.what = STATE_ADDVIEW;
                m.obj = "canaddview";
                m.sendToTarget();
            }
        }.start();
    }

    private void initView() {
        if (canaddviewdata) {
            int len2 = list2.size();
//            noDataView();
            if (parameter.equals("datastr")) {
//                noDataView();
                Toast.makeText(this, "服务器不稳定，获取不到数据值", Toast.LENGTH_SHORT).show();
            } else if (parameter.equals("无数据")) {
//                noDataView();
                Toast.makeText(this, "无数据", Toast.LENGTH_SHORT).show();
            } else if (wendulist2.size() != 0 && gongyilist2.size() != 0 && buzhoulist2.size() != 0 && shijianlist2.size() != 0) {
                DataView();
            }

        }


    }

    private void noDataView() {
        if (isGetviewdata) {
            int len2 = list2.size();
            for (int i = 0; i < len2; i++) {
                huoguo = new UserCtr_huoguo(context, null);
                huoguo.setPadding(5, 10, 5, 10);
                huoguo.setId(i);
                huoguo.setHg_line0_tv1(list2.get(i).get(0).getChinName());
                huoguo.setHg_line1_tv1(list2.get(i).get(1).getChinName());
                huoguo.setHg_line2_tv1(list2.get(i).get(2).getChinName());
                huoguo.setHg_line3_tv1(list2.get(i).get(3).getChinName());
                gridhg.addView(huoguo);
            }
        }
    }

    private void DataView() {
        int len2 = list2.size();

        for (int i = 0; i < len2; i++) {
            huoguo = (UserCtr_huoguo) findViewById(i);

            ArrayList<myData> list22 = new ArrayList<myData>();
            list22 = gongyilist2;
            ArrayList<myData> list21 = new ArrayList<myData>();
            list21 = shijianlist2;
            String str11 = huoguo.getHg_line0_tv1().toString();
            String str22 = wendulist2.get(i).getMytitle();
            if(str11.contains(str22)){
//            if (huoguo.getHg_line0_tv1().toString().contains(wendulist2.get(i).getMytitle())) {

                huoguo.setHg_line0_tv2(wendulist2.get(i).getMyvalue());
            }
            String str33 = huoguo.getHg_line1_tv1().toString();
            String str44 = gongyilist2.get(i).getMytitle();
            if (str33.contains(str44)){
//            if (huoguo.getHg_line1_tv1().toString().contains(gongyilist2.get(i).getMytitle())) {

                huoguo.setHg_line1_tv2(gongyilist2.get(i).getMyvalue());
            }
            String str1 = huoguo.getHg_line2_tv1().toString();
            String str2 = buzhoulist2.get(i).getMytitle();
//            if (huoguo.getHg_line2_tv1().toString().contains(buzhoulist2.get(i).getMytitle())) {
            if (str1.contains(str2)) {
                huoguo.setHg_line2_tv2(buzhoulist2.get(i).getMyvalue());
            }
            String str3 = huoguo.getHg_line3_tv1().toString();
            String str4 = shijianlist2.get(i).getMytitle();
            if(str3.contains(str4)){
//            if (huoguo.getHg_line3_tv1().toString().contains(shijianlist2.get(i).getMytitle())) {

                huoguo.setHg_line3_tv2(shijianlist2.get(i).getMyvalue());
            }
        }

    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        th2.interrupt();
//        Intent stopintent = new Intent(this, MyService.class);
//        stopService(stopintent);
        unregisterReceiver(mbcrhmTabd2);
        unregisterReceiver(mbcrhsTabd2);
    }

}