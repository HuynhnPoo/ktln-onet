using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingUIGameBtn : ButtonBase
{
    private static bool isOpenSettingPanel = true;
    [SerializeField] private GameObject pausePanel;

    public override void OnClick()
    {

        RectTransform rect = pausePanel.GetComponent<RectTransform>();
        if (isOpenSettingPanel)
        {
            // GameObject objBtn = UIManager.Instance.uiCenterMainMenuCanvas.transform.GetChild(1).gameObject;

            pausePanel.SetActive(true);

            rect.ShowPopup(1);
            isOpenSettingPanel = false;
        }
        else
        {
            // GameObject objBtn = UIManager.Instance.uiCenterMainMenuCanvas.transform.GetChild(1).gameObject;
            rect.HidePopup(1).OnComplete(() =>
            {
                pausePanel.SetActive(false);
                isOpenSettingPanel = true;
            });
        }
    }

}
