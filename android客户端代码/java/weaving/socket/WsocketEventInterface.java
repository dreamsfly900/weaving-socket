package weaving.socket;

public interface WsocketEventInterface {
    public   void ReceiveServerEvent(byte command, String text, Wclient soc);
    public   void ReceiveServerEventbytes(byte command,byte[] data, Wclient soc);
    public void  Timeoutevent(Wclient soc);
    public void  JumpServerEvent(String str);

}
