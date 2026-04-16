using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForgotButton : ButtonBase
{
    FormHander hander;


    protected override void OnEnable()
    {
        base.OnEnable();

    }
    protected override void Start()
    {
        base.Start();
       
        hander = UIManager.Instance.uiFormCanvas.GetComponent<FormHander>();

    }
    public override void OnClick()
    {
        Debug.Log("hiên thi thực hiện");
        UIManager.Instance.TitlleFormGame = StringManager.titlleForgot;
        hander?.ForgotPassword();
    }
}
