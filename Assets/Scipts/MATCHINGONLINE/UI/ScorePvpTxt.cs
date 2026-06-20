using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScorePvpTxt : TextBase
{
    // Tạo danh sách các chế độ hiển thị để chọn trên Editor cho dễ quản lý
    public enum ScoreDisplayType
    {
        MyScoreInGame,       // Hiện điểm của mình trong trận (Hiện: "Bạn: X")
        OpponentScoreInGame, // Hiện điểm đối thủ trong trận (Hiện: "Đối thủ: X")
        MyScoreAtEndGame     // Hiện điểm của mình khi kết thúc game (Chỉ hiện: "Điểm của bạn: X" hoặc "X Điểm")
    }

    [Header("Cấu hình loại điểm hiển thị")]
    public ScoreDisplayType displayType;

   [SerializeField] private OnlineMatchManager matchManager;

  
    // Hàm này được TextBase gọi liên tục mỗi frame trong Update()
    protected override void PrintText()
    {
        if (text == null || matchManager == null) return;

        switch (displayType)
        {
            case ScoreDisplayType.MyScoreInGame:
                // Hiển thị điểm của mình lúc đang chơi
                int myScoreInGame = matchManager.GetMyScore();
                text.text = $"Bạn: {myScoreInGame}";
                break;

            case ScoreDisplayType.OpponentScoreInGame:
                // Hiển thị điểm đối thủ lúc đang chơi
                int opponentScore = matchManager.GetOpponentScore();
                bool isWithBot = Photon.Pun.PhotonNetwork.CurrentRoom != null &&
                                 Photon.Pun.PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("IsWithBot");
                string opponentName = isWithBot ? "Bot AI" : "Đối thủ";
                text.text = $"{opponentName}: {opponentScore}";
                break;

            case ScoreDisplayType.MyScoreAtEndGame:
                // Hiển thị DUY NHẤT điểm của mình khi kết thúc game trên Panel kết quả
                int myScoreAtEnd = matchManager.GetMyScore();

                // Bạn có thể đổi chữ tùy ý thích, ví dụ: "Tổng điểm: 50" hoặc chỉ hiện mỗi số "50"
                text.text = $"{myScoreAtEnd}";
                break;
        }
    }
}