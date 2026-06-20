using UnityEngine;
using UnityEngine.SceneManagement;

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
                PlayFabDataManager.Instance.SaveLeaderboard();
            }

            // Tắt UI Win/Lose của Online (Kiểm tra lại GetChild của bạn)
            UIManager.Instance.uiOnlinePlayGameCanvas.transform.GetChild(2).GetChild(1).gameObject.SetActive(false);
            UIManager.Instance.uiOnlinePlayGameCanvas.transform.GetChild(0).GetChild(1).gameObject.SetActive(true); // hien thi pause
        }
        else
        {
            // Lưu Offline
            PlayerPrefs.SetInt(StringManager.levelReached, nextLevel);

            // Tắt UI Win/Lose của Offline
            UIManager.Instance.uiCenterGameoffCanvas.transform.GetChild(1).gameObject.SetActive(false);
        }

        int tempLevel = 0;
        if (nextLevel > 31)  // nếu trên 31 level sẽ chuyển sang ramdom level
        {
            tempLevel = Random.Range(10, 31);
            Debug.Log("hien thi thực hiên temp");
            GameManager.Instance.gridManager.LevelManagerGame.LoadCurrentLevel(tempLevel);
        }
        else
        {
            // 3. Load màn mới
            Debug.Log("hien thi thực hiên chưa tới nextlevel");
            GameManager.Instance.gridManager.LevelManagerGame.LoadCurrentLevel(nextLevel);
        }

    }
}