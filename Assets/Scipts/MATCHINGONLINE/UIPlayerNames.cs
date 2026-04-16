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

    void UpdateUI()
    {
        var players = PhotonNetwork.PlayerList;

        player1Text.text = "Waiting...";
        player2Text.text = "Waiting...";

        foreach (var p in players)
        {
            if (p.IsMasterClient)
            {
                player1Text.text = p.NickName;
                Debug.Log("a" + p.NickName);

            }
            else
            {

                player2Text.text = p.NickName;
                Debug.Log("b" + p.NickName);
            }
        }
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
}