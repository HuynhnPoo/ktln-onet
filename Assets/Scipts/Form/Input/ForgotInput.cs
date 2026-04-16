using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForgotInput : InputBase
{
    public string ForgotPassword { get; private set; }
    protected override void OnEndEdit(string text)
    {
        ForgotPassword = text;
    }

}
