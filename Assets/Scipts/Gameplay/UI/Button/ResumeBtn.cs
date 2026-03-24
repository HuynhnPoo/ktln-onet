using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResumeBtn : ButtonBase
{
    public override void OnClick()
    {
        GameManager.Instance.Pausing(GameManager.Instance.IsPaused);
    }

    
}
