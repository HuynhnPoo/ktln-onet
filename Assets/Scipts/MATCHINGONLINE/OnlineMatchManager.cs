using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // Nếu dùng UI Text thường
public class OnlineMatchManager : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    public TextMeshProUGUI turnStatusText;


    [Header("Match Settings")]
    public float turnTime = 15f;
    private float currentTurnTimer;
    private float timeGlobal = 120;
    private double startTime;
    private bool matchStarted = false;

    private int currentTurnActorNumber;
    private Player currentTurnPlayer; // Lưu trữ đối tượng Player đang giữ lượt
    public bool isMyTurn;

    public static Action<int> OnMatchScored;   // int = score amount
    public static Action OnMatchMade;
    public static Action<List<Vector2Int>> OnMatchFound;
    public static Action OnResultStatus;


    private int myScore = 0;
    private int opponentScore = 0;


    public TextMeshProUGUI myScoreText;
    public TextMeshProUGUI opponentScoreText;
    public override void OnEnable()
    {
        base.OnEnable();
        OnMatchScored += HandleMatchScored;
        OnMatchMade += ChangeTurn;
        OnMatchFound += HandleMatchFound;
        OnResultStatus += DetermineWinner;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        OnMatchScored -= HandleMatchScored;
        OnMatchMade -= ChangeTurn;
        OnMatchFound -= HandleMatchFound;

        OnResultStatus -= DetermineWinner;


    }

    private void Start()
    {

        currentTurnTimer = turnTime;

        if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("MatchStartTime"))
        {
            startTime = (double)PhotonNetwork.CurrentRoom.CustomProperties["MatchStartTime"];
            Debug.Log("hien thi ra starttime" + startTime);
            // matchStarted = true;
        }
        // Nếu là Master, khởi tạo lượt đầu tiên
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom)
        {
            UpdateTurnInRoomProps(PhotonNetwork.LocalPlayer.ActorNumber);
        }

        // Đợi một chút để dữ liệu phòng đồng bộ rồi mới kiểm tra lượt
        Invoke("UpdateCurrentTurnPlayer", 0.2f);

    }

    void Update()
    {
        if (!PhotonNetwork.InRoom && startTime <= 0) return;
        if (PhotonManager.Instance.IsPlayingOnline)
        {
            double elapsedTime = PhotonNetwork.Time - startTime;
            float remainingTime = timeGlobal - (float)elapsedTime;

            if (remainingTime < 0) remainingTime = 0; // Không cho xuống số âm
        }

        if (isMyTurn)
        {
            currentTurnTimer -= Time.deltaTime;
            if (currentTurnTimer <= 0)
            {
                ChangeTurn();
            }
        }

        UpdateUI();
    }


    private void HandleMatchFound(List<Vector2Int> path)
    {
        // Chuyển List Vector2Int sang mảng Vector2 để Photon hiểu được
        Vector2[] pathArray = new Vector2[path.Count];
        for (int i = 0; i < path.Count; i++)
        {
            pathArray[i] = new Vector2(path[i].x, path[i].y);
        }

        // Gửi RPC cho tất cả mọi người
        photonView.RPC(nameof(RPC_HandleMatch), RpcTarget.All, (object)pathArray);
    }

    [PunRPC]
    public void RPC_HandleMatch(Vector2[] pathArray)
    {
        // Chuyển ngược lại về List Vector2Int để GridManager sử dụng
        List<Vector2Int> path = new List<Vector2Int>();
        foreach (Vector2 v in pathArray)
        {
            path.Add(new Vector2Int((int)v.x, (int)v.y));
        }

        // Tìm GridManager trong Scene và thực hiện xóa
        GridManager gm = GameManager.Instance.gridManager;
        if (gm != null)
        {
            gm.HandleMatch(path);
            if (PhotonNetwork.InRoom && isMyTurn)
            {

                OnMatchScored?.Invoke(10);
                // OnlineMatchManager.OnMatchMade?.Invoke(); // → tự đổi lượt
            }

        }
    }

    public void UpdateCurrentTurnPlayer()
    {
        // Kiểm tra an toàn trước khi truy cập Room
        if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.CustomProperties == null) return;

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("CurrentTurnActor", out object actorNum))
        {
            currentTurnActorNumber = (int)actorNum;

            // Xác định xem mình có đang giữ lượt không
            isMyTurn = (PhotonNetwork.LocalPlayer.ActorNumber == currentTurnActorNumber);

            // --- ĐẨY SANG GAMEMANAGER ---
            if (GameManager.Instance != null)
            {
                GameManager.Instance.IsMyturn = this.isMyTurn;
            }

            // TÌM PLAYER TƯƠNG ỨNG VỚI ID ĐỂ GÁN VÀO currentTurnPlayer
            currentTurnPlayer = null; // Reset trước khi tìm
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (p.ActorNumber == currentTurnActorNumber)
                {
                    currentTurnPlayer = p; // Gán Player tìm được vào đây
                    break;
                }
            }
        }
    }

    private void UpdateUI()
    {
        if (turnStatusText == null) return;

        if (currentTurnPlayer != null)
        {
            string displayName = isMyTurn ? "BẠN" : currentTurnPlayer.NickName;
            turnStatusText.text = $"Lượt của: {displayName} ({currentTurnTimer:F1}s)";
        }
        else
        {
            turnStatusText.text = "Đang đợi đồng bộ lượt...";
        }
    }

    public void ChangeTurn()
    {
        if (!isMyTurn || PhotonNetwork.PlayerList.Length < 2) return;
        currentTurnTimer = turnTime;
        int nextActor = -1;
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
            {
                nextActor = p.ActorNumber;
                break;
            }
        }

        if (nextActor != -1) UpdateTurnInRoomProps(nextActor);
    }

    private void UpdateTurnInRoomProps(int actorNumber)
    {
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props.Add("CurrentTurnActor", actorNumber);
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        // Kiểm tra và cập nhật lượt
        if (propertiesThatChanged.ContainsKey("CurrentTurnActor"))
        {
            UpdateCurrentTurnPlayer();
            currentTurnTimer = turnTime;
        }

        // KIỂM TRA VÀ CẬP NHẬT THỜI GIAN TRẬN ĐẤU
        if (propertiesThatChanged.ContainsKey("MatchStartTime"))
        {
            startTime = (double)propertiesThatChanged["MatchStartTime"];
            Debug.Log("Đã nhận startTime mới từ Server: " + startTime);
        }
    }

    private void HandleMatchScored(int score)
    {
        if (!PhotonNetwork.InRoom) return;
        photonView.RPC(nameof(RPC_AddScore), RpcTarget.All,
            PhotonNetwork.LocalPlayer.ActorNumber, score);
    }

    [PunRPC]
    public void RPC_AddScore(int scorerActorNumber, int amount)
    {
        // Tìm xem người vừa ghi điểm là ai trong danh sách Player
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.ActorNumber == scorerActorNumber)
            {
                if (p.IsLocal)
                {
                    // Nếu chính là máy này ghi điểm
                    myScore += amount;
                    if (myScoreText != null)
                        myScoreText.text = $"Bạn: {myScore}";
                }
                else
                {
                    // Nếu là máy khác ghi điểm (đối thủ)
                    opponentScore += amount;
                    if (opponentScoreText != null)
                        opponentScoreText.text = $"Đối thủ: {opponentScore}";
                }
                break;
            }
        }

        Debug.Log($"Player {scorerActorNumber} ghi điểm. MyScore: {myScore}, Opponent: {opponentScore}");
    }

    public void DetermineWinner()
    {
        PhotonManager.Instance.IsPlayingOnline = false;
        Debug.Log("aaaaaaaaaaaaaaaaaa");
        if (myScore > opponentScore)
        {
            Debug.Log("ddiem so " + myScore + " " + opponentScore + " thắng");

            UIManager.Instance.uiOnlineMatchPlayGameCanvas.transform.GetChild(1).gameObject.SetActive(true); // hiển thi ui pause game
            UIManager.Instance.StatusKeyGameOnlineStr = "gameWon.Txt";
            StartCoroutine(RoutineChangeScene());
        }
        else if (myScore < opponentScore)
        {
            Debug.Log("ddiem so " + myScore + " " + opponentScore + "thua");

            UIManager.Instance.uiOnlineMatchPlayGameCanvas.transform.GetChild(1).gameObject.SetActive(true);
            UIManager.Instance.StatusKeyGameOnlineStr = "gameOver.Txt";

        }
        else { Debug.Log("HÒA RỒIS"); }
    }

    IEnumerator RoutineChangeScene()
    {
        yield return new WaitForSeconds(1);
        UIManager.Instance.ChangeScene(UIManager.SceneType.ONLINEMAINMENU);
    }


    // Thêm hàm này vào trong class OnlineMatchManager
    [PunRPC]
    public void RPC_OnPlayerForfeit(int quitterActorNumber)
    {
        PhotonManager.Instance.IsPlayingOnline = false;

        // Nếu ActorNumber truyền lên là của mình -> Mình là người nhấn Quit -> THUA
        if (PhotonNetwork.LocalPlayer.ActorNumber == quitterActorNumber)
        {
            UIManager.Instance.StatusKeyGameOnlineStr = "gameOver.Txt"; // Hoặc key bạn dùng cho "Bỏ cuộc"
            Debug.Log("Bạn đã bỏ cuộc và bị xử thua.");
        }
        else
        {
            // Nếu không phải mình -> Đối thủ nhấn Quit -> MÌNH THẮNG
            UIManager.Instance.StatusKeyGameOnlineStr = "gameWon.Txt";
            Debug.Log("Đối thủ đã thoát. Bạn thắng!");
        }

        // Hiển thị UI kết quả (Panel ở vị trí GetChild(1))
        if (UIManager.Instance.uiOnlineMatchPlayGameCanvas != null)
        {
            UIManager.Instance.uiOnlineMatchPlayGameCanvas.transform.GetChild(1).gameObject.SetActive(true);
        }

        // Tự động quay về Menu sau vài giây hoặc chờ người dùng nhấn nút trên UI
        StartCoroutine(RoutineChangeScene());
    }

    // Hàm rời phòng an toàn
    public void LeaveMatch()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }

}