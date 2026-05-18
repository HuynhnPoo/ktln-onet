using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuitType
{
    None,
    MenuMainOff,
    MenuMainOn
}
public class ReturnMenuButton : ButtonBase
{
    public QuitType typeQuit = QuitType.None;
    public override void OnClick()
    {
        switch (typeQuit)
        {
            case QuitType.MenuMainOff:
                UIManager.Instance.ChangeScene(UIManager.SceneType.MAINMENU);
                break;  
            case QuitType.MenuMainOn:
                UIManager.Instance.ChangeScene(UIManager.SceneType.ONLINEMAINMENU);
                break;
        }


    }


}
