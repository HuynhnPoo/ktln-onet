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

    private void OnEnable()
    {
     
    }

    private void Start()
    {
        LoadCompoment();
       // LoadCurrentLevel(GameManager.Instance.CurrentLevel);
        LoadCurrentLevel(0);
    }
    public void LoadCurrentLevel(int currentLevel)
    {
        Debug.Log(allLevel);
        if (allLevel == null)
        {
            Debug.LogError("all level chưa được gắn");
            return;
        }

        currentLevelData = allLevel.GetLevelLayout(currentLevel);
        // Debug.Log("level hiện tại là"+ currentLevelData);

        if (currentLevelData != null)
        {
            GameMechanics.Init((int)currentLevelData.timeLimit);
            // Đảm bảo dữ liệu cell không bị rỗng trước khi vẽ
            currentLevelData.EnsureGridSize();

            // Gọi GridManager để vẽ
            // Giả sử bạn có tham chiếu tới GridManager qua GameManager hoặc kéo trực tiếp
            GridManager grid = GameManager.Instance.gridManager;
            GameManager.Instance.gridManager.SpawnGridFromLevel(currentLevelData);
        }

    }

    public void LoadCompoment()
    {
        if (allLevel == null)
            allLevel = Resources.Load<LevelDatabaseList>("SO/StorangeLevelDatabase_1");

        Debug.Log(allLevel);
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
