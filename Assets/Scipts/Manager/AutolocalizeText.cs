using UnityEngine;
using TMPro;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using System.Collections;

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

    private void OnEnable()
    {
        // 🔥 Quan trọng: khi object bật lại thì refresh
        if (localizeEvent != null)
        {
            localizeEvent.RefreshString();
        }
    }

    private void UpdateText(string value)
    {
        textUI.text = value;
    }
}

