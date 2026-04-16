using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayGameBtn : ButtonBase
{
   // [SerializeField] private GameObject buttonLastPlayGame;

    public override void OnClick()
    {
        bool hasPlayed = PlayerPrefs.GetInt(StringManager.hasPlayed, 0) == 1;
        if (!hasPlayed)
        {
            PlayerPrefs.SetInt(StringManager.hasPlayed, 1);
            PlayerPrefs.SetInt(StringManager.levelReached, 0);
            PlayerPrefs.Save();
            UIManager.Instance.ChangeScene(UIManager.SceneType.GAMEOFFLINE);
        }
        else
        {
           GameObject objBtn = UIManager.Instance.uiCenterMainMenuCanvas.transform.GetChild(0).GetChild(1).gameObject;
           // Debug.Log(objBtn);
            objBtn.SetActive(true);
            PlayerPrefs.SetInt(StringManager.hasPlayed, 0);
            PlayerPrefs.Save();
            UIManager.Instance.ChangeScene(UIManager.SceneType.GAMEOFFLINE);
        }
    }
}
