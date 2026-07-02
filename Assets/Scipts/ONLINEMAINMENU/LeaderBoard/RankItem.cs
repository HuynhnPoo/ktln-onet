using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RankItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] text;

    public void SetText(int rank, string nameDisplay, int score, int level)
    {
        // Kiểm tra xem bạn đã kéo đủ 4 ô Text trong Inspector chưa
        if (text == null || text.Length < 4)
        {
            Debug.LogError($"[RankItem] Mảng text chưa được kéo đủ 4 phần tử trên Object: {gameObject.name}!");
            return;
        }

        if (text[0] != null) text[0].SetText(rank.ToString());
        if (text[1] != null) text[1].SetText(nameDisplay);
        if (text[2] != null) text[2].SetText(score.ToString());
        if (text[3] != null) text[3].SetText(level.ToString());
    }

    public void SetTextMyRank(int rank, string nameDisplay, int score, int level)
    {
        if (text == null || text.Length < 4)
        {
            Debug.LogError($"[RankItem] Mảng text của MyRank chưa được kéo đủ 4 phần tử trên Object: {gameObject.name}!");
            return;
        }

        if (text[0] != null) { text[0].SetText(rank.ToString()); text[0].color = Color.cyan; }
        if (text[1] != null) { text[1].SetText(nameDisplay); text[1].color = Color.cyan; }
        if (text[2] != null) { text[2].SetText(score.ToString()); text[2].color = Color.cyan; }
        if (text[3] != null) { text[3].SetText(level.ToString()); text[3].color = Color.cyan; }
    }
}