using Photon.Pun;
using Photon.Realtime;
using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    private static PhotonManager instance;
    public static PhotonManager Instance { get => instance; }
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
    public void GetPhotonToken(string playFabUserId,string displayName)
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
        }
        else
        {
            Debug.Log("bạn tham gia phòng của người chơi đã lập");
        }

        // Nếu bạn là người thứ 2 vào phòng, kiểm tra để bắt đầu game
        CheckAndStartGame();
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
        yield return new WaitUntil(() => PhotonNetwork.IsConnectedAndReady);
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            Debug.LogError($"Chưa sẵn sàng! Trạng thái là: {PhotonNetwork.NetworkClientState}");
        }
    }


    public void CreateCustomRoom()
    {
        string roomName = "Room_" + GameMechanics.CreateRamdomRoomName();

        RoomOptions roomOptions = new RoomOptions();

        Debug.Log("hien thi "+ roomName +" "+roomOptions);
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

   /* private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N) )
            {
            Debug.Log("Tổng số phòng: " + cachedRooms.Count);

            foreach (var room in cachedRooms.Values)
            {
                Debug.Log($"Room: {room.Name} | {room.PlayerCount}/{room.MaxPlayers}");
            }
        }
    }*/
}

