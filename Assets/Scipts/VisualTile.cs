using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualTile : MonoBehaviour
{

    private int row, col;
    public int Row => row ;
    public int Col => col ;

    public int index;
    private SpriteRenderer spriteRenderer;

    
    private void Awake()
    {

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning("Tile thiếu spriterender", gameObject);
        }
    }

    // Start is called before the first frame update


    public void SetSkin(TileData skin)
    {
        if (skin == null && index < 0 && index >= skin.tileSprites.Length) return;

        spriteRenderer.sprite = skin.tileSprites[index];
        transform.localScale = Vector2.one * skin.scaleMultiplier;
    }
   public void SetPostionGrid(int x, int Y)
    {
        this.col= x;
        this.row = Y;
    }
   
    public void SetPostionVisual(Vector3 pos)
    {
        //this.target
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
