using System;


namespace MyTcpCommandLibrary
{
    public enum CommandEnum :byte
    {
        /// <summary>
        /// 
        /// </summary>
        ClientSendLoginModel = 0x02,

        ClientSendGameScoreModel = 0x03,

        ServerSendLoginResult = 0x04,

       // ServerSendGameScoreModel = 0x05,

        ClientSendDisConnected = 0x06,

        ServerSendUpdateGameScoreResult = 0x07,

        ClientSendCheckScore = 0x08,

        ServerSendGetGameScoreResult = 0x09
    }
}
