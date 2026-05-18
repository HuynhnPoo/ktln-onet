using DG.Tweening;
using System;
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

    private int amountScore = 0;
    public int AmountScore { get => amountScore; set => amountScore = value; }

    private int highScore = 0;
    public int HighScore { get => highScore; set => highScore = value; }

    private int highScoreOnline = 0;
    public int HighScoreOnline { get => highScoreOnline; set => highScoreOnline = value; }

    private int totalCoinOnline = 0;
    public int TotalCoinOnline { get => totalCoinOnline; set => totalCoinOnline = value; } // cho online

    private static int coin = 0;
    public int Coin { get => coin; set => coin = value; } // tính coin tạm
    public int CurrentLevel { get; set; }

    private bool isGameWin = false;
    public bool IsGameWin { get => isGameWin; set => isGameWin = value; }
    private bool isGameOver = false;
    public bool IsGameOver { set => isGameOver = value; get => isGameOver; }

    private static bool isPaused = false;
    public bool IsPaused { get => isPaused; set => isPaused = value; }

    private static bool isOnlineMode = false;
    public bool IsOnlineMode { get => isOnlineMode; set => isOnlineMode = value; }

    public Action OnChangedStatusGame { get; set; }


    private static bool isMyturn;
    public bool IsMyturn { get => isMyturn; set => isMyturn = value; }

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
        LevelManager lv = FindAnyObjectByType<LevelManager>();
        if (lv != null)
        {
            lv.CofnirmStatusGame();
        }
    }


    private void Init()
    {
        isGameOver = false;
        isGameWin = false;
        if (SceneManager.GetActiveScene().name == SceneType.GAMEOFFLINE.ToString()
            || SceneManager.GetActiveScene().name == SceneType.GAMEONLINE.ToString()
            || SceneManager.GetActiveScene().name == SceneType.MATCHINGONLINE.ToString())
        {
            this.gridManager = FindAnyObjectByType<GridManager>();

        }
    }

    public void Pausing(bool paused)
    {

        if (GameManager.Instance.IsOnlineMode) // khi online sẽ bật cái này
        {
            GameObject obj = UIManager.Instance.uiOnlinePlayGameCanvas.transform.GetChild(2).GetChild(0).gameObject;
            PauseGame(paused, obj);

        }
        else
        {

            GameObject obj = UIManager.Instance.uiCenterGameoffCanvas.transform.GetChild(0).gameObject;
            PauseGame(paused, obj);
        }
    }

    void PauseGame(bool paused, GameObject pausePanel)
    {
        RectTransform rect= pausePanel.GetComponent<RectTransform>();
        if (!paused)  // kiểm tra xem nêu chưa pause thi thực hiện pause
        {
            Time.timeScale = 0f;
            pausePanel.SetActive(true);
            rect.ShowPopup(1);
            isPaused = true;
        }
        else
        {
            rect.HidePopup(1).OnComplete(() =>
            {
                Time.timeScale = 1f;
                pausePanel.SetActive(false);

                isPaused = false;
            });
           
        }
    }
    public void GameOver()
    {
        UIManager.Instance.StatusKeyGameStr = "gameOver.Txt";
        isGameOver = true;
        if (IsOnlineMode)
        {
            PlayFabDataManager.Instance.SavePlayerData();
            PlayFabDataManager.Instance.SaveLeaderboard();

            GameObject obj = UIManager.Instance.uiOnlinePlayGameCanvas.transform.GetChild(2).GetChild(1).gameObject;// panel game oveer được bật
            GameObject nextLevelButton = obj.transform.GetChild(0).GetChild(2).gameObject;
            obj.SetActive(true);
            nextLevelButton.SetActive(false);

        }
        else
        {
            GameObject obj = UIManager.Instance.uiCenterGameoffCanvas.transform.GetChild(1).gameObject;// panel game oveer được bật
            GameObject nextLevelButton = obj.transform.GetChild(0).GetChild(2).gameObject;
            obj.SetActive(true);
            nextLevelButton.SetActive(false);
            SetHighScore();

        }

        Time.timeScale = 0f; // tạm dùng time
    }
    public void GameWon()
    {
        UIManager.Instance.StatusKeyGameStr = "gameWon.Txt";

        isGameWin = true;



        if (IsOnlineMode)
        {
            PlayFabDataManager.Instance.SavePlayerData();
            PlayFabDataManager.Instance.SaveLeaderboard();
            if (PlayFabDataManager.Instance.playerData.highestLevel == CurrentLevel)
            {
                PlayFabDataManager.Instance.playerData.highestLevel += 1;
            }
            UIManager.Instance.uiOnlinePlayGameCanvas.transform.GetChild(2).GetChild(1).gameObject.SetActive(true);
            UIManager.Instance.uiOnlinePlayGameCanvas.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            //OnChangedStatusGame?.Invoke();
        }
        else
        {
            int currentLevelIndexToNext = PlayerPrefs.GetInt(StringManager.levelReached, 0);

            if (currentLevelIndexToNext == CurrentLevel)
            {
                currentLevelIndexToNext += 1;
                PlayerPrefs.SetInt(StringManager.levelReached, currentLevelIndexToNext);
                PlayerPrefs.Save();
            }

            UIManager.Instance.uiCenterGameoffCanvas.transform.GetChild(1).gameObject.SetActive(true);
            SetHighScore();
        }
        Time.timeScale = 0f;
        Debug.Log("bạn đã chiến thắng");
    }


    void SetHighScore()
    {
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt(StringManager.highScoreStr, highScore);
        }
    }

    public bool CanIPlay()
    {
        return isMyturn;
    }

}
