using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResutlsGametxt : TextBase
{
    protected override void PrintText()
    {
        this.text.SetText(GameManager.Instance.StatusGameStr);
    }

  
}
