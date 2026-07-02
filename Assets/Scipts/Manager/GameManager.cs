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

    private static int score = 0;
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

    private static bool isRevive = false;
    public bool IsRevive { get => isRevive; set => isRevive = value; }

    public float ValueRevive { get; set; } = 1000;



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
        Score = 0;
        Coin = 0;


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
            SetHighScore();
        }
    }

    void PauseGame(bool paused, GameObject pausePanel)
    {
        RectTransform rect = pausePanel.GetComponent<RectTransform>();
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
        if (!IsOnlineMode)
        {
            SetHighScore(); // thực hiên lưu score
        }

        UIManager.Instance.StatusKeyGameStr = "gameOver.Txt";
        isGameOver = true;
        if (IsOnlineMode)
        {
            PlayFabDataManager.Instance.SavePlayerData();
            PlayFabDataManager.Instance.SaveLeaderboard();

            GameObject obj = UIManager.Instance.uiOnlinePlayGameCanvas.transform.GetChild(2).GetChild(1).gameObject;// panel game oveer được bật
            GameObject nextLevelButton = obj.transform.GetChild(0).GetChild(2).gameObject;
            GameObject reviveBtn = obj.transform.GetChild(0).GetChild(3).gameObject;
            GameObject watchReviveBtn = obj.transform.GetChild(0).GetChild(4).gameObject;

            obj.SetActive(true);
            nextLevelButton.SetActive(false);
            reviveBtn.SetActive(true);
            watchReviveBtn.SetActive(true);

            UIManager.Instance.uiOnlinePlayGameCanvas.transform.GetChild(0).GetChild(1).gameObject.SetActive(false); // ui setting
        }
        else
        {
            GameObject obj = UIManager.Instance.uiCenterGameoffCanvas.transform.GetChild(1).gameObject;// panel game oveer được bật
            GameObject nextLevelButton = obj.transform.GetChild(0).GetChild(2).gameObject;
            obj.SetActive(true);
            nextLevelButton.SetActive(false);
            // SetHighScore();

            UIManager.Instance.uiCenterGameoffCanvas.transform.parent.GetChild(0).GetChild(1).gameObject.SetActive(false); //ui pausebutton
        }

        SoundManager.Instance.PlaySfx("GameOverSFX");

        Time.timeScale = 0f; // tạm dùng time
    }
    public void GameWon()
    {

        if (!IsOnlineMode)
        {
            SetHighScore(); // thực hiên lưu score
        }


        UIManager.Instance.StatusKeyGameStr = "gameWon.Txt";

        isGameWin = true;
        Debug.Log("hien thi" + IsGameOver + " " + isGameWin);

        if (IsOnlineMode)
        {
            PlayFabDataManager.Instance.SavePlayerData();
            PlayFabDataManager.Instance.SaveLeaderboard();
            if (PlayFabDataManager.Instance.playerData.highestLevel == CurrentLevel)
            {
                PlayFabDataManager.Instance.playerData.highestLevel += 1;
            }
            GameObject obj = UIManager.Instance.uiOnlinePlayGameCanvas.transform.GetChild(2).GetChild(1).gameObject;// panel game oveer được bật
            GameObject nextLevelButton = obj.transform.GetChild(0).GetChild(2).gameObject;
            GameObject reviveBtn = obj.transform.GetChild(0).GetChild(3).gameObject;
            GameObject watchReviveBtn = obj.transform.GetChild(0).GetChild(4).gameObject;

            obj.SetActive(true);
            nextLevelButton.SetActive(true);
            reviveBtn.SetActive(false);
            watchReviveBtn.SetActive(false);

            UIManager.Instance.uiOnlinePlayGameCanvas.transform.GetChild(0).GetChild(1).gameObject.SetActive(false); // ui setting
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
            UIManager.Instance.uiCenterGameoffCanvas.transform.parent.GetChild(0).GetChild(1).gameObject.SetActive(false); //ui pausebutton

            // SetHighScore();
        }
        SoundManager.Instance.PlaySfx("GameWinSFX");
        Time.timeScale = 0f;
        Debug.Log("bạn đã chiến thắng");
    }


    public void SetHighScore()
    {
        highScore = PlayerPrefs.GetInt(StringManager.highScoreStr, 0);
        Debug.Log($"[LOG MANAGER] Lúc lưu điểm - Biến score thực tế đang là: {score}");
        Debug.Log($"hien thi  2 sscore khi keets thuc {score} {highScore}");
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt(StringManager.highScoreStr, highScore);
            PlayerPrefs.Save();
        }
    }

    public bool CanIPlay()
    {
        return isMyturn;
    }

    public float AddValueRevive(int value)

    {
        Debug.Log(ValueRevive * value +" "+ value+"  " + ValueRevive);
      return  ValueRevive* value;
    } 


  
}
