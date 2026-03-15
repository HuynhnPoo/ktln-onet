using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private int width;
    public int Width { set => width = value; get => width; }


    private int height;
    public int Height { set => height = value; get => height; }

    private float cellSize;
    public float CellSize { set => CellSize = value; get => cellSize; }


    private float spacing;
    public float Spacing { set => spacing = value; get => spacing; }


    private int localSize;
    public int LocalSize { set => localSize = value; get => localSize; }


    public bool[,] unlockLevelLayout { set; get; }

    [SerializeField]private TileData tileData;
    private List<Tile> allTilles;

    [SerializeField]private GameObject tilePrefabs;
    public List<GameObject> allTiles = new List<GameObject>();
    public Board Board { get; set; }

    // Start is called before the first frame update
    void Start()
    {

      /*  for (int i = 0; i < 6; i++)
        {
        
          GameObject obj =Instantiate(tilePrefabs);
            obj.transform.SetParent(transform);
           Tile tile =obj.GetComponent<Tile>();
            tile.SetSkin(tileData);
        }*/
    }


    // Update is called once per frame
    void Update()
    {

    }


    public void SpawnGridFromLevel(LevelData level)
    {


        // 1. Xóa các Tile cũ nếu có (để tránh chồng đè khi đổi level)
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        Board = new Board(level.gridWidth,level.gridHeight,level.cellSize,level.spacing);
        
        this.width = level.gridWidth;
        this.height = level.gridHeight;

        // 2. Tính toán để Grid nằm giữa màn hình
        float totalWidth = (this.width - 1) * (level.cellSize +level.spacing);
        float totalHeight = (this.height - 1) * (level.cellSize + level.spacing);
        Vector3 offset = new Vector3(totalWidth / 2f, totalHeight / 2f, 0);

        // 3. Vòng lặp tạo Grid
        for (int x = 0; x < this.width; x++)
        {
            for (int y = 0; y < width; y++)
            {
                GridCell cellData = level.GetCell(x, y);

                // QUAN TRỌNG: Chỉ tạo Tile nếu cell.type khác 0 (Erase/Empty)
                //  Vector3 pos = new Vector3(x * (level.cellSize + level.spacing), y * (level.cellSize + level.spacing), 0) - offset;

                if (cellData != null && cellData.type != 0)
                {
                    Vector3 pos = transform.position + Board.GetPostionWorld(x, y);

                    GameObject obj = Instantiate(tilePrefabs, pos, Quaternion.identity, transform);
                    // obj.transform.localPosition = pos;
                    obj.name = $"Tile_{x}_{y}";

                    Tile tile = obj.GetComponent<Tile>();
                    // Ở đây bạn có thể truyền cellData.type vào để set loại icon tương ứng
                    tile.SetSkin(tileData);
                    tile.SetPostionGrid(x, y);
                    obj.transform.localScale = Vector3.one * tileData.scaleMultiplier;
                } 
                
            }
        }
    }

}
