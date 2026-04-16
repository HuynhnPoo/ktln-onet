using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : SingletonBase<UIManager>
{

    private static bool isLogin = true;
    public bool IsLogin { get => isLogin; set => isLogin = value; }


    public static string titlleFormGame = "";
    public Action<string> OnTitlleChanged;
    public string TitlleFormGame 
    {
        get => titlleFormGame;
        set
        {
            titlleFormGame = value;
            OnTitlleChanged?.Invoke(titlleFormGame);
        }

    }

    public static string statusKeyGameStr = ""; //khi game win hay over
    public string StatusKeyGameStr
    {
        get => statusKeyGameStr;
        set
        {
            statusKeyGameStr = value;
            GameManager.Instance.OnChangedStatusGame.Invoke();
        }
    }
    public string DisplayNameUI { get; set; } = "null";

    public Action<string> OnNotificationChanged;

    private string keyNotificationTxt = "";
    public string KeyNotificationTxt
    {
        get => keyNotificationTxt;
        set
        {
            keyNotificationTxt = value;
            OnNotificationChanged?.Invoke(keyNotificationTxt);
        }
    }
    public enum SceneType
    {

        MAINMENU,
        LEVELMANAGER,
        ONLINEMAINMENU,
        MATCHINGONLINE,
        FORM,
        GAMEOFFLINE,
        GAMEONLINE
    }

    [SerializeField] public GameObject uiCenterMainMenuCanvas { get; private set; }
    [SerializeField] public GameObject uiCenterMainMenuOnlineCanvas { get; private set; }
    [SerializeField] public GameObject uiCenterGameoffCanvas { get; private set; }
    [SerializeField] public GameObject uiFormCanvas { get; private set; }
    [SerializeField] public GameObject uiOnlinePlayGameCanvas { get; private set; }


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {
        if (scene.name != "BOOTSTRAP")
        {
            Init();
        }
    }

    private void Init()
    {
        if (SceneManager.GetActiveScene().name == SceneType.MAINMENU.ToString())
        {
            this.uiCenterMainMenuCanvas = FindGameObjectByNameHide.FindGameObjectByName("center");
            // Debug.Log(uiCenterCanvas);
        }
        else if (SceneManager.GetActiveScene().name == SceneType.ONLINEMAINMENU.ToString())
        {
            this.uiCenterMainMenuOnlineCanvas = FindGameObjectByNameHide.FindGameObjectByName("Canvas_online");
            // Debug.Log(uiCenterCanvas);
        }
        else if (SceneManager.GetActiveScene().name == SceneType.GAMEOFFLINE.ToString())
        {
            this.uiCenterGameoffCanvas = FindGameObjectByNameHide.FindGameObjectByName("UI-Center");
            // Debug.Log(uiCenterCanvas);
        }
        else if (SceneManager.GetActiveScene().name == SceneType.FORM.ToString())
        {
            this.uiFormCanvas = FindGameObjectByNameHide.FindGameObjectByName(StringManager.formCanvas);
            //   Debug.Log(this.uiFormCanvas);
            // Debug.Log(uiCenterCanvas);
        }

        else if (SceneManager.GetActiveScene().name == SceneType.GAMEONLINE.ToString())
        {
            this.uiOnlinePlayGameCanvas = FindGameObjectByNameHide.FindGameObjectByName("Online_Front_Canvas");
            //   Debug.Log(this.uiFormCanvas);
            // Debug.Log(uiCenterCanvas);
        }
    }

    public AsyncOperation ChangeScene(SceneType scene)
    {
        return SceneManager.LoadSceneAsync(scene.ToString());
    }

}
