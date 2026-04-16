using System.Collections;
using UnityEngine.Localization.Settings;

public class ChangeLanguageBtn : ButtonBase
{
    int index = 1; // 1 = Tiếng Việt, 0 = Tiếng Anh

    public override void OnClick()
    {
        // ✅ Truyền index hiện tại vào coroutine TRƯỚC khi đổi
        StartCoroutine(SetLocalCoroutine(index));

        // Đổi index cho lần click tiếp theo
        index = (index == 1) ? 0 : 1;
    }

    IEnumerator SetLocalCoroutine(int localeIndex)
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeIndex];
    }
}