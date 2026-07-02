using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ReviveButton : ButtonBase
{
    int value = 1;
    [SerializeField] private BoostBase boostAsset;
    public override void OnClick()
    {


        if (GameManager.Instance.IsRevive)
        {
            UIManager.Instance.uiOnlinePlayGameCanvas.transform.GetChild(2).GetChild(1).gameObject.SetActive(false);
            GameManager.Instance.IsGameOver = false;
            Debug.Log("thuc hien khi match khonog đươc");
            boostAsset.Use(GameManager.Instance.gridManager);
            GameManager.Instance.IsRevive = false;
            Time.timeScale = 1;
        }
        else
        {
            GameMechanics.AddTime((int)GameMechanics.GetMaxTime() / 3); // sẽ hôi sinh với 1/3 tg
            UIManager.Instance.uiOnlinePlayGameCanvas.transform.GetChild(2).GetChild(1).gameObject.SetActive(false);

            GameManager.Instance.IsGameOver = false;
            //   boostAsset.Use(GameManager.Instance.gridManager);
            Debug.Log("thuc hien khi hết thời gian");
            Time.timeScale = 1;
        }


        value++;
        GameManager.Instance.ValueRevive = GameManager.Instance.AddValueRevive(value); // them tien
        Debug.Log(value + GameManager.Instance.ValueRevive);
        
        UIManager.Instance.uiOnlinePlayGameCanvas.transform.GetChild(0).GetChild(1).gameObject.SetActive(true); // ui setting
    }


}
