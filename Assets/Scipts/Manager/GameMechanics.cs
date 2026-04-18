using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class GameMechanics
{
    private static bool timeUp = false;
    private static int currentScore = 0;
    private static float currentTime = 0;
    private static float maxTime = 0;

    private static BoardGravityType gravityType;
    public static void Init(int time, int amountScore, BoardGravityType type)
    {
        GameManager.Instance.AmountScore = amountScore;
        currentScore = 0;
        timeUp = false;
        currentTime = time;
        maxTime = time;

        gravityType = type;
        Debug.Log(gravityType);
        // Debug.Log("hien thi"+currentTime);
    }

    public static void AddScore(int amout)
    {
        currentScore += amout;
        GameManager.Instance.Score = currentScore;
    }

    public static float CountDown()
    {
        if (!timeUp)
        {
            currentTime -= Time.deltaTime;
            //   Debug.Log("hien thi ra "+ time);
            if (currentTime <= 0)
            {
                timeUp = true;
                currentTime = 0;

                Debug.Log("het gio");

            }
        }
        return currentTime;
    }

    public static float GetTimeRatio() => currentTime / maxTime;

    // hàm kiểm tra theo chiều x
    static bool CheckLineX(int y, int x1, int x2, Board board)
    {
        int min = Mathf.Min(x1, x2);
        int max = Mathf.Max(x1, x2);

        for (int x = min + 1; x < max; x++)
        {
            var cell = board.GetCell(x, y);
            if (cell != null && cell.type != 0)
            {

                Debug.Log($"Bị chặn tại ô: {x}, {y} vì type = {cell.type}");
                return false;
            }
        }

        return true;
    }

    //kiểm tra theo chiều y
    static bool CheckLineY(int x, int y1, int y2, Board board)
    {
        int min = Mathf.Min(y1, y2);
        int max = Mathf.Max(y1, y2);

        for (int y = min + 1; y < max; y++)
        {
            var cell = board.GetCell(x, y);
            if (cell != null && cell.type != 0)
            {


                Debug.Log($"Bị chặn tại ô: {x}, {y} vì type = {cell.type}");
                return false;
            }
        }

        return true;
    }

    //checkRect để trả về các điểm bẻ góc
    static List<Vector2Int> CheckRect(Vector2Int a, Vector2Int b, Board board)
    {
        // Điểm góc 1: (a.x, b.y)
        Vector2Int p1 = new Vector2Int(a.x, b.y);
        if (board.GetCell(p1.x, p1.y).type == 0)
        {
            if (CheckLineY(a.x, a.y, p1.y, board) && CheckLineX(b.y, p1.x, b.x, board))
                return new List<Vector2Int> { a, p1, b };
        }

        // Điểm góc 2: (b.x, a.y)
        Vector2Int p2 = new Vector2Int(b.x, a.y);
        if (board.GetCell(p2.x, p2.y).type == 0)
        {
            if (CheckLineX(a.y, a.x, p2.x, board) && CheckLineY(b.x, p2.y, b.y, board))
                return new List<Vector2Int> { a, p2, b };
        }
        return null;
    }
    static List<Vector2Int> CheckMoreLine(Vector2Int a, Vector2Int b, Board board)
    {
        // Quét 4 hướng từ điểm A để tìm điểm trung gian p sao cho p nối được tới B bằng hình chữ L

        // Ngang
        for (int x = -1; x <= GameManager.Instance.gridManager.Width; x++)
        {
            if (x == a.x) continue;
            if (!CheckLineX(a.y, a.x, x, board)) continue; // Đường từ A tới điểm quét phải trống

            Vector2Int p = new Vector2Int(x, a.y);
            var cellP = board.GetCell(p.x, p.y);
            // Nếu ô p nằm trong bàn chơi mà có vật cản thì bỏ qua
            if (cellP != null && cellP.type != 0) continue;

            List<Vector2Int> rectPath = CheckRect(p, b, board);
            if (rectPath != null)
            {
                rectPath.Insert(0, a);
                return rectPath;
            }
        }

        // Dọ
        for (int y = -1; y <= GameManager.Instance.gridManager.Height; y++)
        {
            if (y == a.y) continue;
            if (!CheckLineY(a.x, a.y, y, board)) continue;

            Vector2Int p = new Vector2Int(a.x, y);
            var cellP = board.GetCell(p.x, p.y);
            if (cellP != null && cellP.type != 0) continue;

            List<Vector2Int> rectPath = CheckRect(p, b, board);
            if (rectPath != null)
            {
                rectPath.Insert(0, a);
                return rectPath;
            }
        }
        return null;
    }

    //
    public static List<Vector2Int> GetPath(Vector2Int a, Vector2Int b, Board board)
    {

        Debug.Log("hien thi cell A" + a.x + " " + a.y + " index " + board.GetCell(a.x, a.y).iconID);
        Debug.Log("hien thi cell A" + b.x + " " + b.y + " index " + board.GetCell(b.x, b.y).iconID);
        if (board.GetCell(a.x, a.y).iconID != board.GetCell(b.x, b.y).iconID) return null; // kiểmm tra icon id

        List<Vector2Int> path;

        if (a.y == b.y && CheckLineX(a.y, a.x, b.x, board))
            return new List<Vector2Int> { a, b };
        // 1. Kiểm tra đường thẳng (1 đoạn)
        if (a.x == b.x && CheckLineY(a.x, a.y, b.y, board))
            return new List<Vector2Int> { a, b };

        // 2. Kiểm tra hình chữ L (2 đoạn - 1 khúc cua)
        path = CheckRect(a, b, board);
        if (path != null) return path;

        // 3. Kiểm tra hình chữ Z/U (3 đoạn - 2 khúc cua)
        path = CheckMoreLine(a, b, board);
        if (path != null) return path;

        return null;
    }

    public static string CreateRamdomRoomName()
    {
        string gluphs = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string nameRoom = " ";

        for (int i = 0; i < 5; i++)
        {
            nameRoom += gluphs[Random.Range(0, gluphs.Length)];
        }
        return nameRoom;
    }

    // them tiền và score khi ở scene ở online
    public static void AddReward(PlayerData playerData, int goldAmount, int scoreAmount)
    {
        if (playerData == null) return;
        playerData.gold += goldAmount;
        GameManager.Instance.Coin += goldAmount;
        Debug.Log("thực hiện add scpre và coin" + GameManager.Instance.Coin);
        playerData.score += scoreAmount;
    }

    // update điềm số  khi online
    public static void UpdateHighestLevel(PlayerData playerData, int level)
    {

        if (playerData == null) return;
        if (level > playerData.highestLevel)
        {
            playerData.highestLevel = level;
            PlayFabDataManager.Instance.SavePlayerData();
        }
    }

    public static void ApplyGravity(Board board, BoardGravityType type, GridCell[,] gridCell)
    {
        int w = board.Width;
        int h = board.Height;

        switch (type)
        {
            case BoardGravityType.GravityDown:
                MoveVertical(board, gridCell, true); // isDown = true
                break;
            case BoardGravityType.GravityUp:
                MoveVertical(board, gridCell, false); // isDown = false
                break;
            case BoardGravityType.ShiftLeft:
                MoveHorizontal(board, gridCell, true); // isLeft = true
                break;
            case BoardGravityType.ShiftRight:
                MoveHorizontal(board, gridCell, false); // isLeft = false
                break;
            case BoardGravityType.CenterCollapse:
                MoveCenterCollapse(board, gridCell);
                break;
            case BoardGravityType.SplitOutward:
               MoveSplitOutward (board, gridCell);
                break;
        }
    }

    private static void SwapCellData(GridCell[,] grid, int x1, int y1, int x2, int y2)
    {
        GridCell temp = grid[x1, y1];
        grid[x1, y1] = grid[x2, y2];
        grid[x2, y2] = temp;

        // Cập nhật lại tọa độ x, y bên trong cell để nó khớp với vị trí mới trong mảng
        grid[x1, y1].x = x1; grid[x1, y1].y = y1;
        grid[x2, y2].x = x2; grid[x2, y2].y = y2;
    }

    private static void ExecuteMove(Board board, GridCell[,] grid, int fromX, int fromY, int toX, int toY)
    {
        // Lấy vị trí thế giới HIỆN TẠI của Tile trước khi nó bị đổi dữ liệu
        ControlTile tile = grid[fromX, fromY].linkedTile;
        if (tile == null) return;

        Vector3 startWorldPos = tile.transform.position;

        // Đổi chỗ
        SwapCellData(grid, fromX, fromY, toX, toY);

        // Bắn sự kiện: "Ô tại (toX, toY) vừa di chuyển từ startWorldPos tới"
        board.OnCellMoved?.Invoke(grid[toX, toY], startWorldPos);
    }

    // --- LOGIC DI CHUYỂN DỌC (Lên/Xuống) ---
    private static void MoveVertical(Board board, GridCell[,] grid, bool isDown)
    {
        int width = board.Width;
        int height = board.Height;


        // isDown = true: Rơi xuống (từ trên xuống dưới)
        // isDown = false: Bay lên (từ dưới lên trên)
        int dir = isDown ? 1 : -1;
        int startY = isDown ? 0 : height - 1;
        int endY = isDown ? height : -1;

        for (int x = 0; x < width; x++)
        {
            for (int y = startY; y != endY; y += dir)
            {
                if (grid[x, y].IsEmpty)
                {
                    // Tìm tile đầu tiên ở phía trên/dưới ô trống này
                    for (int nextX = y + dir; nextX != endY; nextX += dir)
                    {
                        // Nếu gặp vật cản cố định (type 3), dừng lại
                        if (grid[x, nextX].IsObstacle) break;

                        if (grid[x, nextX].IsMatchable)
                        {
                            ExecuteMove(board, grid, x, nextX, x, y);
                            break;
                        }
                    }
                }
            }
        }
    }

    // --- LOGIC DI CHUYỂN NGANG (Trái/Phải) ---
    private static void MoveHorizontal(Board board, GridCell[,] grid, bool isLeft)
    {
        int width = board.Width;
        int height = board.Height;

        int dir = isLeft ? 1 : -1;
        int startX = isLeft ? 0 : width - 1;
        int endX = isLeft ? width : -1;

        int moveCount = 0;

        for (int y = 0; y < height; y++)
        {
            Debug.Log($"<color=gray>  Hàng {y}: quét từ {startX}...</color>");

            for (int x = startX; x != endX; x += dir)
            {
                if (grid[x, y].IsEmpty)
                {
                    for (int nextX = x + dir; nextX != endX; nextX += dir)
                    {
                        if (grid[nextX, y].IsObstacle) break;

                        if (grid[nextX, y].IsMatchable)
                        {
                            ExecuteMove(board, grid, nextX, y, x, y);
                            moveCount++;
                            break;
                        }
                    }
                }
            }
        }

        Debug.Log($"<color=green>✓ MoveHorizontal hoàn thành. Tổng di chuyển: {moveCount} tiles</color>");
    }

    //
    private static void MoveCenterCollapse(Board board, GridCell[,] grid)
    {
        int width = board.Width;
        int height = board.Height;
        int moveCount = 0;

        // 1. Xác định tọa độ ranh giới của vùng tâm (Ví dụ vùng 4x4 hoặc 2x2 tùy kích thước bàn)
        // Nếu bạn muốn cố định 4x4, hãy dùng: int centerRange = 2; 
        // Ở đây mình tính toán linh hoạt theo kích thước bàn:
        int midX = width / 2;
        int midY = height / 2;

        // HÚT THEO TRỤC X (Dồn vào giữa cột)
        for (int y = 0; y < height; y++)
        {
            // --- Nửa bên trái: Dồn sang PHẢI (về phía midX) ---
            for (int x = midX - 1; x >= 0; x--)
            {
                if (grid[x, y].IsEmpty)
                {
                    // Tìm tile từ phía bên trái của ô trống này (xa tâm hơn) để kéo về tâm
                    for (int nextX = x - 1; nextX >= 0; nextX--)
                    {
                        if (grid[nextX, y].IsObstacle) break;
                        if (grid[nextX, y].IsMatchable)
                        {
                            ExecuteMove(board, grid, nextX, y, x, y);
                            moveCount++;
                            break;
                        }
                    }
                }
            }

            // --- Nửa bên phải: Dồn sang TRÁI (về phía midX) ---
            for (int x = midX; x < width; x++)
            {
                if (grid[x, y].IsEmpty)
                {
                    // Tìm tile từ phía bên phải của ô trống này (xa tâm hơn) để kéo về tâm
                    for (int nextX = x + 1; nextX < width; nextX++)
                    {
                        if (grid[nextX, y].IsObstacle) break;
                        if (grid[nextX, y].IsMatchable)
                        {
                            ExecuteMove(board, grid, nextX, y, x, y);
                            moveCount++;
                            break;
                        }
                    }
                }
            }
        }

        // HÚT THEO TRỤC Y (Dồn vào giữa hàng)
        // Lưu ý: Sau khi dồn ngang, ta dồn dọc để các ô tập trung vào 4 ô trung tâm nhất
        for (int x = 0; x < width; x++)
        {
            // --- Nửa phía trên: Dồn xuống DƯỚI (về phía midY) ---
            for (int y = midY - 1; y >= 0; y--)
            {
                if (grid[x, y].IsEmpty)
                {
                    for (int nextY = y - 1; nextY >= 0; nextY--)
                    {
                        if (grid[x, nextY].IsObstacle) break;
                        if (grid[x, nextY].IsMatchable)
                        {
                            ExecuteMove(board, grid, x, nextY, x, y);
                            moveCount++;
                            break;
                        }
                    }
                }
            }

            // --- Nửa phía dưới: Dồn lên TRÊN (về phía midY) ---
            for (int y = midY; y < height; y++)
            {
                if (grid[x, y].IsEmpty)
                {
                    for (int nextY = y + 1; nextY < height; nextY++)
                    {
                        if (grid[x, nextY].IsObstacle) break;
                        if (grid[x, nextY].IsMatchable)
                        {
                            ExecuteMove(board, grid, x, nextY, x, y);
                            moveCount++;
                            break;
                        }
                    }
                }
            }
        }

        Debug.Log($"<color=green>✓ Center Collapse hoàn thành. Tổng di chuyển: {moveCount} tiles</color>");
    }
    

    private static void MoveSplitOutward(Board board, GridCell[,] grid)
    {
        int width = board.Width;
        int height = board.Height;
        int moveCount = 0;

        int centerX = width / 2;

        Debug.Log($"<color=gray>  Tâm bàn: x = {centerX}</color>");

        // ✅ NỬA TRÁI: Quét từ PHẢI sang TRÁI
        // Tiles ở nửa trái di chuyển sang TRÁI
        Debug.Log($"<color=cyan>  Xử lý nửa TRÁI (x < {centerX})</color>");
        for (int x = centerX - 1; x >= 0; x--)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y].IsEmpty)
                {
                    // Tìm tile phía PHẢI (hướng di chuyển ra)
                    for (int nextX = x + 1; nextX < width; nextX++)
                    {
                        if (grid[nextX, y].IsObstacle) break;

                        if (grid[nextX, y].IsMatchable)
                        {
                            ExecuteMove(board, grid, nextX, y, x, y);
                            moveCount++;
                            break;
                        }
                    }
                }
            }
        }

        // ✅ NỬA PHẢI: Quét từ TRÁI sang PHẢI
        // Tiles ở nửa phải di chuyển sang PHẢI
        Debug.Log($"<color=cyan>  Xử lý nửa PHẢI (x >= {centerX})</color>");
        for (int x = centerX; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y].IsEmpty)
                {
                    // Tìm tile phía TRÁI (hướng di chuyển ra)
                    for (int nextX = x - 1; nextX >= 0; nextX--)
                    {
                        if (grid[nextX, y].IsObstacle) break;

                        if (grid[nextX, y].IsMatchable)
                        {
                            ExecuteMove(board, grid, nextX, y, x, y);
                            moveCount++;
                            break;
                        }
                    }
                }
            }
        }

        Debug.Log($"<color=green>✓ MoveSplitOutward hoàn thành. Tổng di chuyển: {moveCount} tiles</color>");
    }

  
}