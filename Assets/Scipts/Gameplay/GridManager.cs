using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class GridManager : MonoBehaviour
{
    private int width;
    public int Width { set => width = value; get => width; }

    private int height;
    public int Height { set => height = value; get => height; }

    [SerializeField] private GameObject tilePrefabs; //
    [SerializeField] private GameObject obstaclePrefabs; // vật cản 
    [SerializeField] private GameObject selectPrefabs; // vật cản 

    private GameObject[,] selectVisualPrefabs; // ô đã đã chọn

    [SerializeField] private TileDatabaseList tileDatabase; //lưu chữ các dữ của các tile từ SO
    [SerializeField] private Transform[] holder;
    public TileData TileData
    {
        get
        {
            if (tileDatabase != null)
            {
                TileData activePack = tileDatabase.GetSelectedSkinPack();
                if (activePack != null) return activePack;

                if (tileDatabase.allSkinPacks != null && tileDatabase.allSkinPacks.Count > 0)
                {
                    return tileDatabase.allSkinPacks[0];
                }
            }
            return null;
        }

    }

    private ControlTile[,] allTiles;
    public Board Board { get; set; }

    public LevelManager LevelManagerGame { private set; get; }
    public LevelData levelData { get; set; }
    [SerializeField] private LineController lineController;

    Vector2Int? selectedTiled = null;

    private void Awake()
    {
        tileDatabase = Resources.Load<TileDatabaseList>("SO/tile/TileDatabase");

    }
    private void SyncSkinFromPlayerData()
    {
        if (PlayFabDataManager.Instance == null ||
            PlayFabDataManager.Instance.playerData == null ||
            tileDatabase == null) return;

        string savedTileId = PlayFabDataManager.Instance.playerData.currentTileId;

        if (!string.IsNullOrEmpty(savedTileId))
        {
            tileDatabase.SetSelectedSkinPack(savedTileId);
            Debug.Log($"[GridManager] Đã sync skin từ PlayerData: {savedTileId}");
        }
    }

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

    void CheckClickOutside() // click bên ngaoif sẽ hủy
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
        HideSelection();
    }

    //
    public void SpawnGridFromLevel(LevelData level)
    {
        // Lấy Seed từ Room Properties
        if (PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("LevelSeed"))
        {
            int seed = (int)PhotonNetwork.CurrentRoom.CustomProperties["LevelSeed"];
            UnityEngine.Random.InitState(seed); // QUAN TRỌNG: Mọi máy sẽ Random ra kết quả y hệt nhau
        }

        SyncSkinFromPlayerData(); // kiểm tra và khởi tạo ngay ban đàu skin tile nếu bạn đã chọn
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

        selectVisualPrefabs = new GameObject[this.width, this.height];

        Debug.Log($"GridManager position: {transform.position}");
        Debug.Log($"Camera position: {Camera.main.transform.position}");
        Debug.Log($"First tile world pos: {Board.GetPostionWorld(0, 0)}");
        Debug.Log($"cellSize: {level.cellSize}, spacing: {level.spacing}");
        Debug.Log($"width: {width}, height: {height}");

        List<Vector2Int> validPositions = new List<Vector2Int>();
        List<Vector2Int> matchablePositions = new List<Vector2Int>();  //các cặp có teher match được với nhau

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridCell cellData = level.GetCell(x, y);
                if (cellData != null && cellData.type != 0)
                {
                    validPositions.Add(new Vector2Int(x, y));
                    if (cellData.IsMatchable)
                    {
                        matchablePositions.Add(new Vector2Int(x, y));
                    }
                }
            }
        }

        // Kiểm tra số lượng ô phải là số chẵn để có thể tạo cặp
        if (matchablePositions.Count % 2 != 0)
        {
            Debug.LogError("Số lượng ô hợp lệ trong LevelData là số LẺ! Không thể tạo cặp Match.");
            return;
        }

        // 3. Tạo danh sách ID ảnh theo cặp
        List<int> pairIndices = CreatePairedList(matchablePositions.Count);

        // 4. Trộn danh sách ID (Shuffle)
        ShuffleList(pairIndices);
        AssignIconId(level, matchablePositions, pairIndices);
        // 5. Spawn Tile vào các vị trí đã xác định

        allTiles = new ControlTile[this.width, this.height];

        // TileData tileCurrentData = tileDatabase.GetSelectedSkinPack();  // Lấy bộ skin đang được active ra để dùng lúc sinh ra bàn chơi

        int matchableIndex = 0;
        Debug.Log(validPositions.Count);
        for (int i = 0; i < validPositions.Count; i++)
        {
            Vector2Int posGrid = validPositions[i];
            Vector3 posWorld = transform.position + Board.GetPostionWorld(posGrid.x, posGrid.y);
            GridCell currentCell = Board.GetCell(posGrid.x, posGrid.y);

            // Trường hợp 1: Nếu là VẬT CẢN (Type == 3)
            if (currentCell.IsObstacle)
            {
                if (obstaclePrefabs != null)
                {
                    GameObject obsObj = Instantiate(obstaclePrefabs, posWorld, Quaternion.identity, transform);
                    obsObj.name = $"Obstacle_{posGrid.x}_{posGrid.y}";
                    currentCell.iconID = -3; // ID đặc biệt cho vật cản
                }
            }
            // Trường hợp 2: Nếu là Ô CHƠI ĐƯỢC (Type == 1 hoặc 2)
            else if (currentCell.IsMatchable)
            {
                int assignedIndex = pairIndices[matchableIndex];
                matchableIndex++; // Tăng biến đếm ô thường lên

                GameObject obj = Instantiate(tilePrefabs, posWorld, Quaternion.identity, transform);
                obj.name = $"Tile_{posGrid.x}_{posGrid.y}";

                ControlTile controlTile = obj.GetComponent<ControlTile>();
                allTiles[posGrid.x, posGrid.y] = controlTile;
                currentCell.linkedTile = controlTile;

                GameObject selectObj = Instantiate(selectPrefabs, posWorld, Quaternion.identity, transform);
                selectObj.SetActive(false);

                selectVisualPrefabs[posGrid.x, posGrid.y] = selectObj;



                VisualTile tile = obj.transform.GetChild(0).GetComponent<VisualTile>();
                tile.index = assignedIndex;
                tile.SetSkin(TileData);
                tile.SetPostionGrid(posGrid.x, posGrid.y);
            }
        }
    }

    private void HandleGravityMove(GridCell cell, Vector3 startWorldPos) // thực hinen di chuyển các tilé
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
            var cell = Board.GetCell(validPos[index].x, validPos[index].y);

            if (cell != null) cell.iconID = pairIndeices[index];
        }
    }

    private List<int> CreatePairedList(int totalCells)
    {
        List<int> list = new List<int>();
        int totalPairs = totalCells / 2;

        // Lấy số lượng ảnh tối đa có trong TileData hiện tại
        int maxSprites = TileData.tileSprites.Length;

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

    void HideSelection()
    {
        if (selectVisualPrefabs == null) return;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (selectVisualPrefabs[x, y] != null && selectVisualPrefabs[x, y].activeSelf)
                {
                    selectVisualPrefabs[x, y].SetActive(false);
                }
            }
        }
    }

    public void SelectTile(int x, int y) // kiểm tra chọn 2 tile
    {
        if (PhotonNetwork.InRoom && !GameManager.Instance.CanIPlay())
        {
            Debug.Log("Không phải lượt của bạn!");
            return;
        }



        if (selectedTiled == null)
        {
            selectedTiled = new Vector2Int(y, x); //lưu vị tri tile 1 
            SoundManager.Instance.PlaySfx("TileClickSFX");

            Debug.Log("hiwn thi tile selct"+selectVisualPrefabs[y,x].transform.parent);
            selectVisualPrefabs[y, x].SetActive(true); // bật obj đã chọn

        }

        else
        {
            Vector2Int firstTilePos = selectedTiled.Value;
            Vector2Int secondTilePos = new Vector2Int(y, x);
            Debug.Log($"tile 1 {firstTilePos.x} {firstTilePos.y} " + $"tile2: {y} {x}");
            selectVisualPrefabs[y, x].SetActive(true);

            if (firstTilePos == secondTilePos)
            {
                selectedTiled = null;
                HideSelection();
                return;
            }

            List<Vector2Int> gridPath = GameMechanics.GetPath(firstTilePos, secondTilePos, Board);

            if (gridPath != null)
            {
                if (PhotonNetwork.InRoom)
                {
                    // Gửi RPC tới TẤT CẢ người chơi (kể cả chính mình)
                    OnlineMatchManager.OnMatchFound?.Invoke(gridPath);
                }
                else if (PlayFabDataManager.Instance.playerData.playerName != "" && GameManager.Instance.IsOnlineMode)
                {
                    //  Debug.Log(PlayFabDataManager.Instance.playerData);
                    GameMechanics.AddReward(PlayFabDataManager.Instance.playerData, 10, GameManager.Instance.AmountScore);// cộng thêm 10 vàng
                    HandleMatch(gridPath); // thuc hien  ket noi

                }
                else
                {
                    GameMechanics.AddScore(GameManager.Instance.AmountScore); // cong  100 ddidiem ở so
                    HandleMatch(gridPath);

                }
                Debug.Log("hai tile có thể connect được với nhau");

            }
            else
            {
                SoundManager.Instance.PlaySfx("TileClickSFX");
                Debug.Log("không thể kết nối");
            }
            selectedTiled = null;
            HideSelection();
        }

    }

    public void HandleMatch(List<Vector2Int> gridPath) // hàm sử lí match thanhf conog
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

        Board.SetCellEmpty(p1.x, p1.y);
        Board.SetCellEmpty(p2.x, p2.y);

        Debug.Log("hien thị ra" + allTiles[p1.x, p1.y] + " " + allTiles[p2.x, p2.y]);
        // Xóa Tile thực tế
        allTiles[p1.x, p1.y].DestroyTile();
        allTiles[p1.x, p1.y] = null; // Quan trọng: Phải gán null trong mảng quản lý
        allTiles[p2.x, p2.y].DestroyTile();
        allTiles[p2.x, p2.y] = null;

        SoundManager.Instance.PlaySfx("MatchCorrectSFX");
        // KÍCH HOẠT TRỌNG LỰC
        if (levelData.gravityType != BoardGravityType.None)
        {
            // Gọi hàm ApplyGravity mà chúng ta đã thảo luận ở các câu trước
            GameMechanics.ApplyGravity(Board, levelData.gravityType, Board.gridCell);

            // Sau khi logic swap xong, ta cần cập nhật lại mảng allTiles hiển thị
            SyncAllTilesArray();
        }

        List<Vector2Int> testMatch = GameMechanics.FindPossibleMatch(Board, width, height);
        Debug.Log($"[Debug Kẹt] Số nước đi tìm thấy: {(testMatch != null ? "Còn nước đi" : "null - Hết nước")}");

        if (GameMechanics.CheckNoMatchTile(Board, width, height))
        {
            Debug.Log("<color=red>Game Mechanics xác nhận: Hết nước đi hợp lệ! Bị kẹt hoàn toàn.</color>");
            GameManager.Instance.GameOver();

            if (GameManager.Instance.IsOnlineMode) 
                GameManager.Instance.IsRevive = true;
            return;

        }
        Board.CheckLevelProgress(this.width, this.height);


    }

    //private void SyncAllTilesArray()
    //{
    //    ControlTile[,] newLayout = new ControlTile[width, height];

    //    // Duyệt qua tất cả ControlTile hiện có trên scene
    //    ControlTile[] currentTiles = GetComponentsInChildren<ControlTile>();
    //    foreach (var t in currentTiles)
    //    {
    //        VisualTile v = t.GetComponentInChildren<VisualTile>();
    //        // Tìm xem tile này đang ở đâu trong logic Board
    //        for (int x = 0; x < width; x++)
    //        {
    //            for (int y = 0; y < height; y++)
    //            {
    //                if (Board.GetCell(x, y) != null && !Board.GetCell(x, y).IsEmpty)
    //                {
    //                    // Nếu iconID khớp (hoặc bạn có ID riêng cho mỗi GridCell object)
    //                    // Ở đây tốt nhất là ControlTile nên giữ tham chiếu GridCell
    //                }
    //            }
    //        }
    //    }
    //}

    private void SyncAllTilesArray()
    {
        allTiles = new ControlTile[width, height];

        ControlTile[] currentTiles = GetComponentsInChildren<ControlTile>();
        foreach (var t in currentTiles)
        {
            VisualTile v = t.GetComponentInChildren<VisualTile>();
            if (v != null)
            {
                int col = v.Col;
                int row = v.Row;
                if (col >= 0 && col < width && row >= 0 && row < height)
                {
                    allTiles[col, row] = t;
                }
            }
        }
    }

    public void HighlightTwoCells(GridCell cellA, GridCell cellB)
    {
        if (cellA == null || cellB == null) return;

        // Truy cập thông qua cầu nối linkedTile
        if (cellA.linkedTile != null)
        {
            cellA.linkedTile.ActiveHint();
        }

        if (cellB.linkedTile != null)
        {
            cellB.linkedTile.ActiveHint();
        }
    }


    public void ApplyNewSkinPack(string itemId)
    {
        TileData newSkinData = Resources.Load<TileData>($"SO/tile/{itemId}");

        if (newSkinData == null || newSkinData.tileSprites.Length == 0)
        {
            Debug.LogError($"[GridManager] Không tìm thấy TileData tại: SO/tile/{itemId}");
            return;
        }

        // ✅ FIX 1: Cập nhật skin được chọn trong Database để lần spawn sau dùng đúng
        if (tileDatabase != null)
        {
            tileDatabase.SetSelectedSkinPack(itemId); // Cần thêm hàm này vào TileDatabaseList
        }

        // ✅ FIX 2: Chỉ apply lên tile hiện tại nếu bàn chơi đang tồn tại
        if (allTiles == null)
        {
            Debug.Log("[GridManager] Bàn chơi chưa khởi tạo, skin sẽ được dùng ở lần chơi tiếp theo.");
            return;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                ControlTile controlTile = allTiles[x, y];
                if (controlTile != null)
                {
                    VisualTile vTile = controlTile.GetComponentInChildren<VisualTile>();
                    if (vTile != null)
                    {
                        vTile.SetSkin(newSkinData);
                    }
                }
            }
        }

        Debug.Log($"[Skin] Đã cập nhật bàn chơi sang skin: {itemId}");
    }
}