using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridManager : MonoBehaviour
{
    private int width;
    public int Width { set => width = value; get => width; }

    private int height;
    public int Height { set => height = value; get => height; }

    [SerializeField] private GameObject tilePrefabs;
    [SerializeField] private TileData tileData;
    public Board Board { get; set; }
    private ControlTile[,] allTiles;

    public LevelManager LevelManagerGame { private set; get; }
    public LevelData levelData { get; set; }
    [SerializeField] private LineController lineController;

    Vector2Int? selectedTiled = null;


    private void OnEnable()
    {
        LevelManagerGame = GetComponent<LevelManager>();

        lineController = GetComponent<LineController>();

    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckClickOutside();
        }
    }

    void CheckClickOutside()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            // Debug.Log(hit.collider);
            if (hit.collider.GetComponentInChildren<VisualTile>() != null)
            {
                //Debug.Log(hit.collider+"aaaaa");
                return;
            }
        }
        selectedTiled = null;

        /* // ❗ click ngoài tile
         if (selectedTiled != null)
         {
             Debug.Log("Click ngoài → bỏ chọn");
         }*/
    }
    public void SpawnGridFromLevel(LevelData level)
    {
        transform.position = Vector3.zero;
        levelData = level;
        // 1. Xóa các Tile cũ
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        this.width = level.gridWidth;
        this.height = level.gridHeight;
        Board = new Board(width, height, level.cellSize, level.spacing, level);
        Board.OnCellMoved += HandleGravityMove;

        Debug.Log($"GridManager position: {transform.position}");
        Debug.Log($"Camera position: {Camera.main.transform.position}");
        Debug.Log($"First tile world pos: {Board.GetPostionWorld(0, 0)}");
        Debug.Log($"cellSize: {level.cellSize}, spacing: {level.spacing}");
        Debug.Log($"width: {width}, height: {height}");
        // 2. Tìm tất cả các ô hợp lệ (vị trí mà Editor đã vẽ type != 0)
        List<Vector2Int> validPositions = new List<Vector2Int>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridCell cellData = level.GetCell(x, y);
                if (cellData != null && cellData.type != 0)
                {
                    validPositions.Add(new Vector2Int(x, y));
                }
            }
        }

        // Kiểm tra số lượng ô phải là số chẵn để có thể tạo cặp
        if (validPositions.Count % 2 != 0)
        {
            Debug.LogError("Số lượng ô hợp lệ trong LevelData là số LẺ! Không thể tạo cặp Match.");
            return;
        }

        // 3. Tạo danh sách ID ảnh theo cặp
        List<int> pairIndices = CreatePairedList(validPositions.Count);

        // 4. Trộn danh sách ID (Shuffle)
        ShuffleList(pairIndices);
        AssignIconId(level, validPositions, pairIndices);
        // 5. Spawn Tile vào các vị trí đã xác định

        allTiles = new ControlTile[this.width, this.height];
        Debug.Log(validPositions.Count);
        for (int i = 0; i < validPositions.Count; i++)
        {

            Vector2Int posGrid = validPositions[i];
            int assignedIndex = pairIndices[i]; // Lấy ID đã được trộn

            Vector3 posWorld = transform.position + Board.GetPostionWorld(posGrid.x, posGrid.y);

            GameObject obj = Instantiate(tilePrefabs, posWorld, Quaternion.identity, transform);
            obj.name = $"Tile_{posGrid.x}_{posGrid.y}";

            ControlTile controlTile = obj.GetComponent<ControlTile>();
            allTiles[posGrid.x, posGrid.y] = controlTile;
            Board.GetCell(posGrid.x, posGrid.y).linkedTile = controlTile;

            VisualTile tile = obj.transform.GetChild(0).GetComponent<VisualTile>();

            // Gán ID và Skin (Sử dụng hàm Setup hoặc SetSkin của bạn)
            tile.index = assignedIndex;
            tile.SetSkin(tileData); // Hàm SetSkin sẽ dùng tile.index để lấy sprite
            tile.SetPostionGrid(posGrid.x, posGrid.y);

            Debug.Log(tile.transform.parent.name + " " + tile.index);
            //  obj.transform.localScale = Vector3.one * tileData.scaleMultiplier;
        }
    }

    private void HandleGravityMove(GridCell cell, Vector3 startWorldPos)
    {
        // Lấy Tile từ chính cái cell vừa được move
        ControlTile tileToMove = cell.linkedTile;

        if (tileToMove != null)
        {
            // Cập nhật lại tọa độ logic cho VisualTile trước khi di chuyển
            VisualTile vTile = tileToMove.GetComponentInChildren<VisualTile>();
            vTile.SetPostionGrid(cell.x, cell.y); // Để vTile.Col và vTile.Row mang giá trị mới

            // Thực hiện Lerp
            tileToMove.MoveToNewPosition(startWorldPos + transform.position, Board);

            // Cập nhật lại mảng quản lý chính
            allTiles[cell.x, cell.y] = tileToMove;
        }
    }

    void AssignIconId(LevelData level, List<Vector2Int> validPos, List<int> pairIndeices)
    {

        for (int index = 0; index < validPos.Count; index++)
        {

            // Debug.Log("hien ra X" + posTile.x + " Y " + posTile.y + level.name + "cặp được gắn value" + pairIndeices[index]);
            var cell = Board.GetCell(validPos[index].x, validPos[index].y);

            if (cell != null) cell.iconID = pairIndeices[index];
        }
    }

    private List<int> CreatePairedList(int totalCells)
    {
        List<int> list = new List<int>();
        int totalPairs = totalCells / 2;

        // Lấy số lượng ảnh tối đa có trong TileData hiện tại
        int maxSprites = tileData.tileSprites.Length;

        for (int i = 0; i < totalPairs; i++)
        {
            // Chọn ngẫu nhiên 1 index trong kho ảnh
            int randomIdx = Random.Range(0, maxSprites);

            // Thêm 2 lần vào danh sách để tạo 1 cặp
            list.Add(randomIdx);
            list.Add(randomIdx);
        }
        return list;
    }

    private void ShuffleList(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public void SelectTile(int x, int y)
    {
        Debug.Log("x va y" + y + x);
        if (selectedTiled == null)
        {
            selectedTiled = new Vector2Int(y, x);
        }

        else
        {
            Vector2Int firstTilePos = selectedTiled.Value;
            Vector2Int secondTilePos = new Vector2Int(y, x);
            Debug.Log($"tile 1 {firstTilePos.x} {firstTilePos.y} " + $"tile2: {y} {x}");

            if (firstTilePos == secondTilePos)
            {
                selectedTiled = null;
                return;
            }

            List<Vector2Int> gridPath = GameMechanics.GetPath(firstTilePos, secondTilePos, Board);

            if (gridPath != null)
            {

                HandleMatch(gridPath);
                GameMechanics.AddScore(GameManager.Instance.AmountScore);

                if (PlayFabDataManager.Instance.playerData != null) // chỉ thực hiện khi online được đnăg nhập
                {
                    GameMechanics.AddReward(PlayFabDataManager.Instance.playerData, 10, GameManager.Instance.AmountScore);// cộng thêm 10 vàng

                }
                Debug.Log("hai tile có thể connect được với nhau");

            }
            else
            {
                Debug.Log("không thể kết nối");
            }
            selectedTiled = null;
        }

    }

    private void HandleMatch(List<Vector2Int> gridPath)
    {
        // Chuyển tọa độ Grid sang World để vẽ Line
        Vector3[] worldPoints = new Vector3[gridPath.Count];
        //Vector3 centerOffset = Board.GetCenterOffset();
        for (int i = 0; i < gridPath.Count; i++)
        {
            worldPoints[i] = Board.GetPostionWorld(gridPath[i].x, gridPath[i].y) + transform.position;
        }

        lineController.DrawPath(worldPoints); // Gọi hàm vẽ của bạn

        // Hai điểm đầu và cuối là 2 Tile cần xóa
        Vector2Int p1 = gridPath[0];
        Vector2Int p2 = gridPath[gridPath.Count - 1];

        // Debug.Log(p1.x+ p1.y +"--"+p2.x+ p2.y);
        Board.SetCellEmpty(p1.x, p1.y);
        Board.SetCellEmpty(p2.x, p2.y);

        Debug.Log("hien thị ra" + allTiles[p1.x, p1.y] + " " + allTiles[p2.x, p2.y]);
        // Xóa Tile thực tế
        allTiles[p1.x, p1.y].DestroyTile();
        allTiles[p1.x, p1.y] = null; // Quan trọng: Phải gán null trong mảng quản lý
        allTiles[p2.x, p2.y].DestroyTile();
        allTiles[p2.x, p2.y] = null;

        // KÍCH HOẠT TRỌNG LỰC
        if (levelData.gravityType != BoardGravityType.None)
        {
            // Gọi hàm ApplyGravity mà chúng ta đã thảo luận ở các câu trước
            // Bạn cần truyền mảng allTiles vào để GameMechanics có thể Swap cả tham chiếu Script
            GameMechanics.ApplyGravity(Board, levelData.gravityType, Board.gridCell);

            // Sau khi logic swap xong, ta cần cập nhật lại mảng allTiles hiển thị
            SyncAllTilesArray();
        }
        Board.CheckLevelProgress(this.width, this.height);
    }

    private void SyncAllTilesArray()
    {
        ControlTile[,] newLayout = new ControlTile[width, height];

        // Duyệt qua tất cả ControlTile hiện có trên scene
        ControlTile[] currentTiles = GetComponentsInChildren<ControlTile>();
        foreach (var t in currentTiles)
        {
            VisualTile v = t.GetComponentInChildren<VisualTile>();
            // Tìm xem tile này đang ở đâu trong logic Board
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (Board.GetCell(x, y) != null && !Board.GetCell(x, y).IsEmpty)
                    {
                        // Nếu iconID khớp (hoặc bạn có ID riêng cho mỗi GridCell object)
                        // Ở đây tốt nhất là ControlTile nên giữ tham chiếu GridCell
                    }
                }
            }
        }
        // allTiles = newLayout;
    }
}