using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelection : MonoBehaviour
{
    [SerializeField] private Button[] buttons;

    private void OnEnable()
    {
        buttons = GetComponentsInChildren<Button>();

        // nêu đang ngập tài khoản sẽ active các lelevl đã chơi online
        if (PlayFabDataManager.Instance.playerData.playerName != "")
        {
            Debug.Log("hien thi ra online"+ GameManager.Instance.IsOnlineMode);
            int levelReached = PlayFabDataManager.Instance.playerData.highestLevel;
            ActiveButtonLevel(levelReached);
        }

        else // active cấc button đã chơi ở chế độ off
        {

            int levelReached = PlayerPrefs.GetInt(StringManager.levelReached, 0);

            ActiveButtonLevel(levelReached);
        }
    }

    void ActiveButtonLevel(int levelReached)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i > levelReached)
            {
                buttons[i].interactable = false;
            }
            else
            {
                buttons[i].interactable = true;
            }
        }
    }
}
