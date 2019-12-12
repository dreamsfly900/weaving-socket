package weaving.socket;

import android.util.Log;

import java.io.IOException;

public class NETDataSocket implements WsocketEventInterface {
    Wclient wc=new Wclient(DataType.json,this);
    final String IP;
    final int port;
    public  NETDataSocket(String _IP,int _port)
    {
        IP=_IP;
        port=_port;
        new Thread(new Runnable(){
        @Override
        public void run() {
            if(wc.Stat(IP,port,false))
            {
                wc.Send((byte) 0x01,"你好！1231231");
            }else {
                wc.Stop();
            }
        }
    }).start();

    }

    @Override
    public void ReceiveServerEvent(byte command, String text, Wclient soc) {
        try {
            Log.i("-----进入循环----", text);
            wc.Send((byte) 0x01,"你好！1231231");
        }catch (Exception e){}
    }

    @Override
    public void ReceiveServerEventbytes(byte command, byte[] data, Wclient soc) {
        try {
            Log.i("-----进入循环----", new String(data, "utf-8"));
            wc.Send((byte) 0x01,"你好！1231231");
        }catch (Exception e){}

    }

    @Override
    public void Timeoutevent(Wclient soc) {
        soc.Stop();
        wc=new Wclient(DataType.bytes,this);
        new Thread(new Runnable(){
            @Override
            public void run() {
                if(wc.Stat(IP,port,false))
                {

                }else {
                    wc.Stop();
                }
            }
        }).start();
        Log.i("-----超时离线----","超时离线");
    }

    @Override
    public void JumpServerEvent(String str) {

    }
}
