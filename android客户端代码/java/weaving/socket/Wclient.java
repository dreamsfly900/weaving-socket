package chuangzhiwei.com.ledopen.socket;
import android.util.Log;

import java.io.DataOutputStream;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.InetSocketAddress;
import java.net.Socket;
import java.util.Date;
import java.util.concurrent.LinkedBlockingQueue;
import java.util.concurrent.ThreadPoolExecutor;
import java.util.concurrent.TimeUnit;

public class Wclient {
    Socket socclient;
    String IP;int Port;
     public int TimeOutNumber=30;
    public  boolean Isline=false;
    byte[] alldata =new byte[0];
    Date timeout = new Date(System.currentTimeMillis());
    DataType DT;
    WsocketEventInterface WEIF;
    public Wclient(DataType dt,WsocketEventInterface weif) {

        DT=dt;
        WEIF=weif;
    }
    final ThreadPoolExecutor threadPoolExecutor = new ThreadPoolExecutor(3,5,1, TimeUnit.SECONDS,
            new LinkedBlockingQueue<Runnable>(50));
    public boolean Stat(String ip,int port,boolean istoken)
    {
        try {
            IP=ip;
            Port=port;
            socclient = new Socket();
            socclient.connect(new InetSocketAddress(IP, Port));

            int ss=0;
            while (!socclient.isConnected()) {
                ss++;
                Thread.sleep(1000);//线程休息0.5秒继续执行
                if (ss > 10)
                    return false;
            }
            Isline = true;
            serverRead();
            if(istoken) {
                 ss=0;
                while (Tokan=="")
                {
                    Thread.sleep(1000);//线程休息0.5秒继续执行
                    ss++;
                    if (ss > 10)
                        return false;
                }
                return true;
            }
         //   this.mCallback.onConnect(this);
           return  true;
        } catch (Exception e) {
            return  false;
        }
    }
    public void Unup() {
        if (DT == DataType.custom) {
            int bytesRead = alldata.length;

            if (bytesRead == 0)
            {
                return;
            }
            final   byte[] tempbtye = new byte[bytesRead];

            System.arraycopy(alldata,0, tempbtye, 0,tempbtye.length);
            Runnable runnable = new Runnable() {
                @Override
                public void run() {
                    try {
                        WEIF.ReceiveServerEventbytes(tempbtye[0],tempbtye,Wclient.this);
                    } catch (Exception e) {
                        //e.printStackTrace();
                       //   Log.i("ReceiveServerEventbytes--->", "————ReceiveServerEventbytes————" + e.getMessage());
                    }
                }
            };
            threadPoolExecutor.execute(runnable);
            alldata = new byte[0];
        } else if (DT == DataType.bytes) {
            Unupbyte();

        } else if (DT == DataType.json) {

            Unupjson();
        }
    }
    public int ConvertToInt(byte[] list)
    {
        int ret = 0;
        int i = 0;
        for (byte item : list)
        {
          if(item>=0) 
    ret += ((item) << i);
else 
   ret += ((item& 0xFF) << i);
            i += 8;
        }
        return ret;
    }

    public byte[] ConvertToByteList(int v)
    {
        int value = v;
        int i=0;
        while (value != 0) {
            i++;
            value >>= 8;
        }
        byte[] ret = new byte[i];
        value = v;
        i=0;
        while (value != 0)
        {
            ret[i]=((byte)value);
            value >>= 8;
            i++;
        }

        return ret;
    }
    void Unupbyte()
    {
        try
        {
            {
            //    lb0x99:
                int bytesRead = alldata.length;

                if (bytesRead == 0)
                {
                    return;
                }

                byte[] tempbtye = new byte[bytesRead];

                System.arraycopy(alldata,0, tempbtye, 0,tempbtye.length);


                if (bytesRead > 2)
                {
                    int a = tempbtye[1];
                    if (bytesRead > 4 + a)
                    {
                        int len = 0;

                        byte[] bbcrc = new byte[4 + a];
                        System.arraycopy(tempbtye, 0, bbcrc, 0, 4 + a);
                        if (CRC.DataCRC(bbcrc, 4 + a))
                        {
                            byte[] bb = new byte[a];
                            System.arraycopy(tempbtye, 2, bb, 0, a);
                            len = ConvertToInt(bb);
                        }
                        else
                        {
                            byte[] temps = new byte[tempbtye.length - 1];
                            System.arraycopy(tempbtye, 1, temps, 0, temps.length);
                            alldata = temps;
                            return;
                                //  goto lb0x99;
                        }
                        try
                        {
                            if ((len + 4 + a) > tempbtye.length)
                            {
                                return;
                            }
                            else if (tempbtye.length > (len + 4 + a))
                            {
                                byte[] temps = new byte[tempbtye.length - (len + 4 + a)];
                                System.arraycopy(tempbtye, (len + 4 + a), temps, 0, temps.length);
                                alldata = temps;
                                //return;
                                //  goto lb0x99;
                            }
                            else if (tempbtye.length == (len + 4 + a))
                            { alldata = new byte[0]; }
                        }
                        catch (Exception e)
                        {
                          //  ErrorMge?.Invoke(3, e.StackTrace + "unup001:" + e.Message + "2 + a" + 2 + a + "---len" + len + "--tempbtye" + tempbtye.Length);
                            alldata = new byte[0];
                        }
                        try
                        {
                           final byte[] bs = new byte[len];
                            System.arraycopy(tempbtye, (4 + a), bs, 0, bs.length);
                            if (tempbtye[0]== -103)
                                return;
                           final byte command=tempbtye[0];
                            Runnable runnable = new Runnable() {
                                @Override
                                public void run() {
                                    try {
                                        WEIF.ReceiveServerEventbytes(command,bs,Wclient.this);
                                    } catch (Exception e) {
                                        //e.printStackTrace();
                                        //   Log.i("ReceiveServerEventbytes--->", "————ReceiveServerEventbytes————" + e.getMessage());
                                    }
                                }
                            };
                            threadPoolExecutor.execute(runnable);


                            return;
                        }
                        catch (Exception e)
                        {
                          //  ErrorMge?.Invoke(3, e.StackTrace + "unup122:" + e.Message);
                            alldata = new byte[0];
                        }

                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
        }
        catch (Exception e)
        {
            //ErrorMge?.Invoke(3, "unup:" + e.Message + "---" + e.StackTrace);
            alldata = new byte[0];
        }
    }
    String   Tokan ="";
    void Unupjson()
    {
        try
        {
            {
             //   lb0x99:
                int bytesRead = alldata.length;

                if (bytesRead == 0)
                {
                    return;
                }


                byte[] tempbtye = new byte[bytesRead];

                System.arraycopy(alldata,0, tempbtye,0, tempbtye.length);


                if (bytesRead > 2)
                {
                    int a = tempbtye[1];
                    if (bytesRead > 2 + a)
                    {
                        int len = 0;

                        String temp =new String(tempbtye, 2, a, "utf-8");// System.Text.Encoding.UTF8.GetString(tempbtye, 2, a);
                        len = 0;
                        try
                        {
                            len = Integer.parseInt(temp);
                            if (len == 0)
                            { alldata = new byte[0]; return; }
                        }
                        catch(Exception e)
                        {
                            byte[] temps = new byte[tempbtye.length - 1];
                            System.arraycopy(tempbtye, 1, temps, 0, temps.length);
                            alldata = temps;
                            return;
                        }

                        try
                        {
                            if ((len + 2 + a) > tempbtye.length)
                            {

                                return;
                            }
                            else if (tempbtye.length > (len + 2 + a))
                            {
                                byte[] temps = new byte[tempbtye.length - (len + 2 + a)];
                                System.arraycopy(tempbtye, (len + 2 + a), temps, 0, temps.length);
                                alldata = temps;

                            }
                            else if (tempbtye.length == (len + 2 + a))
                            { alldata = new byte[0]; }
                        }
                        catch (Exception e)
                        {
                         //   ErrorMge?.Invoke(3, e.StackTrace + "unup001:" + e.Message + "2 + a" + 2 + a + "---len" + len + "--tempbtye" + tempbtye.Length);
                            alldata = new byte[0];
                        }
                        try
                        {

                            temp = new String(tempbtye, 2 + a, len, "utf-8");//System.Text.Encoding.UTF8.GetString(tempbtye, 2 + a, len);

                            if (tempbtye[0]== -1)
                            {
                                if (temp.indexOf("token") >= 0)
                                    Tokan = temp.split("\\|")[1];
                                else if (temp.indexOf("jump") >= 0)
                                {
                                    Tokan = "连接数量满";
                                    WEIF.JumpServerEvent(temp.split("\\|")[1]);
                                    Isline=false;
                                }
                                else
                                {
                                    //ReceiveServerEvent?.Invoke(tempbtye[0], temp);

                                  //  ReceiveServerEvent?.BeginInvoke(tempbtye[0], temp,null, null);
                                    //ReceiveServerEventobj?.BeginInvoke(tempbtye[0], temp, this, null, null);
                                    final byte command=tempbtye[0];
                                    final  String data=temp;
                                    Runnable runnable = new Runnable() {
                                        @Override
                                        public void run() {
                                            try {
                                                WEIF.ReceiveServerEvent(command,data,Wclient.this);
                                            } catch (Exception e) {
                                                //e.printStackTrace();
                                                //   Log.i("ReceiveServerEventbytes--->", "————ReceiveServerEventbytes————" + e.getMessage());
                                            }
                                        }
                                    };
                                    threadPoolExecutor.execute(runnable);
                                }
                            }
                            else if (tempbtye[0]== -103)
                                return;
                            else
                            {
                                final byte command=tempbtye[0];
                                final  String data=temp;
                                Runnable runnable = new Runnable() {
                                    @Override
                                    public void run() {
                                        try {
                                            WEIF.ReceiveServerEvent(command,data,Wclient.this);
                                        } catch (Exception e) {
                                            //e.printStackTrace();
                                            //   Log.i("ReceiveServerEventbytes--->", "————ReceiveServerEventbytes————" + e.getMessage());
                                        }
                                    }
                                };
                                threadPoolExecutor.execute(runnable);
                            }



                        }
                        catch (Exception e)
                        {
                           // ErrorMge?.Invoke(3, e.StackTrace + "unup122:" + e.Message);
                            alldata = new byte[0];
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
        }
        catch (Exception e)
        {
         //   ErrorMge?.Invoke(3, "unup:" + e.Message + "---" + e.StackTrace);
            alldata = new byte[0];
        }
    }
    public boolean Send(byte command, String text)
    {
        try
        {
            if (DT == DataType.json)
            {
                byte[] sendb = text.getBytes("UTF-8");
                byte[] lens =Integer.toString ( sendb.length).getBytes("UTF-8");
                byte[] b = new byte[2 + lens.length + sendb.length];
                b[0] = command;
                b[1] = (byte)lens.length;
                System.arraycopy(lens, 0, b, 2, lens.length);
                System.arraycopy(sendb, 0, b, 2 + lens.length, sendb.length);

                Send(b);
            }
            else if (DT == DataType.bytes)
            {
                return Send(command, text.getBytes("UTF-8"));
            }

        }
        catch (Exception ee)
        {
            if (Isline)
            {
               // WEIF.Timeoutevent(this);
                //Send(command, text);
            }


            Stop();
            WEIF.Timeoutevent(this);
           // ErrorMge(9, "send:" + ee.Message);
            return false;
        }
        return true;
    }

    public void Stop() {
        Isline = false;
        alldata = new byte[0];
        try {
            socclient.close();
        } catch (Exception e) {
        }
    }

    public boolean Send(byte command, byte[] text)
    {
        boolean bb = false;
        try
        {
            if (DT == DataType.json)
            {
                return Send(command, new String(text,"utf-8") );
            }
            else if (DT == DataType.bytes)
            {

                byte[] sendb = text;
                byte[] lens = ConvertToByteList(sendb.length);
                byte[] b = new byte[2 + 2 + lens.length + sendb.length];
                b[0] = command;
                b[1] = (byte)lens.length;
               // lens.CopyTo(b, 2);
                System.arraycopy(lens, 0, b, 2, lens.length);
                b= CRC.ConCRC( b, 2 + lens.length);
                System.arraycopy(sendb, 0, b, 2 + 2+lens.length, sendb.length);
              //  sendb.CopyTo(b, 2 + 2 + lens.length);
                bb = Send(b);

            }

        }
        catch (Exception ee)
        {

            Stop();
            WEIF.Timeoutevent(this);

            return false;
        }
        return bb;
    }

    public boolean Send(byte[] b) {
        try {
            OutputStream outPutStream = socclient.getOutputStream();//生成输出流
            DataOutputStream dout = new DataOutputStream(outPutStream);
            dout.write(b);////写入
            dout.flush(); //强制请求清空缓冲区，让i/o系统立马完成它应该完成的输入、输出动作。
            return true;
        } catch (Exception e) {
            Stop();
            WEIF.Timeoutevent(this);
            return false;
        }
    }

    private void serverRead() {
        new Thread() {
            @Override
            public void run() {



                while (Isline) {
                    try {

                        InputStream bff = socclient.getInputStream(); //获得网络连接输入，同时返回一个InputStream实例
                        int len = bff.available();//读取输入流的长度

                        if (len <= 0) {

                            if (alldata.length > 3)
                            {
                               Unup();
                            }else {
                                sleep(10);//线程休息0.5秒继续执行

                            }
                        } else {
                            timeout= new Date(System.currentTimeMillis());
                            byte[] tempbyte = new byte[len];
                            bff.read(tempbyte, 0, len);//读取输入流，将数据读取到tempbyte数组中

                               // Log.i("-----进入循环----", "----tempb-------");

                               // Log.i("tempbyte.len--->", "————本次读到的长度——————"+String.valueOf(tempbyte.length));
                                byte[] tempb  = new byte[alldata.length];
                          //  Log.i("tempb.len----->", "————上一次的长度——————"+String.valueOf(tempb.length));
                                System.arraycopy(alldata, 0, tempb, 0, tempb.length);
                                int lle = alldata.length;
                                int bytesRead = tempbyte.length;
                                byte[] temp = new byte[lle + bytesRead];
                                System.arraycopy(tempb, 0, temp, 0, lle);
                                System.arraycopy(tempbyte, 0, temp,lle, bytesRead);
                                alldata=temp;
                            Unup();
                             //   Log.i("tempbyte.length--->", "————合并后的长度————"+String.valueOf(tempbyte.length));
                        }
                            Date tt= new Date(System.currentTimeMillis());
                         long miao= ((tt.getTime()-timeout.getTime())/1000);
                        Log.i("tempbyte.length--->", "————超时时间————"+miao);
                                 if(miao>TimeOutNumber)
                                 {
                                     Isline=false;
                                     //超时
                                     WEIF.Timeoutevent(Wclient.this);
                                     return;
                                 }


                    } catch (Exception e) {
                        Isline=false;
                       // intentState.putExtra("read", "read error");
                    //    sendBroadcast(intentState);
                        WEIF.Timeoutevent(Wclient.this);
                        return;


                    }

                }
            }
        }.start();
    }

}
