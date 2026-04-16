using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class ControlTile : MonoBehaviour
{
    private VisualTile tile;
    Vector2Int startPos;
    Vector2Int endPos;

   
    private void OnEnable()
    {
        
        if (tile == null)
        tile =GetComponentInChildren<VisualTile>();
    }

    public void MoveToNewPosition(Vector3 startWorldPos, Board board)
    {
        StopAllCoroutines();

        // Đảm bảo VisualTile đã cập nhật x, y mới
        VisualTile vTile = GetComponentInChildren<VisualTile>();

        // Lấy vị trí đích cục bộ (Local) từ board và cộng với vị trí hiện tại của GridManager
        // Cách này giúp tile luôn nằm đúng trong khung của GridManager
        Vector3 targetPos = board.GetPostionWorld(vTile.Col, vTile.Row);

        // Chuyển đổi targetPos từ Local sang World (tỉ lệ với cha của nó là GridManager)
        Vector3 finalWorldPos = transform.parent.TransformPoint(targetPos);

        StartCoroutine(LerpMove(startWorldPos, finalWorldPos, 0.3f));
    }

    private IEnumerator LerpMove(Vector3 start, Vector3 end, float duration)
    {
        float elapsed = 0;
        transform.position = start;
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = end;
    }

    private void OnMouseDown()
    {
        //   Debug.Log("log được thực  hiên"+ tile.Row +" "+ tile.Col);
        startPos = new Vector2Int(tile.Row, tile.Col);
    }

    private void OnMouseUp()
    {
        GameManager.Instance.gridManager.SelectTile(startPos.x,startPos.y);
        
    }


    // hàm xóa kết nối thnahf công
    public void DestroyTile()
    {
        Destroy(this.gameObject);
    }
  
}
