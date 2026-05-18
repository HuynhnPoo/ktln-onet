using System.Collections;
using UnityEngine;

public class LevelPlayedTxt : TextBase
{
    protected override void PrintText()
    {
        if (PlayFabDataManager.Instance.playerData.playerName !="")
        {
            int currentLevel = PlayFabDataManager.Instance.playerData.highestLevel;
            text.SetText(currentLevel.ToString());
        }
        else
        {

            int currentLevel = PlayerPrefs.GetInt(StringManager.levelReached, 0);
            text.SetText(currentLevel.ToString());
        }
    }


}