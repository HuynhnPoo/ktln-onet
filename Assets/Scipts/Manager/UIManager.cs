using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : SingletonBase<UIManager>
{
    public enum SceneType
    {

        MAINMENU,
        LEVELMANAGER,
        GAMEOFFLINE
    }

    [SerializeField] public GameObject uiCenterMainMenuCanvas { get; private set; }
    [SerializeField] public GameObject uiCenterGameoffCanvas { get; private set; }


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
        }else if (SceneManager.GetActiveScene().name == SceneType.GAMEOFFLINE.ToString())
        {
            this.uiCenterGameoffCanvas = FindGameObjectByNameHide.FindGameObjectByName("UI-Center");
           // Debug.Log(uiCenterCanvas);
        }
    }

    public AsyncOperation ChangeScene(SceneType scene)
    {
        return SceneManager.LoadSceneAsync(scene.ToString());
    }
   
}
