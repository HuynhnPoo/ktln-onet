using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UIManager;

public class GameManager : SingletonBase<GameManager>
{

    [SerializeField] public GridManager gridManager { private set; get; } // để quản lí grid manager cua gameplay

    private int score = 0;
    public int Score { get => score; set => score = value; }
    
    private int coin = 0;
    public int Coin { get => coin; set => coin = value; }
    public int CurrentLevel { get; set; }

    private static bool isGameWin = false;
    public bool IsGameWin { get => isGameWin; set => isGameWin = value; }
    private static bool isGameOver { set; get; } = false;

    private static bool isPaused = false;
    public bool IsPaused { get => isPaused; set => isPaused = value; }

    public string Notification { get; set; } = "null";
    public string StatusGameStr { get; set; } = "null";


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
        if (SceneManager.GetActiveScene().name == SceneType.GAMEOFFLINE.ToString())
        {
            //GameObject obj = 
            //Debug.Log(obj);
            this.gridManager =FindAnyObjectByType<GridManager>();
            Debug.Log(this.gridManager);
            // Debug.Log(uiCenterCanvas);
        }
    }

    public void Pausing(bool paused)
    {
        GameObject obj = UIManager.Instance.uiCenterGameoffCanvas.transform.GetChild(0).gameObject;
        if (!paused)  // kiểm tra xem nêu chưa pause thi thực hiện pause
        {
            Time.timeScale = 0f;
            obj.SetActive(true);
         
            isPaused = true;
        }
        else
        {
            Time.timeScale = 1f;
            obj.SetActive(false);

            isPaused = false;
        }
    }

    public void GameOver()
    {
        StatusGameStr = " Game Over !";
        Debug.Log("bạn đã thua");
       GameObject obj= UIManager.Instance.uiCenterGameoffCanvas.transform.GetChild(1).gameObject;// panel game oveer được bật
        GameObject nextLevelButton = obj.transform.GetChild(0).GetChild(2).gameObject;
        obj.SetActive(true);
        nextLevelButton.SetActive(false);


        Time.timeScale = 0f; // tạm dùng time
    }
    public void GameWon()
    {
        StatusGameStr = " Game Won !";
        UIManager.Instance.uiCenterGameoffCanvas.transform.GetChild(1).gameObject.SetActive(true);
        Time.timeScale = 0f;
        Debug.Log("bạn đã chiến thắng");
    }

}
