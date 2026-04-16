using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelection : MonoBehaviour
{
    [SerializeField] private Button[] buttons;

    private void OnEnable()
    {
        buttons=GetComponentsInChildren<Button>();

        int levelReached = PlayerPrefs.GetInt(StringManager.levelReached, 0);

        for (int i=0;i< buttons.Length;i++)
        {
            if(i> levelReached)
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
