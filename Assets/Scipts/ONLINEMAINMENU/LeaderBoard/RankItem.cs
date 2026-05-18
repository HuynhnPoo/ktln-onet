using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RankItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] text;
    private void OnEnable()
    {
        text = GetComponentsInChildren<TextMeshProUGUI>();
    }

    public void SetText(int rank, string nameDisplay, int score, int level)
    {
        text[0].SetText(rank.ToString());
        text[1].SetText(nameDisplay.ToString());
        text[2].SetText(score.ToString());
        text[3].SetText(level.ToString());
    }
    public void SetTextMyRank(int rank, string nameDisplay, int score, int level)
    {
        text[0].SetText(rank.ToString());
        text[1].SetText(nameDisplay.ToString());
        text[2].SetText(score.ToString());
        text[3].SetText(level.ToString());

        text[0].color = Color.cyan;
        text[1].color = Color.cyan;
        text[2].color = Color.cyan;
        text[3].color = Color.cyan;
    }
}
