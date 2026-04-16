using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineMatchButton : ButtonBase
{
    public override void OnClick()
    {
        PhotonManager.Instance.StartMatchmaking();
        //PhotonManager.Instance.CreateCustomRoom();
        UIManager.Instance.ChangeScene(UIManager.SceneType.MATCHINGONLINE);
    }    

}