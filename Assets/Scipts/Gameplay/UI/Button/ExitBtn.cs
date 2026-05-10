using UnityEngine;

public class ExitBtn : ButtonBase
{
    public override void OnClick()
    {
        Time.timeScale = 1f;
        GameManager.Instance.IsPaused = false;

        // Nếu là Online, có thể bạn muốn lưu lại tiền/điểm lần cuối trước khi thoát
        if (GameManager.Instance.IsOnlineMode && PlayFabDataManager.Instance.playerData != null)
        {
            PlayFabDataManager.Instance.SavePlayerData();
        }

        UIManager.SceneType sceneType = GameManager.Instance.IsOnlineMode ?
            UIManager.SceneType.ONLINEMAINMENU : UIManager.SceneType.MAINMENU;

        UIManager.Instance.ChangeScene(sceneType);
    }
}