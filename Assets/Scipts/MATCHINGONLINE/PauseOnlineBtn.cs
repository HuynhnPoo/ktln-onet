using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseOnlineBtn : ButtonBase
{
    private static bool isPauseGameOnline=false;
    public override void OnClick()
    {
        ChangePause(isPauseGameOnline);
    }

    void ChangePause(bool isPause)
    {
        if (!isPauseGameOnline) 
        {

            Debug.Log(UIManager.Instance.uiOnlineMatchPlayGameCanvas.transform.GetChild(0).gameObject.name);
            UIManager.Instance.uiOnlineMatchPlayGameCanvas.transform.GetChild(0).gameObject.SetActive(true);
  
            isPauseGameOnline =true;
        }
        else
        {
            UIManager.Instance.uiOnlineMatchPlayGameCanvas.transform.GetChild(0).gameObject.SetActive(false);
            isPauseGameOnline = false;
        }
    }

   
}
