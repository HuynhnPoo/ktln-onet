using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoutButton : ButtonBase
{
    public override void OnClick()
    {
        UIManager.Instance.ChangeScene(UIManager.SceneType.FORM);
    }

    //private void Start()
    //{
    //    PhotonNetwork.ConnectUsingSettings();
    //}
    //// Hàm này tự động chạy khi kết nối thành công tới Server Photon
    //public override void OnConnectedToMaster()
    //{
    //    Debug.Log("<color=green>Photon:</color> Đã kết nối Master Server thành công!");

    //    // Thường sau khi kết nối xong, ta sẽ vào Lobby để tìm phòng
    //    PhotonNetwork.JoinLobby();
    //}

    //// Hàm này chạy khi vào Sảnh (Lobby) thành công
    //public override void OnJoinedLobby()
    //{
    //    Debug.Log("<color=cyan>Photon:</color> Đã vào Lobby. Sẵn sàng Matchmaking!");
    //}

    //// Hàm này chạy nếu kết nối thất bại (sai AppID, sai Token, mất mạng...)
    //public override void OnDisconnected(DisconnectCause cause)
    //{
    //    Debug.LogError("<color=red>Photon:</color> Kết nối thất bại. Lý do: " + cause);
    //}
}
