package com.dahezhixin.haidilaomonitor.myactivity;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.IntentFilter;
import android.os.Bundle;
import android.os.Message;
import android.widget.GridLayout;
import android.widget.LinearLayout;
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
public class huoguoTab1 extends Activity {
    private Context context = huoguoTab1.this;
    LinearLayout layouthg;
    GridLayout gridhg;

    private Thread th1;

    private Intent myIntent;
    private MyBroadcastReceiverHandleMessage mbcrhmdata;
    private MyBroadcastReceiverHandleState mbcrhsdata;

    private int len;
    private ArrayList<TreeMap<Integer, LayoutCont>> list1;
    private static int STATE_ADDVIEW = 1;
    private static int STATE_ADDDATA = 2;
    private boolean canaddviewdata = false;
    private boolean isGetviewdata = false;
    private boolean canaddview = false;

    UserCtr_huoguo huoguo;
    String wendu = "当前温度";
    String gongyi = "执行工艺";
    String buzhou = "工艺步骤";
    String shijian = "下次执行时间";
    String zongshu = "总数";
    String kaiguan = "按钮开关";
    String qidong = "启动";
    String fengmingqi = "蜂鸣器";
    String parameter;

    private ArrayList<myData> zongshulist = new ArrayList<myData>();
    private ArrayList<myData> kaiguanlist = new ArrayList<myData>();
    private ArrayList<myData> qidonglist = new ArrayList<myData>();
    private ArrayList<myData> fengminglist = new ArrayList<myData>();
    private ArrayList<myData> wendulist = new ArrayList<myData>();
    private ArrayList<myData> gongyilist = new ArrayList<myData>();
    private ArrayList<myData> buzhoulist = new ArrayList<myData>();
    private ArrayList<myData> shijianlist = new ArrayList<myData>();
    private ArrayList<myData> wendulist1 = new ArrayList<myData>();
    private ArrayList<myData> gongyilist1 = new ArrayList<myData>();
    private ArrayList<myData> buzhoulist1 = new ArrayList<myData>();
    private ArrayList<myData> shijianlist1 = new ArrayList<myData>();


    int Dlen;
    ArrayList<myData> dataList;
    ArrayList<LayoutCont> content;

    private android.os.Handler mHandler = new android.os.Handler() {
        public void handleMessage(Message msg) {
            if (msg.what == STATE_ADDVIEW) {
                if (((String) msg.obj).equals("canaddview")) {
                    isGetviewdata = true;
                    noDataView();//填充控件
                }
            } else if (msg.what == STATE_ADDDATA) {
                if (((String) msg.obj).equals("canadddata")) {
                    canaddviewdata = true;
                    initView();//添加数据
                }
            }
        }
    };


    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.layout_huoguotab1);
        content = (ArrayList<LayoutCont>) getIntent().getSerializableExtra("listtab1");
        gridhg = (GridLayout) findViewById(R.id.layout_gridhuoguo);
        /**
         * 先将界面的数据传输过来
         */
        getData();
/**
 *不断向服务器发送数据请求
 */
        th1 = new Thread() {
            public void run() {
                try {
                    while (true) {

                        String str = "{\"Request\":\"Get_MD\",\"Root\":\"\",\"Parameter\":\"生产区\",\"Token\":" +
                                "\"c0cb6dbc0a8aa2a3a65a8e3030673dd5\",\"Querycount\":0,\"Number\":null}";
                        myIntent = new Intent(huoguoTab1.this, MyService.class);
                        myIntent.putExtra("msg1", str);
                        myIntent.putExtra("command", (byte) 0x08);
                        startService(myIntent);

                        sleep(2000);
                    }
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }
            }
        };
        th1.start();


        mbcrhmdata = new MyBroadcastReceiverHandleMessage();
        IntentFilter ifhm = new IntentFilter();
        ifhm.addAction("sendMessage");
        registerReceiver(mbcrhmdata, ifhm);
        mbcrhmdata.setOnMessageInterfaceListener(new MyBroadcastReceiverHandleMessage.MessageInterface() {
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
                if(dataList!=null) {

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
                    for (int m = 0; m < lendata1; m++) {
                        wendulist1.add(wendulist.get(m));//数据按1-16分组 分给每个界面
                        gongyilist1.add(gongyilist.get(m));
                        buzhoulist1.add(buzhoulist.get(m));
                        shijianlist1.add(shijianlist.get(m));
                    }
                }
                Message m = Message.obtain(mHandler);
                m.what = STATE_ADDDATA;
                m.obj = "canadddata";
                m.sendToTarget();
            }
        });

        mbcrhsdata = new MyBroadcastReceiverHandleState();
        IntentFilter ifhs = new IntentFilter();
        ifhs.addAction("sendState");
        registerReceiver(mbcrhsdata, ifhs);
        mbcrhsdata.setOnConnectStateInterfaceListener(new MyBroadcastReceiverHandleState.ConnectStateInterface() {
            @Override
            public void getConnectState(String state) {
                if (state == "connect error") {
                    Toast.makeText(huoguoTab1.this, "服务器正在连接，请稍等......", Toast.LENGTH_SHORT).show();
                } else if (state == "connect success") {
                    Toast.makeText(huoguoTab1.this, "服务器连接成功", Toast.LENGTH_SHORT).show();
                }

            }
        });
        mbcrhsdata.setOnReadStateInterfaceListener(new MyBroadcastReceiverHandleState.ReadStateInterface() {
            @Override
            public void getReadState(String state) {
                if (state == "read error") {
                    Toast.makeText(huoguoTab1.this, "数据读取中，请稍等......", Toast.LENGTH_SHORT).show();
                }
            }
        });
        mbcrhsdata.setOnSendStateInterfaceListener(new MyBroadcastReceiverHandleState.SendStateInterface() {
            @Override
            public void getSendState(String state) {
                if (state == "send error") {
                    Toast.makeText(huoguoTab1.this, "发送失败，请重新发送", Toast.LENGTH_SHORT).show();
                } else if (state == "wait pro send") {
                    Toast.makeText(huoguoTab1.this, "正在发送中，请稍后...", Toast.LENGTH_SHORT).show();
                } else if (state == "wait connect") {
                    Toast.makeText(huoguoTab1.this, "正在连接服务器", Toast.LENGTH_SHORT).show();
                }
            }
        });


    }

    private void getData() {
        new Thread() {
            public void run() {
                len = content.size();

                list1 = new ArrayList<TreeMap<Integer, LayoutCont>>();
                int num = 0;
                for (int i = 0; i < len; i += 4) {
                    TreeMap<Integer, LayoutCont> tm = new TreeMap<Integer, LayoutCont>();
                    for (int j = 0; j < 4; j++) {
                        int index = 4 * num + j;
                        if (content.get(index).getChinName().contains(wendu)) {
                            tm.put(0, content.get(index));
                        } else if (content.get(index).getChinName().contains(gongyi)) {
                            tm.put(1, content.get(index));
                        } else if (content.get(index).getChinName().contains(buzhou)) {
                            tm.put(2, content.get(index));
                        } else if (content.get(index).getChinName().contains(shijian)) {
                            tm.put(3, content.get(index));
                        }
                    }
                    num++;
                    list1.add(tm);
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

            if (parameter.equals("datastr")) {
                Toast.makeText(this, "服务器不稳定，获取不到数据值", Toast.LENGTH_SHORT).show();
            } else if (parameter.equals("无数据")) {
                Toast.makeText(this, "无数据", Toast.LENGTH_SHORT).show();
            } else if (wendulist1.size() != 0 && gongyilist1.size() != 0 && buzhoulist1.size() != 0 && shijianlist1.size() != 0) {
                DataView();
            }

        }
    }

    /**
     * 手动生成控件
     */
    private void noDataView() {
        if (isGetviewdata) {
            int len2 = list1.size();
            for (int i = 0; i < len2; i++) {
                huoguo = new UserCtr_huoguo(context, null);
                huoguo.setPadding(5, 10, 5, 10);
                huoguo.setId(i);
                huoguo.setHg_line0_tv1(list1.get(i).get(0).getChinName());
                huoguo.setHg_line1_tv1(list1.get(i).get(1).getChinName());
                huoguo.setHg_line2_tv1(list1.get(i).get(2).getChinName());
                huoguo.setHg_line3_tv1(list1.get(i).get(3).getChinName());
                gridhg.addView(huoguo);
            }
        }
    }

    /**
     * 填充数据
     */
    private void DataView() {
        int len1 = list1.size();
        for (int i = 0; i < len1; i++) {
            huoguo = (UserCtr_huoguo) findViewById(i);

            if (huoguo.getHg_line0_tv1().toString().contains(wendulist1.get(i).getMytitle())) {
                huoguo.setHg_line0_tv2(wendulist1.get(i).getMyvalue());
            }


            if (huoguo.getHg_line1_tv1().toString().contains(gongyilist1.get(i).getMytitle())) {
                huoguo.setHg_line1_tv2(gongyilist1.get(i).getMyvalue());
            }

            if (huoguo.getHg_line2_tv1().toString().contains(buzhoulist1.get(i).getMytitle())) {
                huoguo.setHg_line2_tv2(buzhoulist1.get(i).getMyvalue());
            }

            if (huoguo.getHg_line3_tv1().toString().contains(shijianlist1.get(i).getMytitle())) {
                huoguo.setHg_line3_tv2(shijianlist1.get(i).getMyvalue());
            }

        }

    }

    @Override
    protected void onDestroy() {
        super.onDestroy();
        th1.interrupt();
        unregisterReceiver(mbcrhmdata);
        unregisterReceiver(mbcrhsdata);
    }
}
