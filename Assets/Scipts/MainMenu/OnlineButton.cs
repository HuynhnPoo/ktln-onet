using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OnlineButton : ButtonBase
{
    public override void OnClick()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable) // kiểm tra internet 
        {
            Debug.Log("hien thi có mạng");
            UIManager.Instance.ChangeScene(UIManager.SceneType.FORM);
        }
        else
        {
            Debug.LogWarning("Hiện tại bạn chưa có internet");

            StartCoroutine(AcitveNotiInternet()); // hiện noti khi chưa có internet 
            return;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) 
        {
            StartCoroutine(AcitveNotiInternet());
            return;
        }
    }
    IEnumerator AcitveNotiInternet()
    {
        // 1. Lấy tham chiếu
        GameObject notiObject = UIManager.Instance.uiCenterMainMenuCanvas.transform.parent.GetChild(2).GetChild(0).gameObject;
        RectTransform rectTransform = notiObject.GetComponent<RectTransform>();
        RectTransform canvasRect = UIManager.Instance.uiCenterMainMenuCanvas.transform.parent.GetComponent<RectTransform>();

        // 2. Tính toán vị trí
        float exitY = canvasRect.rect.height + rectTransform.rect.height;
        Vector2 hidePos = new Vector2(0, exitY);
        Vector2 showPos = Vector2.zero;


        // 3. THỰC HIỆN HIỆN PANEL (Dùng MoveUI)
        notiObject.SetActive(true);
        rectTransform.anchoredPosition = showPos; // Đảm bảo vị trí đích là (0,0)
        rectTransform.MoveUI(hidePos, 1f, Ease.OutBack);

        yield return new WaitForSeconds(1.5f);

        // 4. THỰC HIỆN ẨN PANEL (Dùng MoveFromUI)
        rectTransform.MoveFromUI(showPos, hidePos, 0.8f, Ease.InBack)
            .OnComplete(() =>
            {
                notiObject.SetActive(false);
            });
    }

}
