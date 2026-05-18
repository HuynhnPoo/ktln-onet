using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderBoardCRT : MonoBehaviour
{
    [SerializeField] private GameObject rankItemPrefab;
    [SerializeField] private GameObject myRankItemPrefab;
    [SerializeField] private Transform contentContainer;

    private void OnEnable()
    {
        if (rankItemPrefab == null)
        {
            rankItemPrefab = Resources.Load<GameObject>("columRank");
        }

        GetLeaderBoard();
        GetMyRank();
    }
    // Start is called before the first frame update
    void Start()
    {
       
    }

    public void GetLeaderBoard()
    {
        foreach (Transform child in contentContainer.transform)
        {
            Destroy(child.gameObject);
        }

        GetLeaderboardRequest request = new GetLeaderboardRequest
        {
            StatisticName = "Score",
            StartPosition = 0,
            MaxResultsCount = 10,

        };

        PlayFabClientAPI.GetLeaderboard(request, (results) =>
        {
            foreach (var item in results.Leaderboard)
            {
                // 1. Lấy Score chính (từ StatValue)
                int mainScore = item.StatValue;

                // 2. Tìm High_Level trong danh sách Statistics đi kèm
                int highLevel = 0;
                if (item.Profile != null && item.Profile.Statistics != null)
                {
                    var levelStat = item.Profile.Statistics.Find(s => s.Name == "High_Level");
                    if (levelStat != null) highLevel = levelStat.Value;
                }

                GameObject obj = Instantiate(rankItemPrefab, contentContainer);

                RankItem script = obj.GetComponent<RankItem>();
                script.SetText(item.Position + 1, item.DisplayName, mainScore, highLevel);

                Debug.Log(
                    "#" + (item.Position + 1) +
                    " | Name: " + item.DisplayName +
                    " | Score: " + mainScore +
                    " | Level: " + highLevel
                );

            }
        }, OnError);

    }

    public void GetMyRank()
    {
        // khơi tạo
        GetLeaderboardAroundPlayerRequest request = new GetLeaderboardAroundPlayerRequest
        {
            StatisticName = "Score",
            MaxResultsCount = 1
        };

        PlayFabClientAPI.GetLeaderboardAroundPlayer(request, (result) =>
        {
            foreach (var item in result.Leaderboard)
            {
                // 1. Lấy Score chính (từ StatValue)
                int mainScore = item.StatValue;

                // 2. Tìm High_Level trong danh sách Statistics đi kèm
                int highLevel = 0;
                if (item.Profile != null && item.Profile.Statistics != null)
                {
                    var levelStat = item.Profile.Statistics.Find(s => s.Name == "High_Level");
                    if (levelStat != null) highLevel = levelStat.Value;
                }
                //// GameObject obj = Instantiate(rankItemPrefab, contentContainer);

                RankItem script = myRankItemPrefab.GetComponent<RankItem>();
                script.SetTextMyRank(item.Position + 1, item.DisplayName, mainScore, highLevel);

                Debug.Log(
                    "MY RANK #" + (item.Position + 1) +
                    " | Name: " + item.DisplayName +
                    " | Score: " + mainScore +
                    " | Level: " + highLevel
                );
            }
        }, OnError);
    }

    private void OnError(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }

}
