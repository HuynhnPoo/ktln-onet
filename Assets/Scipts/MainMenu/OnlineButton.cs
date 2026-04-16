using System.Collections;
using System.Collections.Generic;
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

          //  UIManager.Instance.uiCenterMainMenuCanvas.transform.parent.GetChild(2).GetChild(0).gameObject.SetActive(true);
        }
    }

    private void Update()
    {

        if (Input.GetKey(KeyCode.X)) StartCoroutine(AcitveNotiInternet());
    }

    IEnumerator AcitveNotiInternet()
    {
        UIManager.Instance.uiCenterMainMenuCanvas.transform.parent.GetChild(2).GetChild(0).gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        UIManager.Instance.uiCenterMainMenuCanvas.transform.parent.GetChild(2).GetChild(0).gameObject.SetActive(false);
    }

}
