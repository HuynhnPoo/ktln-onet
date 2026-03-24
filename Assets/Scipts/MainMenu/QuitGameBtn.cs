using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class QuitGameBtn : ButtonBase
{
    public override void OnClick()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit(); // original code to quit Unity 
#endif
    }

}
