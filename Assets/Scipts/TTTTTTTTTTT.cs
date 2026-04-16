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

            Debug.Log("hhhhhh"+PlayFabDataManager.Instance.playerData.playerName + " " + PlayFabDataManager.Instance.playerData.highestLevel);
        }
    }
}
