using System.Collections;
using UnityEngine;

public class LevelPlayedTxt : TextBase
{
    protected override void PrintText()
    {
        int currentLevel = PlayerPrefs.GetInt(StringManager.levelReached, 0);
        text.SetText(currentLevel.ToString());
    }

    
}