using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStatusTxt : TextBase
{
    protected override void PrintText()
    {
        text.SetText(UIManager.Instance.levelStatusStr);
    }

  
    
}
