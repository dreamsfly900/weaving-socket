using UnityEngine;
using System.Collections;

using MyTcpCommandLibrary;
public class CheckCommand 
{

    public static ICheckServerMessageFactory CheckCommandType(byte commandtype)
    {
        ICheckServerMessageFactory factory = null;
        switch (commandtype)
        {
            case (byte)CommandEnum.ServerSendLoginResult:
                factory =new LoginMessageFactory();
                break;
            case (byte)CommandEnum.ServerSendGetGameScoreResult:
                factory = new GameScoreMessageFactory();
                break;

            //case (byte)CommandEnum.ServerSendGameScoreModel:
            //    factory = new GameScoreMessageFactory();
            //    break;
        }

        return factory;
    }

    
}

