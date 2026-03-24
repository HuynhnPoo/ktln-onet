using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelBtn : ButtonBase
{
    public override void OnClick()
    {
        Debug.Log(GameManager.Instance.CurrentLevel);
    }

   
}
