using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class UIPlayerNames : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI player1Text;
    public TextMeshProUGUI player2Text;

    void Start()
    {
        UpdateUI();
    }

    // Ví dụ cấu trúc trong UIPlayerNames.cs của bạn
    void UpdateUI()
    {
        // 1. Kiểm tra an toàn xem các Text Component đã được kéo vào Inspector chưa
        if (player1Text == null || player2Text == null) return;

        // 2. Hiển thị tên của bạn
        if (PhotonNetwork.LocalPlayer != null)
        {
            player1Text.text = PhotonNetwork.LocalPlayer.NickName;
        }

        // 3. KIỂM TRA ĐỐI THỦ: Tránh NullReferenceException khi đấu với BOT
        bool isWithBot = PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("IsWithBot");

        if (isWithBot)
        {
            player2Text.text = "BOT AI"; // Đặt tên cố định cho Bot nếu phòng có thuộc tính IsWithBot
        }
        else
        {
            // Nếu là đấu mạng thật, cần kiểm tra xem đối thủ đã vào phòng chưa
            Player opponent = GetOpponentPlayer();
            if (opponent != null)
            {
                player2Text.text = opponent.NickName;
            }
            else
            {
                player2Text.text = "Đang đợi người chơi...";
            }
        }
    }

    // Hàm bổ trợ để lấy Player đối thủ một cách an toàn
    private Player GetOpponentPlayer()
    {
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (!p.IsLocal) return p;
        }
        return null;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Mình vừa vào phòng");
        UpdateUI();
    }
    // 🔥 CÁI QUAN TRỌNG NHẤT
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Người mới vào: " + newPlayer.NickName);
        UpdateUI();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateUI();
    }
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("IsWithBot"))
        {
            Debug.Log("UIPlayerNames: Phát hiện phòng cập nhật chế độ BOT! Đang cập nhật UI...");
            UpdateUI();
        }
    }
}