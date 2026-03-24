using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetGameBtn : ButtonBase
{
    public override void OnClick()
    {
        PlayerPrefs.DeleteKey(StringManager.hasPlayed);
        GameObject objBtn = UIManager.Instance.uiCenterMainMenuCanvas.transform.GetChild(0).GetChild(1).gameObject;
        objBtn.SetActive(false);
        GameObject objBtnSetting = UIManager.Instance.uiCenterMainMenuCanvas.transform.GetChild(1).gameObject;
        Debug.Log(objBtnSetting);
        objBtnSetting.SetActive(false);


    }


}
