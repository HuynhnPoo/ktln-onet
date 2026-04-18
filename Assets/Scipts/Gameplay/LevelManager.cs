using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelManager : MonoBehaviour, ICompoment
{
    [SerializeField] private LevelDatabaseList allLevel;
    private LevelData currentLevelData;

    private bool[,] levelLayout;

    int height, width;


    public bool isOnlineMode;
    private void Awake()
    {
        LoadCompoment();
        
    }
    private void Start()
    {
        // LoadCurrentLevel(0);
        CofnirmStatusGame();
    }

   public void CofnirmStatusGame()
    {
        int levelToLoad = 0;

        if (isOnlineMode && PlayFabDataManager.Instance.playerData != null)
        {
            // Nếu Online: Lấy level cao nhất hiện tại của người dùng
            levelToLoad = PlayFabDataManager.Instance.playerData.highestLevel - 1;
        }
        else
        {
            // Nếu Offline: Lấy từ GameManager (thường là từ Local Save hoặc biến tạm)
            levelToLoad = GameManager.Instance.CurrentLevel;
        
        }

        Debug.Log(levelToLoad);

        LoadCurrentLevel(9); // nhớ set id trong SO
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
            GameMechanics.Init((int)currentLevelData.timeLimit, currentLevelData.scorePerNormalMatch,currentLevelData.gravityType);
            // Đảm bảo dữ liệu cell không bị rỗng trước khi vẽ
            currentLevelData.EnsureGridSize();


            // Gọi GridManager để vẽ
            // Giả sử bạn có tham chiếu tới GridManager qua GameManager hoặc kéo trực tiếp
            GridManager grid = GameManager.Instance.gridManager;
            Debug.Log("grid :"+ grid);
            GameManager.Instance.gridManager.SpawnGridFromLevel(currentLevelData);
        }

    }

    public void LoadCompoment()
    {
        if (allLevel == null)
            allLevel = Resources.Load<LevelDatabaseList>("SO/StorangeLevelDatabase_1");
        Debug.Log(isOnlineMode);
        GameManager.Instance.IsOnlineMode = isOnlineMode;


    }

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
