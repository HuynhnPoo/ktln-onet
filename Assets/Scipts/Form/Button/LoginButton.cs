using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginButton : ButtonBase
{
    FormHander hander;


    public override void OnClick()
    {
       
        if (UIManager.Instance.IsLogin)
        {
            UIManager.Instance.TitlleFormGame = StringManager.titlleLogin;
            hander = UIManager.Instance.uiFormCanvas.GetComponent<FormHander>();
            hander?.Login();
            
        }
        else
        {
          //  Debug.LogWarning("aaaa");
            UIManager.Instance.IsLogin = true;
           
            UIManager.Instance.uiFormCanvas.transform.GetChild(0).GetChild(0).GetChild(2).gameObject.SetActive(false);
        }
    }
}
