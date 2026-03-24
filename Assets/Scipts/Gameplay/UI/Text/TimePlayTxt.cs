using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimePlayTxt : TextBase
{
    protected override void PrintText()
    {
        this.text.SetText(Mathf.FloorToInt(GameMechanics.CountDown()).ToString() + "s");
    }
}
