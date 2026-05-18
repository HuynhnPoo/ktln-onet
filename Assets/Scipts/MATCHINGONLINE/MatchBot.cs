using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchBot : MonoBehaviour
{
    private GridManager gridManager;
    private OnlineMatchManager matchManager;

    private float botDecisionTime = 2;
    private bool isBotThinking = false;
    // Start is called before the first frame update

    void Start()
    {
        gridManager = GameManager.Instance.gridManager;
        matchManager = GetComponent<OnlineMatchManager>();
    }

    bool IsBotTurn()
    {
        if (PhotonNetwork.CurrentRoom == null || matchManager == null)
            return false;
        bool isWithBot = PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("IsWithBot");
        bool isBotTurnActive = false;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("CurrentTurnActor", out object actorNum))
        {
            isBotTurnActive = ((int)actorNum == -1);
        }

        return isWithBot && isBotTurnActive;
    }
    // Update is called once per frame
    void Update()
    {
        if (IsBotTurn() && !isBotThinking)
        {
            StartCoroutine(BotThinkAndMatchRoutine());
        }
    }

    void ForceChangeTurnToPlayer()
    {
        matchManager.UpdateTurnInRoomProps(PhotonNetwork.LocalPlayer.ActorNumber);
    }

    IEnumerator BotThinkAndMatchRoutine()
    {
        isBotThinking = true;
        yield return new WaitForSeconds(botDecisionTime);

        if (gridManager != null && gridManager.Board != null)
        {
            // Kiểm tra lại một lần nữa xem có đúng turn Bot không trước khi tìm đường
            if (!IsBotTurn())
            {
                isBotThinking = false;
                yield break;
            }

            List<Vector2Int> foundPath = GameMechanics.FindPossibleMatch(gridManager.Board, gridManager.Width, gridManager.Height);

            if (foundPath != null && foundPath.Count >= 2)
            {
                Vector2[] pathArray = new Vector2[foundPath.Count];
                for (int i = 0; i < foundPath.Count; i++)
                {
                    // Sửa nguyên nhân 1: Ép kiểu chuẩn
                    pathArray[i] = new Vector2(foundPath[i].x, foundPath[i].y);
                }

                matchManager.photonView.RPC("RPC_HandleMatch", RpcTarget.All, (object)pathArray);
/*
                if (PhotonNetwork.IsMasterClient)
                {
                    Invoke(nameof(ForceChangeTurnToPlayer), 0.6f);
                }*/

                // Chờ cho đến khi đổi turn xong mới cho phép isBotThinking = false
                yield return new WaitForSeconds(0.7f);
            }
            else
            {
                Debug.LogWarning("<color=orange>[Bot AI]</color> Hết nước đi.");
                if (PhotonNetwork.IsMasterClient)
                {
                    ForceChangeTurnToPlayer();
                }
            }
        }

        isBotThinking = false;
    }
}
