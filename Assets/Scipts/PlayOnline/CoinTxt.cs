using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinTxt : TextBase
{
    protected override void PrintText()
    {
        text.SetText(GameManager.Instance.TotalCoinOnline.ToString());
    }

   
}
