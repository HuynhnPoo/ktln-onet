using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitBtn : ButtonBase
{
    public override void OnClick()
    {
        Time.timeScale = 1f;
        GameManager.Instance.IsPaused = false;
        UIManager.Instance.ChangeScene(UIManager.SceneType.MAINMENU);
    }

}
