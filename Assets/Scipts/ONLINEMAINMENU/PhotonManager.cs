using Photon.Pun;
using Photon.Realtime;
using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using static UIManager;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public static PhotonManager Instance { get => instance; }
    private static bool isPlayingOnline = true;
    public bool IsPlayingOnline { get => isPlayingOnline; set => isPlayingOnline = value; }

    private static PhotonManager instance;

    string keyLevelIdStr = "LevelID";


    [SerializeField] public OnlineMatchManager OnlineMatchManager;
    private void Awake()
    {
        if (Instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Giữ kết nối xuyên suốt các Scene
        }
        else
        {
            Destroy(gameObject);
        }
       
    }


    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Log ra để debug xem tên scene thực tế là gì
        Debug.Log($"Đã load Scene: {scene.name}");

        if (scene.name == SceneType.MATCHINGONLINE.ToString())
        {
            // Thử tìm lại
            OnlineMatchManager = FindObjectOfType<OnlineMatchManager>();

            if (OnlineMatchManager == null)
            {
                Debug.LogError("Không tìm thấy OnlineMatchManager trong Scene này!");
            }
            else
            {
                Debug.Log("Đã tìm thấy OnlineMatchManager thành công.");
            }
        }
    }
    private void Start()
    {
        
    }
    public void GetPhotonToken(string playFabUserId, string displayName)
    {

        PhotonNetwork.NickName = displayName;

        var request = new GetPhotonAuthenticationTokenRequest
        {
            PhotonApplicationId = "a0b94226-3f86-4079-af75-4460bad8b663",
        };

        PlayFabClientAPI.GetPhotonAuthenticationToken(request, results =>
        {
            var customAuth = new AuthenticationValues { AuthType = CustomAuthenticationType.Custom };

            // Lưu ý: "username" và "token" phải viết thường y hệt như cấu hình của Photon/PlayFab
            customAuth.AddAuthParameter("username", playFabUserId);
            customAuth.AddAuthParameter("token", results.PhotonCustomAuthenticationToken);

            PhotonNetwork.AuthValues = customAuth;

            // Thêm dòng này để theo dõi trạng thái
            Debug.Log("Đang gửi lệnh kết nối tới Photon...");
            PhotonNetwork.ConnectUsingSettings();
        },
error =>
{
    Debug.LogError("Lỗi lấy Token từ PlayFab: " + error.GenerateErrorReport());
});
    }

    // --- Các hàm Callback của Photon ---
    public override void OnConnectedToMaster()
    {
        Debug.Log("Đã kết nối Master Server. Đang vào Lobby...");
        // UIManager.Instance.ChangeScene(UIManager.SceneType.ONLINEMAINMENU);
        PhotonNetwork.JoinLobby(); // Vào sảnh chờ để có thể tìm phòng
    }


    public override void OnJoinedLobby()
    {
        Debug.Log("Đã vào Lobby thành công!");
        //  UIManager.Instance.ChangeScene(UIManager.SceneType.ONLINEMAINMENU);
    }

    // Nếu không tìm thấy phòng nào (chưa có ai tạo hoặc các phòng đã đầy)
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Không tìm thấy phòng nào đang chờ. Đang tự tạo phòng mới...");

        //RoomOptions roomOptions = new RoomOptions();
        //roomOptions.MaxPlayers = 2;
        //roomOptions.IsVisible = true; // Rất quan trọng: Phải để True để người sau có thể tìm thấy phòng này
        //roomOptions.IsOpen = true;

        //// Truyền 'null' để Photon tự cấp ID phòng, giúp tránh trùng tên tuyệt đối
        //PhotonNetwork.CreateRoom(null, roomOptions);

        CreateCustomRoom();
    }

    // Khi CHÍNH BẠN vào phòng thành công
    public override void OnJoinedRoom()
    {
        Debug.Log($"Bạn đã vào phòng: {PhotonNetwork.CurrentRoom.Name}");
        Debug.Log($"Số người hiện tại: {PhotonNetwork.CurrentRoom.PlayerCount}");
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("bạn đã tạo phòng này.");

            //int levelToPlay = 9;
            //ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
            //props.Add(keyLevelIdStr, levelToPlay);
            //PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        }
        else
        {
            Debug.Log("bạn tham gia phòng của người chơi đã lập");
        }

        // Nếu bạn là người thứ 2 vào phòng, kiểm tra để bắt đầu game
        CheckAndStartGame();
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        //   base.OnRoomPropertiesUpdate(propertiesThatChanged);

        if (propertiesThatChanged.ContainsKey(keyLevelIdStr))
        {
            int levelId = (int)propertiesThatChanged[keyLevelIdStr];

            LevelManager.OnRequestLoadLevel?.Invoke(levelId);
        }
    }

    // Khi NGƯỜI CHƠI KHÁC vào phòng của bạn
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} đã tham gia phòng!");

        // Kiểm tra lại số lượng người để bắt đầu
        CheckAndStartGame();
    }

    private void CheckAndStartGame()
    {
        Debug.Log("My name: " + PhotonNetwork.NickName);
        // Chỉ MasterClient (người tạo phòng) mới có quyền ra lệnh đổi Scene cho tất cả
        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                Debug.Log("Đã đủ 2 người! Bắt đầu trận đấu...");
                // 1. Lấy Level từ PlayFab của Master Client để làm level thi đấu
                int levelToPlay = 4; // Hoặc PlayFabDataManager.Instance.playerData.highestLevel;
                int randomSeed = UnityEngine.Random.Range(0, 99999);

                // 2. Set vào Custom Properties của Room
                ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
                props.Add(keyLevelIdStr, levelToPlay);
                props.Add("LevelSeed", randomSeed);
                props.Add("MatchStartTime", PhotonNetwork.Time);

                PhotonNetwork.CurrentRoom.SetCustomProperties(props);

                // 3. Chuyển Scene cho cả 2 người
                PhotonNetwork.AutomaticallySyncScene = true;
                // Dùng PhotonNetwork.LoadLevel thay vì SceneManager để kéo cả 2 người đi cùng
                PhotonNetwork.LoadLevel(UIManager.SceneType.MATCHINGONLINE.ToString()); // Thay bằng tên Scene chơi game của bạn
            }
            else
            {
                Debug.Log("Đang chờ đối thủ...");
            }
        }
    }

    public void StartMatchmaking()
    {
        // In ra trạng thái hiện tại để kiểm tra
        Debug.Log("Trạng thái hiện tại: " + PhotonNetwork.NetworkClientState);
        StartCoroutine(WaitForPhotonReady());

    }

    private IEnumerator WaitForPhotonReady()
    {
        while (PhotonNetwork.NetworkClientState != ClientState.JoinedLobby)
        {
            yield return null;
        }
        PhotonNetwork.JoinRandomRoom();

    }

    public void CreateCustomRoom()
    {
        string roomName = "Room_" + GameMechanics.CreateRamdomRoomName();

        RoomOptions roomOptions = new RoomOptions();

        Debug.Log("hien thi " + roomName + " " + roomOptions);
        roomOptions.MaxPlayers = 2;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;

        PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError($"Mất kết nối Photon: {cause}");
    }

    private Dictionary<string, RoomInfo> cachedRooms = new Dictionary<string, RoomInfo>();

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList)
            {
                cachedRooms.Remove(room.Name);
            }
            else
            {
                cachedRooms[room.Name] = room;
            }
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("user rơi phong là" + otherPlayer);
        if(isPlayingOnline)
        {
            OnlineMatchManager.photonView.RPC("RPC_OnPlayerForfeit", RpcTarget.All,otherPlayer.ActorNumber);
        }
    }
}

