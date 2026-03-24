using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseBtn : ButtonBase
{
    public override void OnClick()
    {
        GameManager.Instance.Pausing(GameManager.Instance.IsPaused);
    }

}
