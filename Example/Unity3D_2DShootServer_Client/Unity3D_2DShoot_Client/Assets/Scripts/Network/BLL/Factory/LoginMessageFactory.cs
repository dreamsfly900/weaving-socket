using UnityEngine;
using System.Collections;
using System;

public class LoginMessageFactory : ICheckServerMessageFactory
{
    public ICheckServerMessage CheckServerMessage()
    {
        //throw new NotImplementedException();
        return new LoginMessage();
    }
}
