package com.dahezhixin.haidilaomonitor.myactivity;

import android.app.Activity;
import android.content.Intent;
import android.content.IntentFilter;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.preference.PreferenceManager;
import android.util.Log;
import android.view.KeyEvent;
import android.view.View;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.EditText;
import android.widget.Toast;

import com.dahezhixin.haidilaomonitor.R;
import com.dahezhixin.haidilaomonitor.msgservice.MyBroadcastReceiverHandleMessage;
import com.dahezhixin.haidilaomonitor.msgservice.MyBroadcastReceiverHandleState;
import com.dahezhixin.haidilaomonitor.msgservice.MyService;


public class Login extends Activity {
    private Intent myIntent, mIntent;
    //声明自定义的broadcastreceiver
    private MyBroadcastReceiverHandleMessage mbcrhmlog;
    private MyBroadcastReceiverHandleState mbcrhslog;
    String logntoken, lognerror;
    private String str1 = null;

    private SharedPreferences prefer;
    private SharedPreferences.Editor sharedit;

    private Button login;
    private EditText userName, password;
    private CheckBox rememberpass;
//    private RadioGroup accountflag;
//    private RadioButton comuser, admin;
    private String account = "admin", mima = "admin";
    private String strUsName, strPsWorld, usName, psWord, md5PsWorld, logerror, token;
    public int Querycount;
    public boolean Flag;
    String strlogin;


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.login1);

        userName = (EditText) findViewById(R.id.etname);
        password = (EditText) findViewById(R.id.etpass);
//        accountflag = (RadioGroup) findViewById(R.id.radiogroupflg);
//        comuser = (RadioButton) findViewById(R.id.rbcomuser);
//        admin = (RadioButton) findViewById(R.id.rbadmin);
        rememberpass = (CheckBox) findViewById(R.id.rememberpswd);
//        exit = (Button) findViewById(R.id.btexit);
        login = (Button) findViewById(R.id.btlogin);
        //记住密码 1.设置manager
        prefer = PreferenceManager.getDefaultSharedPreferences(this);
        Boolean isRemember = prefer.getBoolean("remember_pass", false);

//        accountflag.setOnCheckedChangeListener(new RadioGroup.OnCheckedChangeListener() {
//            @Override
//            public void onCheckedChanged(RadioGroup group, int checkedId) {
//                RadioButton r = (RadioButton) findViewById(checkedId);
//                if (checkedId == comuser.getId()) {
//                    Querycount = 0;//普通用户
//                    Flag = true;
//                } else {
//                    Querycount = 1;//管理员
//                    Flag = false;
//                }
//            }
//        });
        //记住密码 2
        if (isRemember) {
            //将账号和密码都设置到文本框中
            usName = prefer.getString("usName", "");
            psWord = prefer.getString("psWord", "");
            userName.setText(usName);
            password.setText(psWord);
            rememberpass.setChecked(true);
        }


        //启动服务
        mIntent = new Intent(Login.this, MyService.class);
        startService(mIntent);

        login.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                strUsName = userName.getText().toString();
                strPsWorld = password.getText().toString();
//                md5PsWorld = MD5util.string2MD5(strPsWorld);
////                str1 = "{UserName:\"fangfang\",Passworld:\"7cad0b44c11c8e5459e1bf59e4d8d7ea\",Flag:\"true\",Token:\"\",Error:\"\"}\n";
//                strlogin = "{UserName:\"" + strUsName + "\",Passworld:\"" + md5PsWorld +
//                        "\",Flag:\"" + Flag + "\",Token:\"\",Error:\"\"}";
//                myIntent = new Intent(Login.this, MyService.class);
//                myIntent.putExtra("msg1", strlogin);
//                myIntent.putExtra("command", (byte) 0x00);
//                startService(myIntent);

                if(strUsName.equals(account)&&strPsWorld.equals(account)) {

                    //记住密码 3.
                    sharedit = prefer.edit();
                    if (rememberpass.isChecked()) {
                        sharedit.putInt("accountflag", Querycount);
                        sharedit.putBoolean("Flag", Flag);
                        sharedit.putBoolean("remember_pass", true);
                        sharedit.putString("usName", strUsName);
                        sharedit.putString("psWord", strPsWorld);
                    } else {
                        sharedit.clear();
                    }
                    sharedit.commit();


                    Intent intent = new Intent();
                    intent.setClass(Login.this,HuoGuo.class);
                    startActivity(intent);
                    finish();
                }
            }
        });

        mbcrhmlog = new MyBroadcastReceiverHandleMessage();
        IntentFilter ifhm = new IntentFilter();
        ifhm.addAction("sendMessage");
        registerReceiver(mbcrhmlog, ifhm);
        mbcrhmlog.setOnMessageInterfaceListener(new MyBroadcastReceiverHandleMessage.MessageInterface() {
            @Override
            public void getMessage(String msg) {
//                try {
//                    JSONObject loginJson = new JSONObject(msg);
//                    logntoken = loginJson.getString("Token");
//                    lognerror = loginJson.getString("Error");
//                } catch (JSONException e) {
//                    e.printStackTrace();
//                }
//                if (logntoken != null) {//用户名 密码正确了
//                    //记住密码 3.
//                    sharedit = prefer.edit();
//                    if (rememberpass.isChecked()) {
//                        sharedit.putInt("accountflag", Querycount);
//                        sharedit.putBoolean("Flag", Flag);
//                        sharedit.putBoolean("remember_pass", true);
//                        sharedit.putString("usName", strUsName);
//                        sharedit.putString("psWord", strPsWorld);
//                    } else {
//                        sharedit.clear();
//                    }
//                    sharedit.commit();
//
//
//                    Intent intent = new Intent();
//                    Bundle bundle = new Bundle();
//                    bundle.putInt("querycount", Querycount);
//                    intent.putExtra("loginName", strUsName);
//                    intent.putExtra("passworld", strPsWorld);
//                    intent.putExtras(bundle);
//
//                    startActivity(intent);
//                    finish();
//                } else {
//                    Toast.makeText(Login.this, lognerror + "请重新输入用户名或密码", Toast.LENGTH_SHORT).show();
//                }
//
//
            }
        });

        mbcrhslog = new MyBroadcastReceiverHandleState();
        IntentFilter ifhs = new IntentFilter();
        ifhs.addAction("sendState");
        registerReceiver(mbcrhslog, ifhs);
        mbcrhslog.setOnConnectStateInterfaceListener(new MyBroadcastReceiverHandleState.ConnectStateInterface() {
            @Override
            public void getConnectState(String state) {
                if (state == "connect error") {
                    Toast.makeText(Login.this, "服务器正在连接，请稍等......", Toast.LENGTH_SHORT).show();
                } else if (state == "connect success") {
                    Toast.makeText(Login.this, "服务器连接成功", Toast.LENGTH_SHORT).show();
                }

            }
        });
        mbcrhslog.setOnReadStateInterfaceListener(new MyBroadcastReceiverHandleState.ReadStateInterface() {
            @Override
            public void getReadState(String state) {
                if (state == "read error") {
                    Toast.makeText(Login.this, "数据读取中，请稍等......", Toast.LENGTH_SHORT).show();
                }
            }
        });
        mbcrhslog.setOnSendStateInterfaceListener(new MyBroadcastReceiverHandleState.SendStateInterface() {
            @Override
            public void getSendState(String state) {
                if (state == "send error") {
                    Toast.makeText(Login.this, "发送失败，请重新发送", Toast.LENGTH_SHORT).show();
                } else if (state == "wait pro send") {
                    Toast.makeText(Login.this, "正在发送中，请稍后...", Toast.LENGTH_SHORT).show();
                } else if (state == "wait connect") {
                    Toast.makeText(Login.this, "正在连接服务器", Toast.LENGTH_SHORT).show();
                }

            }
        });

//        exit.setOnClickListener(new View.OnClickListener() {
//            @Override
//            public void onClick(View v) {
//                finish();
//                onDestroy();
//                Intent stopintent = new Intent(Login.this, MyService.class);
//                stopService(stopintent);
//            }
//        });
    }
    //双击退出
    private long firstTime = 0;
    @Override
    public boolean onKeyUp(int keyCode, KeyEvent event) {
        // TODO Auto-generated method stub
        switch(keyCode)
        {
            case KeyEvent.KEYCODE_BACK:
                long secondTime = System.currentTimeMillis();
                if (secondTime - firstTime > 2000) {                                         //如果两次按键时间间隔大于2秒，则不退出
                    Toast.makeText(this, "再按一次退出程序", Toast.LENGTH_SHORT).show();
                    firstTime = secondTime;//更新firstTime
                    return true;
                } else {
                    Intent stopintent = new Intent(Login.this, MyService.class);
                    stopService(stopintent);//两次按键小于2秒时，退出应用
                    finish();
                    System.exit(0);
                }
                break;
        }
        return super.onKeyUp(keyCode, event);
    }
    @Override
    protected void onDestroy() {
        super.onDestroy();
        unregisterReceiver(mbcrhmlog);
        unregisterReceiver(mbcrhslog);
        Log.i("myLoginactivity---->","onDestory");
    }

}
