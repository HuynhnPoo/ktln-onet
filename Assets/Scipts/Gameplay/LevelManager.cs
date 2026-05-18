using JetBrains.Annotations;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelManager : MonoBehaviour, ICompoment
{
    [SerializeField] private LevelDatabaseList allLevel;
    private LevelData currentLevelData;
    public static Action<int> OnRequestLoadLevel;

    private bool[,] levelLayout;

    int height, width;


    public bool isOnlineMode;
    public bool isPvPMode;
    private void Awake()
    {
        LoadCompoment();

    }
    private void OnEnable()
    {
        OnRequestLoadLevel += HandlePvPLoad;
    }
    private void OnDisable()
    {
        OnRequestLoadLevel -= HandlePvPLoad;
    }
    private void Start()
    {
        // LoadCurrentLevel(0);
        if (!isPvPMode)
            CofnirmStatusGame();
        else
            CheckForRoomLevel();
    }

    public void CofnirmStatusGame()
    {
        int levelToLoad = 0;

        if (isOnlineMode && PlayFabDataManager.Instance.playerData != null)
        {
            // Nếu Online: Lấy level cao nhất hiện tại của người dùng
            levelToLoad = PlayFabDataManager.Instance.playerData.highestLevel;
        }
        else
        {
            // Nếu Offline: Lấy từ GameManager (thường là từ Local Save hoặc biến tạm)
            levelToLoad = GameManager.Instance.CurrentLevel;

        }

        Debug.Log(levelToLoad);

        LoadCurrentLevel(3); // nhớ set id trong SO
    }

    private void CheckForRoomLevel()
    {
        // Nếu đã có LevelID trong CustomProperties của phòng, nạp luôn
        if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("LevelID", out object levelID))
        {
            Debug.Log("LevelManager: Tìm thấy LevelID sẵn trong phòng: " + levelID);
            LoadCurrentLevel((int)levelID);
        }
    }
    public void LoadCurrentLevel(int currentLevel)
    {


        if (allLevel == null)
        {
            Debug.LogError("all level chưa được gắn");
            return;
        }

        currentLevelData = allLevel.GetLevelLayout(currentLevel);
        Debug.Log("level hiện tại là" + currentLevelData + "     " + currentLevel);

        if (currentLevelData != null)
        {
            //   Debug.Log("độ khó là "+ currentLevelData.gravityType);
            GameMechanics.Init((int)currentLevelData.timeLimit, currentLevelData.scorePerNormalMatch, currentLevelData.gravityType);
            // Đảm bảo dữ liệu cell không bị rỗng trước khi vẽ
            currentLevelData.EnsureGridSize();


            // Gọi GridManager để vẽ
            // Giả sử bạn có tham chiếu tới GridManager qua GameManager hoặc kéo trực tiếp
            GridManager grid = GameManager.Instance.gridManager;
            Debug.Log("grid :" + grid);
            GameManager.Instance.gridManager.SpawnGridFromLevel(currentLevelData);
        }

    }

    private void HandlePvPLoad(int levelID)
    {
        if (isPvPMode)
        {
            Debug.Log("PvP Mode: Nhận được Level ID từ Event: " + levelID);
            LoadCurrentLevel(levelID);
        }
    }
    public void LoadCompoment()
    {
        if (allLevel == null)
            allLevel = Resources.Load<LevelDatabaseList>("SO/StorangeLevelDatabase_1");
        Debug.Log(isOnlineMode);
        GameManager.Instance.IsOnlineMode = isOnlineMode;


    }

    //void sy

    /*private void CreateDefaultLayout()
    {
        levelLayout = new bool[grid.Width, grid.Height];
        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                levelLayout[x, y] = true;
            }
        }
        grid.SetLevelLayout(levelLayout);
    }*/
}
