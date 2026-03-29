using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameDisplayInput : InputBase
{
    public string NameDisplay { get; private set; }
    protected override void OnEndEdit(string text)
    {
        NameDisplay = text;
    }

   
}
