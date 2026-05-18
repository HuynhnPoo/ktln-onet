using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResumeGameBtn : ButtonBase
{
    public QuitType typeGame;
    public override void OnClick()
    {
        switch (typeGame)
        {
            case QuitType.MenuMainOff:
                UIManager.Instance.ChangeScene(UIManager.SceneType.LEVELMANAGER);
                break;
            case QuitType.MenuMainOn:
                UIManager.Instance.ChangeScene(UIManager.SceneType.LEVELONLINEMANAGER);
                break;

        }
    }


}
