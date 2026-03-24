using UnityEngine;
using System.Collections.Generic;


public enum MovementType
{
    None = 0,
    CircularSpiral = 1,
    RowColumnShift = 2,
    FillToCenter = 3,
}

[CreateAssetMenu(fileName = "LevelData", menuName = "Level/LevelData")]
public class LevelData : ScriptableObject
{
    [Header("Thông tin cơ bản")]
    public int levelID = 1;
    public string levelName = "Level 1";

    [Header("Background & Giao diện")]
    [Tooltip("Sprite nền của level")]
    public Sprite backgroundSprite;
    [Tooltip("Màu overlay phủ lên background. Alpha = 0 → không dùng.")]
    public Color backgroundOverlay = new Color(0f, 0f, 0f, 0f);

    // ── Kích thước lưới ──────────────────────────────────────────────
    [Header("Kích thước lưới")]
    public int gridWidth = 10;
    public int gridHeight = 8;

    [Tooltip("Kích thước mỗi ô theo đơn vị Unity (world units). Dùng khi spawn tile trong game.")]
    public float cellSize = 1f;

    [Tooltip("Khoảng cách giữa các ô (world units). 0 = sát nhau.")]
    public float spacing = 0.1f;

    [Header("Giới hạn chơi")]
    public bool useTimeLimit = true;
    public float timeLimit = 300f;
    public bool useMoveLimit = false;
    public int maxMoves = 50;
    public int maxBoostPairs = 0; // 2 đến 4 cặp Boost

    [Header("Độ khó — Kiểu di chuyển")]
    public MovementType movementType = MovementType.None;
    public float movementSpeed = 1f;
    [Range(-1, 1)]
    public int movementDirection = 1;

    [Header("Dữ liệu lưới")]
    public List<GridCell> cells = new List<GridCell>();

    [Header("Thông tin type")]
    [Tooltip("Số loại icon match tối đa")]
    public int maxNormalTypes = 8;

    // ── Tính toán tiện ích ────────────────────────────────────────────
    /// <summary>Tổng kích thước lưới theo trục X (world units)</summary>
    public float TotalWidth => gridWidth * (cellSize + spacing) - spacing;
    /// <summary>Tổng kích thước lưới theo trục Y (world units)</summary>
    public float TotalHeight => gridHeight * (cellSize + spacing) - spacing;

    /// <summary>Lấy vị trí world của tâm ô (x, y), gốc lưới ở (0,0)</summary>
    public Vector2 GetCellWorldPosition(int x, int y)
    {
        float step = cellSize + spacing;
        return new Vector2(x * step, y * step);
    }

    // ── Helpers ──────────────────────────────────────────────────────
    public void EnsureGridSize()
    {
        if (cells == null) cells = new List<GridCell>();
        int required = gridWidth * gridHeight;
        if (cells.Count != required)
        {
            cells.Clear();
            for (int y = 0; y < gridHeight; y++)
                for (int x = 0; x < gridWidth; x++)
                    cells.Add(new GridCell { x = x, y = y, type = 0 });
        }
    }

    public GridCell GetCell(int x, int y)
    {
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight) return null;
        int index = y * gridWidth + x;
        return (index >= 0 && index < cells.Count) ? cells[index] : null;
    }

    public void SetCell(int x, int y, int newType)
    {
        var cell = GetCell(x, y);
        if (cell != null) cell.type = newType;
    }
}

