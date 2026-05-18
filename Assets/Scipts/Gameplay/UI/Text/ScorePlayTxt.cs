using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePlayTxt : TextBase
{
    [SerializeField] private bool isHighScore = false;
    protected override void PrintText()
    {

        if (!isHighScore)
            text.SetText(GameManager.Instance.Score.ToString("D4"));

        else
        {
            if (PlayFabDataManager.Instance.playerData.playerName != "")
            {
                Debug.Log("thư hien cho online" + PlayFabDataManager.Instance.playerData.playerName);
                text.SetText(GameManager.Instance.HighScoreOnline.ToString("D4"));
            }
            else
            {
                Debug.Log("thư hien cho "+ isHighScore);
                text.SetText(GameManager.Instance.HighScore.ToString("D4"));
            }

        }
    }


}
