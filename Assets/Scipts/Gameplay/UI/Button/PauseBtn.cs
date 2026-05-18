using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseBtn : ButtonBase
{
    public override void OnClick()
    {
        if (GameManager.Instance.IsOnlineMode)
        {
            PlayFabDataManager.Instance.SavePlayerData();
            PlayFabDataManager.Instance.SaveLeaderboard();

            GameManager.Instance.Pausing(GameManager.Instance.IsPaused);
        }
        else
        {

            GameManager.Instance.Pausing(GameManager.Instance.IsPaused);
        }
    }

}
