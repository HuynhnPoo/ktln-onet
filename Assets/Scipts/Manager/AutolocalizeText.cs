using UnityEngine;
using TMPro;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using System.Collections;
using UnityEditor;
using static UIManager;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(TextMeshProUGUI))]
[RequireComponent(typeof(LocalizeStringEvent))]
public class AutoLocalizeText : MonoBehaviour
{
    private TextMeshProUGUI textUI;
    private LocalizeStringEvent localizeEvent;

    private void Awake()
    {
        textUI = GetComponent<TextMeshProUGUI>();
        localizeEvent = GetComponent<LocalizeStringEvent>();

        // Xoá listener cũ (nếu có)
        localizeEvent.OnUpdateString.RemoveAllListeners();

        // Tự động bind
        localizeEvent.OnUpdateString.AddListener(UpdateText);
      
    }


    private void Start()
    {
    }
    private void OnEnable()
    {
        // 🔥 Quan trọng: khi object bật lại thì refresh
        if (localizeEvent != null)
        {
            localizeEvent.RefreshString();
        }
        UIManager.Instance.OnNotificationChanged += UpdateNoti;

        UpdateNoti(UIManager.Instance.KeyNotificationTxt);
    }

    private void Update()
    {



        if (SceneManager.GetActiveScene().name == SceneType.GAMEOFFLINE.ToString())
        {
            if (GameManager.Instance.IsGameWin && localizeEvent != null && transform.name == "ResultGameText")
            {
                string tableName = "Menu Labels";
                localizeEvent.StringReference.SetReference(tableName, UIManager.Instance.StatusKeyGameStr);
                localizeEvent.RefreshString();
            }

            else if (GameManager.Instance.IsGameOver && localizeEvent != null && transform.name == "ResultGameText")
            {
                string tableName = "Menu Labels";
                localizeEvent.StringReference.SetReference(tableName, UIManager.Instance.StatusKeyGameStr);
                localizeEvent.RefreshString();
            }

        }

    }


   public void UpdateNoti(string keyNotification)
    {

        if (SceneManager.GetActiveScene().name == SceneType.FORM.ToString() && localizeEvent != null && transform.name == "Notification_Text")
        {
            string tableName = "Menu Labels";


                Debug.Log(UIManager.Instance.KeyNotificationTxt);

            if (UIManager.Instance.KeyNotificationTxt == StringManager.notifail)
            {
                textUI.color = Color.red;

                localizeEvent.StringReference.SetReference(tableName, UIManager.Instance.KeyNotificationTxt);
                localizeEvent.RefreshString();
            }

            else
            {
                textUI.color = Color.green;

                localizeEvent.StringReference.SetReference(tableName, UIManager.Instance.KeyNotificationTxt);
                localizeEvent.RefreshString();
            }


        }
    }


    private void UpdateText(string value)
    {
        textUI.text = value;
    }
}

