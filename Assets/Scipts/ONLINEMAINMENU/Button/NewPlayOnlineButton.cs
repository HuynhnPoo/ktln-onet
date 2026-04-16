using UnityEngine;

public class NewPlayOnlineButton : ButtonBase
{
    [SerializeField] private GameObject menuLevelButton;

    protected override void Start()
    {
        base.Start();

        // Tìm button nếu chưa kéo vào Inspector
        if (menuLevelButton == null)
        {
            menuLevelButton = UIManager.Instance.uiCenterMainMenuOnlineCanvas.transform.GetChild(1).GetChild(0).GetChild(1).gameObject;
        }

        // KIỂM TRA BAN ĐẦU: Chỉ hiện nếu level cao nhất > 0 (nghĩa là đã từng chơi qua)
        if (PlayFabDataManager.Instance.playerData != null)
        {
            int highest = PlayFabDataManager.Instance.playerData.highestLevel;
            menuLevelButton.SetActive(highest > 0);
        }
        else
        {
            menuLevelButton.SetActive(false); // Mặc định ẩn nếu chưa có dữ liệu
        }
    }

    public override void OnClick()
    {
        if (PlayFabDataManager.Instance.playerData != null)
        {
            // RESET dũ liệu về ban đầu khi chơi mới
            PlayFabDataManager.Instance.playerData.highestLevel = 0;
            PlayFabDataManager.Instance.SavePlayerData();
        }

        // Khi bấm New Play thì phải ẨN nút Menu Level đi (vì đã reset về level 0)
        if (menuLevelButton != null)
        {
            menuLevelButton.SetActive(false);
        }

        // Reset level hiện tại về 0 để vào game từ đầu
        GameManager.Instance.CurrentLevel = 0;

        Debug.Log("Reset data and starting New Game...");
        UIManager.Instance.ChangeScene(UIManager.SceneType.GAMEONLINE);
    }
}