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

    private static int coin = 0;
    public int Coin { get => coin; set => coin = value; }
    public int CurrentLevel { get; set; }

    private static bool isGameWin = false;
    public bool IsGameWin { get => isGameWin; set => isGameWin = value; }
    private static bool isGameOver = false;
    public bool IsGameOver { set => isGameOver = value; get => isGameOver; }

    private static bool isPaused = false;
    public bool IsPaused { get => isPaused; set => isPaused = value; }

    private static bool isOnlineMode = false;
    public bool IsOnlineMode { get => isOnlineMode; set => isOnlineMode = value; }

    public Action OnChangedStatusGame { get; set; }


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
        IsGameWin = false;
        if (SceneManager.GetActiveScene().name == SceneType.GAMEOFFLINE.ToString() || SceneManager.GetActiveScene().name == SceneType.GAMEONLINE.ToString())
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
        if (!paused)  // kiểm tra xem nêu chưa pause thi thực hiện pause
        {
            Time.timeScale = 0f;
            pausePanel.SetActive(true);

            isPaused = true;
        }
        else
        {
            Time.timeScale = 1f;
            pausePanel.SetActive(false);

            isPaused = false;
        }
    }
    public void GameOver()
    {
        UIManager.Instance.StatusKeyGameStr = "gameOver.Txt";
        isGameOver = true;
        if (IsOnlineMode)
        {
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
            if (PlayFabDataManager.Instance.playerData.highestLevel == CurrentLevel)
            {
                PlayFabDataManager.Instance.playerData.highestLevel += 1;
            }
            UIManager.Instance.uiOnlinePlayGameCanvas.transform.GetChild(2).GetChild(1).gameObject.SetActive(true);
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

}
