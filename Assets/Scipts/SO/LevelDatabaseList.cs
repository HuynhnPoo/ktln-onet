using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StorangeLevelDatabase", menuName = "Level/List Level", order = 0)]
public class LevelDatabaseList : ScriptableObject
{
    [Header("All Levels")]
    public List<LevelData> levels = new List<LevelData>();

    public LevelData GetLevelLayout(int levelNumber)
    {
        foreach (LevelData level in levels)
        {
            if (level.levelID == levelNumber)
            {

                Debug.Log(level + "hien thi lvel " + level.levelName);
                return level;
            }
        }

        Debug.LogWarning($"Level {levelNumber} not found! Returning default level.");
        return levels.Count > 0 ? levels[0] : null;
    }

    public int GetTotalLevels()
    {
        return levels.Count;
    }
}