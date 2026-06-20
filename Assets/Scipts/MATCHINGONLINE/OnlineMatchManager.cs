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
    private float turnTime = 5; // thơi gian chuyển tủrn
    private float currentTurnTimer;
    public float TimeGlobal { set; get; }
    private float timeGlobal = 120; //thời gian trung

    private float startTime;
    private bool matchStarted = false;
    private bool isMatchEnded = false;

    private int currentTurnActorNumber;
    private Player currentTurnPlayer; // Lưu trữ đối tượng Player đang giữ lượt
    public bool isMyTurn;

    public static Action<int> OnMatchScored;   // int = score amount
    public static Action OnMatchMade;
    public static Action<List<Vector2Int>> OnMatchFound;
    public static Action OnResultStatus;


    private int myScore = 0;
    private int opponentScore = 0;
    private float timeWaitCounter = 0;
    private float maxWaitTimeout = 5; // thời gian để tạo ra bot


    //public TextMeshProUGUI myScoreText;
    //public TextMeshProUGUI opponentScoreText;
    public ScorePvpTxt myScoreText;
    public ScorePvpTxt opponentScoreText;
    public CoinPvpTxt coinPvpText;

    public TextMeshProUGUI timeGlobalText;
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

        // Nếu lúc load Scene mà phòng đã có sẵn thuộc tính này (thường là Master Client hoặc Client vào sau khi phòng đã set xong)
        if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("MatchStartTime"))
        {
            startTime = Convert.ToSingle(PhotonNetwork.CurrentRoom.CustomProperties["MatchStartTime"]);
        }
        else
        {
            // Nếu chưa có, gán tạm bằng 0 để Update() biết đường mà chờ đợi
            startTime = 0f;
        }

        if (PhotonNetwork.IsMasterClient && PhotonNetwork.InRoom)
        {
            UpdateTurnInRoomProps(PhotonNetwork.LocalPlayer.ActorNumber);
        }

        Invoke("UpdateCurrentTurnPlayer", 0.2f);
    }
    void Update()
    {
        // 1. Nếu không ở trong phòng hoặc game đã hạ màn thì dừng toàn bộ Update
        if (!PhotonNetwork.InRoom) return;
        if (isMatchEnded) return;

        // 2. Logic xử lý BOT hoặc đợi đồng bộ lượt
        if (currentTurnPlayer == null)
        {
            timeWaitCounter += Time.deltaTime;
            if (timeWaitCounter > maxWaitTimeout)
            {
                Debug.Log("Cho BOT vào room");
                StartGameWithBot(); // thực hiện tạo bot ở phòng BOT
            }
        }
        else
        {
            timeWaitCounter = 0f;
        }

        // 3. LOGIC ĐẾM THỜI GIAN TRẬN ĐẤU (Đã sửa đổi để kiểm tra startTime)
        // CHỈ TÍNH TOÁN KHI: startTime đã nhận giá trị hợp lệ từ Room Properties (> 0)
        if (startTime > 0)
        {
            double elapsedTime = PhotonNetwork.Time - startTime;
            float remainingTime = timeGlobal - (float)elapsedTime;

            if (remainingTime < 0) remainingTime = 0;

            if (timeGlobalText != null)
            {
                timeGlobalText.SetText(remainingTime.ToString("F1") + "s");
            }
            TimeGlobal = remainingTime;

            // Chỉ cần thời gian về 0 và là Master Client thì kết thúc game luôn
            if (remainingTime <= 0 && PhotonNetwork.IsMasterClient)
            {
                Debug.Log("<color=red>[TIMER]</color> Hết giờ! Master Client phát lệnh kết thúc trận.");
                DetermineWinner();
            }
        }
        else
        {
            // TRONG LÚC ĐỢI ĐỒNG BỘ: Giữ nguyên số thời gian gốc trên UI, không cho chạy số âm lung tung
            if (timeGlobalText != null)
            {
                timeGlobalText.SetText(timeGlobal.ToString("F1") + "s");
            }
        }

        // 4. Logic trừ thời gian lượt (Turn) của người chơi/BOT
        if (isMyTurn)
        {
            currentTurnTimer -= Time.deltaTime;
            if (currentTurnTimer <= 0)
            {
                ChangeTurn();
            }
        }
        else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("IsWithBot") && currentTurnActorNumber == -1)
        {
            currentTurnTimer -= Time.deltaTime;
            if (currentTurnTimer <= 0 && PhotonNetwork.IsMasterClient)
            {
                Debug.Log("BOT hết thời gian, tự động thu hồi lượt.");
                UpdateTurnInRoomProps(PhotonNetwork.LocalPlayer.ActorNumber);
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
    public void RPC_HandleMatch(Vector2[] pathArray, PhotonMessageInfo info)
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
            // 3. Sử dụng PhotonMessageInfo để xác định chính xác AI hay Người chơi gửi yêu cầu ăn ô

            if (PhotonNetwork.InRoom)
            {

                int senderActorNumber = currentTurnActorNumber;
                // Đồng bộ trực tiếp điểm số dựa theo ID người gửi gói tin RPC này qua mạng
                photonView.RPC(nameof(RPC_AddScore), RpcTarget.All, senderActorNumber, 10);
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

            if (currentTurnActorNumber == -1)
            {
                Debug.Log("<color=yellow>[Turn System]</color> Đồng bộ: Hiện tại đang là lượt của BOT AI.");
                return;
            }

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


    // hàm hiên thi ui change turn
    private void UpdateUI()
    {
        if (turnStatusText == null) return;
        if (currentTurnActorNumber == -1)
        {
            turnStatusText.text = $"Lượt của: BOT AI ({currentTurnTimer:F1}s)";
            return;
        }
        if (currentTurnPlayer != null)
        {
            string displayName = isMyTurn ? "BẠN" : currentTurnPlayer.NickName;
            turnStatusText.text = $"Lượt của: {displayName} ({currentTurnTimer:F1}s)";
        }
        else
        {
            turnStatusText.text = $"Đang đợi đợi người chơi... {timeWaitCounter:F1}s";
        }
    }

    public void ChangeTurn() // thực hiện chang tủn
    {
        // 1. Nếu không phải lượt của mình thì không được phép tự chuyển turn
        if (!isMyTurn) return;

        // 2. Kiểm tra xem phòng này có đang kích hoạt chế độ chơi với Bot không
        bool isWithBot = PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("IsWithBot");

        // 3. SỬA TẠI ĐÂY: Nếu KHÔNG PHẢI đấu với Bot VÀ số người chơi thật < 2 thì mới chặn đổi lượt
        if (!isWithBot && PhotonNetwork.PlayerList.Length < 2) return;

        // Reset lại thời gian đếm ngược của turn mới
        currentTurnTimer = turnTime;

        if (isWithBot)
        {
            // Nếu chơi với Bot, ta đặt mã ActorNumber của lượt hiện tại là -1 (Đại diện cho Bot)
            UpdateTurnInRoomProps(-1);
            Debug.Log("<color=cyan>[Turn System]</color> Đã chuyển lượt từ Người chơi sang BOT (-1).");
        }
        else
        {
            // Logic tìm người chơi tiếp theo trong phòng khi đấu Online PvP thật
            int nextActor = -1;
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                if (p.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    nextActor = p.ActorNumber;
                    break;
                }
            }

            if (nextActor != -1)
            {
                UpdateTurnInRoomProps(nextActor);
                Debug.Log($"<color=cyan>[Turn System]</color> Đã chuyển lượt sang Player ID: {nextActor}");
            }
        }
    }
    public void UpdateTurnInRoomProps(int actorNumber)
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
            startTime = Convert.ToSingle(propertiesThatChanged["MatchStartTime"]);
            Debug.Log("Đã nhận startTime mới từ Server: " + startTime);
        }
    }

    // thực hien khi kết nôi thành công sẽ  tăng điểm
    private void HandleMatchScored(int score)
    {
        if (!PhotonNetwork.InRoom) return;
        photonView.RPC(nameof(RPC_AddScore), RpcTarget.All,
            PhotonNetwork.LocalPlayer.ActorNumber, score);
    }

    [PunRPC]
    public void RPC_AddScore(int scorerActorNumber, int amount)
    {
        if (scorerActorNumber == -1)
        {
            opponentScore += amount;

            return;
        }
        // Tìm xem người vừa ghi điểm là ai trong danh sách Player
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (p.ActorNumber == scorerActorNumber)
            {
                if (p.IsLocal)
                {
                    // Nếu chính là máy này ghi điểm
                    myScore += amount;

                }
                else
                {
                    // Nếu là máy khác ghi điểm (đối thủ)
                    opponentScore += amount;

                }
                break;
            }
        }

        Debug.Log($"Player {scorerActorNumber} ghi điểm. MyScore: {myScore}, Opponent: {opponentScore}");
    }

    public int GetMyScore()
    {
        return myScore;
    }

    public int GetOpponentScore()
    {
        return opponentScore;
    }
    // hàm thực hiện kiểm tra  win và hay thua
    // 1. Hàm này được gọi từ Update của Master Client
    public void DetermineWinner()
    {
        if (isMatchEnded) return;

        if (PhotonNetwork.InRoom)
        {
            // Gửi lệnh RPC cho TẤT CẢ mọi người trong phòng (bao gồm cả Master và Client)
            photonView.RPC(nameof(RPC_ExecuteDetermineWinner), RpcTarget.All);
        }
        else
        {
            // Nếu chơi một mình thì tự chạy trực tiếp
            ExecuteDetermineWinnerLocal();
        }
    }

    // 2. Hàm tiếp nhận lệnh từ mạng Photon
    [PunRPC]
    public void RPC_ExecuteDetermineWinner()
    {
        ExecuteDetermineWinnerLocal();
    }

    // 3. Hàm xử lý logic hiển thị UI và cộng điểm thực sự trên từng máy
    private void ExecuteDetermineWinnerLocal()
    {
        if (isMatchEnded) return;
        isMatchEnded = true; // Khóa Update ngay lập tức, thời gian sẽ dừng hẳn ở 0.0s

        PhotonManager.Instance.IsPlayingOnline = false;

        if (UIManager.Instance.uiOnlineMatchPlayGameCanvas != null)
        {
            UIManager.Instance.uiOnlineMatchPlayGameCanvas.transform.GetChild(1).gameObject.SetActive(true);

        }

        int valueCoin = 0;

        if (myScore > opponentScore)
        {
            Debug.Log("Điểm số " + myScore + " - " + opponentScore + " -> THẮNG");
            valueCoin = 50;
            UIManager.Instance.StatusKeyGameOnlineStr = "gameWon.Txt";
            SoundManager.Instance.PlaySfx("GameWinSFX");
        }
        else if (myScore < opponentScore)
        {
            valueCoin = 10;
            Debug.Log("Điểm số " + myScore + " - " + opponentScore + " -> THUA");
            UIManager.Instance.StatusKeyGameOnlineStr = "gameOver.Txt";
            SoundManager.Instance.PlaySfx("GameOverSFX");
        }
        else
        {
            valueCoin = 20;
            Debug.Log("Điểm số " + myScore + " - " + opponentScore + " -> HÒA");
            UIManager.Instance.StatusKeyGameOnlineStr = "gameWon.Txt";
        }

        if (PlayFabDataManager.Instance != null && PlayFabDataManager.Instance.playerData != null)
        {
            GameMechanics.AddCoinPvP(PlayFabDataManager.Instance.playerData, valueCoin);
        }

        if (coinPvpText != null)
        {
            coinPvpText.DisplayRewardCoins(valueCoin);
        }

        StartCoroutine(RoutineChangeScene()); // chuyển về menuoninle
    }
    IEnumerator RoutineChangeScene() // chuyển về sence mainmeunu online
    {
        yield return new WaitForSeconds(2);
        UIManager.Instance.ChangeScene(UIManager.SceneType.ONLINEMAINMENU);
    }


    // hàm thực hiên khi thoát
    [PunRPC]
    public void RPC_OnPlayerForfeit(int quitterActorNumber)
    {
        if (isMatchEnded) return; // Tránh chạy trùng lặp nếu game đã kết thúc trước đó
        isMatchEnded = true;

        PhotonManager.Instance.IsPlayingOnline = false;
        int valueCoin = 0;
        // Hiển thị UI kết quả (Panel ở vị trí GetChild(1))
        if (UIManager.Instance.uiOnlineMatchPlayGameCanvas != null)
        {
            UIManager.Instance.uiOnlineMatchPlayGameCanvas.transform.GetChild(1).gameObject.SetActive(true);
        }
        // Nếu ActorNumber truyền lên là của mình -> Mình là người nhấn Quit -> THUA
        if (PhotonNetwork.LocalPlayer.ActorNumber == quitterActorNumber)
        {
            UIManager.Instance.StatusKeyGameOnlineStr = "gameOver.Txt"; // Hoặc key bạn dùng cho "Bỏ cuộc"
            Debug.Log("Bạn đã bỏ cuộc và bị xử thua.");
            valueCoin = 10;
        }
        else
        {
            // Nếu không phải mình -> Đối thủ nhấn Quit -> MÌNH THẮNG
            UIManager.Instance.StatusKeyGameOnlineStr = "gameWon.Txt";
            Debug.Log("Đối thủ đã thoát. Bạn thắng!");
            valueCoin = 50;
        }


        GameMechanics.AddCoinPvP(PlayFabDataManager.Instance.playerData, valueCoin);
        coinPvpText.DisplayRewardCoins(valueCoin);
        StopAllCoroutines();
        // Tự động quay về Menu sau vài giây hoặc chờ người dùng nhấn nút trên UI
        StartCoroutine(RoutineChangeScene());
    }

    // Hàm rời phòng an toàn
    public void LeaveMatch()
    {
        if (PhotonNetwork.InRoom)
        {
            // Gửi thông báo cho mọi người rằng mình chủ động thoát (xử thua bản thân)
            photonView.RPC(nameof(RPC_OnPlayerForfeit), RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
            StartCoroutine(WaitAndLeave());
        }
    }


    private IEnumerator WaitAndLeave()
    {
        yield return new WaitForSeconds(0.2f);
        PhotonNetwork.LeaveRoom();
    }

    // Đánh dấu phòng này có Bot qua Custom Properties
    void StartGameWithBot()
    {
        if (!PhotonNetwork.InRoom || !PhotonNetwork.IsMasterClient) return;

        Debug.Log("<color=yellow>[BOT MODE]</color> Quá thời gian chờ. Đang tiến hành ẩn/khóa phòng và cấu hình BOT...");
       PhotonNetwork.CurrentRoom.IsOpen=false; // đóng phòng không ai vào
        PhotonNetwork.CurrentRoom.IsVisible=false; // ẩn phòng khỏi danh sách tạo ngẫu nhiên
        
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props.Add("IsWithBot", true);
        // 2. QUAN TRỌNG: Tự sinh thời gian bắt đầu trận đấu ngay tại đây vì không có người thứ 2 vào phòng
        if (!PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("MatchStartTime"))
        {
            props.Add("MatchStartTime", (float)PhotonNetwork.Time);
            startTime = (float)PhotonNetwork.Time;
        }
        else
        {
            // Nếu ĐÃ CÓ thời gian từ trận PvP trước đó, lấy lại thời gian đó, KHÔNG ghi đè bừa bãi
            startTime = Convert.ToSingle(PhotonNetwork.CurrentRoom.CustomProperties["MatchStartTime"]);
        }
        props.Add("CurrentTurnActor", PhotonNetwork.LocalPlayer.ActorNumber);

        PhotonNetwork.CurrentRoom.SetCustomProperties(props);

        // Kích hoạt biến thời gian trên máy local ngay lập tức
        //currentTurnTimer = turnTime;
        matchStarted = true;
        UpdateCurrentTurnPlayer();
    }
}