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
    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnMouseDown()
    {
     //   Debug.Log("log được thực  hiên"+ tile.Row +" "+ tile.Col);
      startPos=new Vector2Int(tile.Row,tile.Col);


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
    // Update is called once per frame
    void Update()
    {
        
    }
}
