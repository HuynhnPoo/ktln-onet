using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartBtn : ButtonBase
{
    public override void OnClick()
    {
        Time.timeScale = 1; // chuyển cho game chạy được


        GameManager.Instance.IsPaused = false;

        // Xác định quay lại scene nào dựa trên mode hiện tại
        UIManager.SceneType targetScene = GameManager.Instance.IsOnlineMode ?
            UIManager.SceneType.GAMEONLINE : UIManager.SceneType.GAMEOFFLINE;
        UIManager.Instance.ChangeScene(targetScene);

    }
}
