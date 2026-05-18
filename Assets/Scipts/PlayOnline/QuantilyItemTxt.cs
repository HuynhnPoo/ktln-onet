using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuantilyItemTxt : TextBase
{
    public string nameItemBoost = "";

    protected override void PrintText()
    {
       
        text.SetText(PlayFabDataManager.Instance.playerData.GetItemCount(nameItemBoost).ToString());
    }
}
