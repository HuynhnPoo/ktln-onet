using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameDisplayTxt : TextBase
{
    protected override void PrintText()
    {
        text.SetText(UIManager.Instance?.DisplayNameUI);
    }

    
  
}
