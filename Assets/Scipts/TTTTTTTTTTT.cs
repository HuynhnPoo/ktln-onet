using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TTTTTTTTTTT : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {

            Debug.Log("hhhhhh" + PlayFabDataManager.Instance.playerData.playerName + " " + PlayFabDataManager.Instance.playerData.highestLevel);
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            PlayerData playerData = PlayFabDataManager.Instance.playerData;
            playerData.gold = 99999999;
            playerData.highestLevel = 10;
            Debug.Log("thhực hien set cho admin "+ playerData.gold +" "+ playerData.highestLevel);
            PlayFabDataManager.Instance.SavePlayerData();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {

        }

    }
}
