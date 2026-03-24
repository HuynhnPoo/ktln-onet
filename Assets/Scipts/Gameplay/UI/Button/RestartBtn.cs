using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartBtn : ButtonBase
{
    public override void OnClick()
    {
        Time.timeScale = 1; // chuyển cho game chạy được

        GameManager.Instance.IsPaused = false;
        UIManager.Instance.ChangeScene(UIManager.SceneType.GAMEOFFLINE);

    }
}
