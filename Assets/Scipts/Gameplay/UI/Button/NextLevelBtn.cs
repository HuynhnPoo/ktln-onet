using UnityEngine;

public class NextLevelBtn : ButtonBase
{
    public override void OnClick()
    {
        Time.timeScale = 1;

        // 1. Tăng level
        GameManager.Instance.CurrentLevel++;
        int nextLevel = GameManager.Instance.CurrentLevel;

        // 2. Xử lý lưu trữ
        if (GameManager.Instance.IsOnlineMode)
        {
            if (PlayFabDataManager.Instance.playerData != null)
            {
                // Cập nhật lên PlayFab
                GameMechanics.UpdateHighestLevel(PlayFabDataManager.Instance.playerData, nextLevel);
                PlayFabDataManager.Instance.SavePlayerData();
            }

            // Tắt UI Win/Lose của Online (Kiểm tra lại GetChild của bạn)
            UIManager.Instance.uiOnlinePlayGameCanvas.transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            // Lưu Offline
            PlayerPrefs.SetInt(StringManager.hasPlayed, nextLevel);

            // Tắt UI Win/Lose của Offline
            UIManager.Instance.uiCenterGameoffCanvas.transform.GetChild(1).gameObject.SetActive(false);
        }

        // 3. Load màn mới
        // Lưu ý: Đã tăng CurrentLevel ở trên rồi thì truyền trực tiếp nextLevel vào thôi
        GameManager.Instance.gridManager.LevelManagerGame.LoadCurrentLevel(nextLevel);
    }
}