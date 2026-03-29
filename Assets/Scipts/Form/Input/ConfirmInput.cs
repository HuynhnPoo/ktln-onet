using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmInput : InputBase
{

    public string PasswordConfirm { get; private set; }

    protected override void OnEndEdit(string text)
    {
        PasswordConfirm = text;
    }
}
