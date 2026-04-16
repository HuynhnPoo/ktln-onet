using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeForgotButton : ButtonBase
{
  private static bool hasForgot=false;

    public override void OnClick()
    {
        if (!hasForgot) // tắt ui form login để bật form forgots
        {
            UIManager.Instance.uiFormCanvas.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            UIManager.Instance.uiFormCanvas.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
            hasForgot=true;
        }
        else
        {
            UIManager.Instance.uiFormCanvas.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
            UIManager.Instance.uiFormCanvas.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            hasForgot = false;
        }
      //  throw new System.NotImplementedException();
    }
}
