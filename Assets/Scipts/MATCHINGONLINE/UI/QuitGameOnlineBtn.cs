using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitGameOnlineBtn : ButtonBase
{
    public override void OnClick()
    {
        PhotonManager.Instance.IsPlayingOnline = false;
        UIManager.Instance.uiOnlineMatchPlayGameCanvas.transform.GetChild(1).gameObject.SetActive(true);

        PhotonManager.Instance.OnlineMatchManager.LeaveMatch();
    }

   
}
