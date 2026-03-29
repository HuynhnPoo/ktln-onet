using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmitDisplayName : ButtonBase
{
    FormHander FormHander;
    public override void OnClick()
    {

        FormHander = UIManager.Instance.uiFormCanvas.GetComponent<FormHander>();
       // Debug.Log(FormHander);
        FormHander?.SubmitDisplayName();
    }

    
}
