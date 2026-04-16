using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterButton : ButtonBase
{
    FormHander hander;

    public override void OnClick()
    {
          UIManager.Instance.TitlleFormGame = StringManager.titlleRegister;
        if (!UIManager.Instance.IsLogin)
        {
            Debug.Log(UIManager.Instance.IsLogin);
            hander = UIManager.Instance.uiFormCanvas.GetComponent<FormHander>();
            hander?.Register();
            UIManager.Instance.IsLogin = false;
        }
        else
        {
            UIManager.Instance.IsLogin = false;
            UIManager.Instance.uiFormCanvas.transform.GetChild(0).GetChild(0).GetChild(2).gameObject.SetActive(true);

        }
    }
}
