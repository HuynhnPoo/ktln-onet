using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveValueTxt : TextBase
{
    protected override void PrintText()
    {
        text.SetText(GameManager.Instance.ValueRevive.ToString());
    }
}
