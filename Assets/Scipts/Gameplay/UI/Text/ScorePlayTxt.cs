using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePlayTxt : TextBase
{
    [SerializeField] private bool isHighScore = false;
    protected override void PrintText()
    {
        if(!isHighScore)
        text.SetText(GameManager.Instance.Score.ToString("D4"));

        else text.SetText(GameManager.Instance.HighScore.ToString("D4"));
    }

   
}
