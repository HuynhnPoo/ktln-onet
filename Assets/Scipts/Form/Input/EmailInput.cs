using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmailInput : InputBase
{
    public string EmailName { get; private set; }
    protected override void OnEndEdit(string text)
    {
        EmailName=text;
    }

}
