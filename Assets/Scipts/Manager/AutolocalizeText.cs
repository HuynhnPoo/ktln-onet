using UnityEngine;
using TMPro;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using System.Collections;
using static UIManager;
using System;

[RequireComponent(typeof(TextMeshProUGUI))]
[RequireComponent(typeof(LocalizeStringEvent))]
public class AutoLocalizeText : MonoBehaviour
{
    private TextMeshProUGUI textUI;
    private LocalizeStringEvent localizeEvent;
    private const string TABLE_NAME = "Menu Labels";

    private void Awake()
    {
        textUI = GetComponent<TextMeshProUGUI>();
        localizeEvent = GetComponent<LocalizeStringEvent>();
    }

    private void OnEnable()
    {
        localizeEvent.OnUpdateString.RemoveAllListeners();
        localizeEvent.OnUpdateString.AddListener(UpdateText);

        if (UIManager.Instance != null)
            UIManager.Instance.OnNotificationChanged += UpdateNotiForm;

        if (GameManager.Instance != null)
            GameManager.Instance.OnChangedStatusGame += StatusResultsGame;

        // ✅ Dùng Coroutine để đợi Localization sẵn sàng
        StartCoroutine(WaitForLocalizationThenRefresh());
    }

    private void OnDisable()
    {
        if (UIManager.Instance != null)
            UIManager.Instance.OnNotificationChanged -= UpdateNotiForm;
        if (GameManager.Instance != null)
            GameManager.Instance.OnChangedStatusGame -= StatusResultsGame;
    }

    // ✅ Đợi LocalizationSettings init xong rồi mới RefreshString
    private IEnumerator WaitForLocalizationThenRefresh()
    {
        // Đợi hệ thống Localization khởi tạo xong (chỉ cần thiết lần đầu)
        yield return LocalizationSettings.InitializationOperation;

        // Force refresh để lấy đúng ngôn ngữ hiện tại
        localizeEvent.RefreshString();

        // Sau đó mới kiểm tra state UI
        RefreshUIState();
    }

    private void RefreshUIState()
    {
        if (transform.name == "ResultGameText")
            StatusResultsGame();

        if (transform.name == "Notification_Text" && UIManager.Instance != null)
            UpdateNotiForm(UIManager.Instance.KeyNotificationTxt);
        if (transform.name == "Titlle_Text" && UIManager.Instance != null)
            UpdateTitlleForm(UIManager.Instance.TitlleFormGame);
    }

    private void UpdateTitlleForm(string titlleFormGame)
    {
        if (SceneManager.GetActiveScene().name == SceneType.FORM.ToString()
              && localizeEvent != null
              && transform.name == "Titlle_Text")
        {
            localizeEvent.StringReference.SetReference(TABLE_NAME, titlleFormGame);
            localizeEvent.RefreshString();
        }
    }

    public void StatusResultsGame()
    {
        if (SceneManager.GetActiveScene().name == SceneType.GAMEOFFLINE.ToString() && localizeEvent != null)
        {
            if (GameManager.Instance.IsGameWin || GameManager.Instance.IsGameOver)
            {
                Debug.Log($"<color=yellow>Localize:</color> Cập nhật trạng thái cho {transform.name}");
                localizeEvent.StringReference.SetReference(TABLE_NAME, UIManager.Instance.StatusKeyGameStr);
                localizeEvent.RefreshString();
            }
        }
    }


    public void UpdateNotiForm(string keyNotification)
    {
        if (SceneManager.GetActiveScene().name == SceneType.FORM.ToString()
            && localizeEvent != null
            && transform.name == "Notification_Text")
        {
            textUI.color = (keyNotification == StringManager.notifail) ? Color.red : Color.green;
            localizeEvent.StringReference.SetReference(TABLE_NAME, keyNotification);
            localizeEvent.RefreshString();
        }
    }

    private void UpdateText(string value)
    {
        if (textUI != null) textUI.text = value;
    }
}