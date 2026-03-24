using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class ChangeLanguageBtn : ButtonBase
{
    int index = 1;
    public override void OnClick()
    {
        if (index == 1)
        {

            StartCoroutine(SetLocalCoroutine());
            index = 0; // vị trí ngonon ngữ tiếng anh
        }
        else if (index == 0)
        {
            StartCoroutine(SetLocalCoroutine());
            index = 1; // vị trí ngôn ngữ tiếng việt
        }

    }

    IEnumerator SetLocalCoroutine()
    {
        yield return LocalizationSettings.InitializationOperation;

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index]; // đung để chuyển ngôn ngữ

    }

}
