using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingUIGameBtn : ButtonBase
{
    private static bool isOpenSettingPanel=true;
    [SerializeField] private GameObject pausePanel; 

    public override void OnClick()
    {
        if (isOpenSettingPanel)
        {

            // GameObject objBtn = UIManager.Instance.uiCenterMainMenuCanvas.transform.GetChild(1).gameObject;
            pausePanel.SetActive(true);
            isOpenSettingPanel = false;
        }
        else {
            // GameObject objBtn = UIManager.Instance.uiCenterMainMenuCanvas.transform.GetChild(1).gameObject;
            pausePanel.SetActive(false);
            isOpenSettingPanel = true;
        }
    }

}
