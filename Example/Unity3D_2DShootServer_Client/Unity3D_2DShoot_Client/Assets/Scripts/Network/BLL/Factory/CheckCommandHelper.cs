using UnityEngine;
using System.Collections;

public static class CheckCommandHelper 
{
    public static ICheckServerMessageFactory SwitchCheckCommand(byte command)
    {
        ICheckServerMessageFactory factory = null;
        switch (command)
        {
            case (0x1):
                factory = new LoginMessageFactory();
                break;
            case (0x2):
                factory = new GameScoreMessageFactory();
                break;
                //其他类型略；
        }
        return factory;
    }
    
}
