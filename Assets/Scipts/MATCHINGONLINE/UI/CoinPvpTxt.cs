using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinPvpTxt : TextBase
{
    protected override void PrintText()
    {
       
    }

  
    public void DisplayRewardCoins(int coinAmount)
    {
        if (text == null) return;

        // Hiển thị dạng: "+50 Coins" hoặc tùy bạn chỉnh sửa chuỗi chữ
        text.text = $"+{coinAmount} Xu";

    }
}
