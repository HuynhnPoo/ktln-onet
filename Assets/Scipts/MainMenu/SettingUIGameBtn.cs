using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingUIGameBtn : ButtonBase
{
    private static bool isOpenSettingPanel=true;

    public override void OnClick()
    {
        if (isOpenSettingPanel)
        {

        GameObject objBtn = UIManager.Instance.uiCenterMainMenuCanvas.transform.GetChild(1).gameObject;
        objBtn.SetActive(true);
            isOpenSettingPanel = false;
        }
        else {
            GameObject objBtn = UIManager.Instance.uiCenterMainMenuCanvas.transform.GetChild(1).gameObject;
            objBtn.SetActive(false);
            isOpenSettingPanel = true;
        }
    }

}
