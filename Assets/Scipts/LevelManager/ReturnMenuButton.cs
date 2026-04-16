using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnMenuButton : ButtonBase
{
    public override void OnClick()
    {
        UIManager.Instance.ChangeScene(UIManager.SceneType.MAINMENU);
    }

   
}
