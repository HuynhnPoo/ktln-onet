using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : ButtonBase
{
    [SerializeField] private int currentLevel;

    public override void OnClick()
    {
        GameManager.Instance.CurrentLevel = currentLevel;

       
        UIManager.Instance.ChangeScene(UIManager.SceneType.GAMEOFFLINE);
    }
}
