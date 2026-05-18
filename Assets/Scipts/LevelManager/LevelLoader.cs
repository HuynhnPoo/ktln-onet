using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : ButtonBase
{
    [SerializeField] private int currentLevel;

    public QuitType typeGame;
    public override void OnClick()
    {
        GameManager.Instance.CurrentLevel = currentLevel;

        switch (typeGame)
        {
            case QuitType.MenuMainOff:
                UIManager.Instance.ChangeScene(UIManager.SceneType.GAMEOFFLINE);
                break;
            case QuitType.MenuMainOn:
                UIManager.Instance.ChangeScene(UIManager.SceneType.GAMEONLINE);
                break;
        }
      
    }
}
