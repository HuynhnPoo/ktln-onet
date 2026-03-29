using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoutButton : ButtonBase
{
    public override void OnClick()
    {
        UIManager.Instance.ChangeScene(UIManager.SceneType.FORM);
    }

   
}
