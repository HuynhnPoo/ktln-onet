using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField]private LevelDatabaseList allLevel;
    private LevelData currentLevelData;

    private bool[,] levelLayout;

    int height,width;

    private void Start()
    {
        LoadCurrentLevel(1);
    }
    public void LoadCurrentLevel(int currentLevel)
    {
        if (allLevel == null)
        {
            Debug.LogError("all level chưa được gắn");
            return;
        }

        currentLevelData = allLevel.GetLevelLayout(currentLevel);

        if (currentLevelData != null)
        {
            // Đảm bảo dữ liệu cell không bị rỗng trước khi vẽ
            currentLevelData.EnsureGridSize();

            // Gọi GridManager để vẽ
            // Giả sử bạn có tham chiếu tới GridManager qua GameManager hoặc kéo trực tiếp
            GridManager grid = GameManager.Instance.gridManager;
            GameManager.Instance.gridManager.SpawnGridFromLevel(currentLevelData);
        }

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
