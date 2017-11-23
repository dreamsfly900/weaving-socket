using UnityEngine;
using System.Collections;

public class GameScoreMessageFactory : ICheckServerMessageFactory
{
    public ICheckServerMessage CheckServerMessage()
    {
        //throw new NotImplementedException();
        return new GameScoreMessage();
    }
}
