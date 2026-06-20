using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinTxt : TextBase
{
    public bool isTotalCoin;
    protected override void PrintText()
    {
        if (isTotalCoin)
        {

            text.SetText(ShopManager.Instance.localPlayerData.gold.ToString());
        }
        else
        {
            text.SetText(GameManager.Instance.Coin.ToString());

        }
    }


}
