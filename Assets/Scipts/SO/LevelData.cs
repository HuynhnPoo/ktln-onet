using System;
using System.Collections.Generic;
using UnityEngine;

// ─────────────────────────────────────────────────────────────────────────────
//  BoardGravityType — kiểu di chuyển tile sau khi match
//  Thay thế MovementType cũ (chỉ có 4 option không đủ dùng)
// ─────────────────────────────────────────────────────────────────────────────
public enum BoardGravityType
{
    None,           // Tile đứng yên — level cơ bản
    GravityDown,    // Tile phía trên rơi xuống lấp chỗ trống (phổ biến nhất)
    GravityUp,      // Tile phía dưới bay lên
    ShiftLeft,      // Cả hàng dồn sang trái
    ShiftRight,     // Cả hàng dồn sang phải
    CenterCollapse, // Tile 2 bên dồn vào tâm lưới
    SplitOutward,   // Tile tách ra 2 rìa (ngược CenterCollapse)
    Mixed,          // Cột chẵn rơi xuống, cột lẻ bay lên — khó nhất
}

// ─────────────────────────────────────────────────────────────────────────────
//  LevelData — ScriptableObject chứa toàn bộ config 1 màn chơi
//  Tạo asset: chuột phải → Create → Onet → Level Data
// ─────────────────────────────────────────────────────────────────────────────
[CreateAssetMenu(fileName = "LevelData", menuName = "Level/Level Data")]
public class LevelData : ScriptableObject
{
    // ══════════════════════════════════════════════════════════════════
    //  THÔNG TIN CƠ BẢN
    // ══════════════════════════════════════════════════════════════════

    [Tooltip("ID duy nhất của level (dùng để load/save tiến trình)")]
    public int levelID = 1;

    [Tooltip("Tên hiển thị trong Editor và UI")]
    public string levelName = "Level 1";

    // ══════════════════════════════════════════════════════════════════
    //  BACKGROUND & GIAO DIỆN
    // ══════════════════════════════════════════════════════════════════

    [Tooltip("Sprite nền của màn chơi")]
    public Sprite backgroundSprite;

    [Tooltip("Màu overlay phủ lên nền (alpha = 0 = không phủ)")]
    public Color backgroundOverlay = Color.clear;

    // ══════════════════════════════════════════════════════════════════
    //  KÍCH THƯỚC LƯỚI
    // ══════════════════════════════════════════════════════════════════

    [Min(2), Tooltip("Số ô theo chiều ngang")]
    public int gridWidth = 6;

    [Min(2), Tooltip("Số ô theo chiều dọc")]
    public int gridHeight = 6;

    [Range(0.1f, 5f), Tooltip("Kích thước 1 ô (Unity units)")]
    public float cellSize = 1f;

    [Range(0f, 1f), Tooltip("Khoảng cách giữa các ô (Unity units). 0 = sát nhau.")]
    public float spacing = 0.1f;

    // ══════════════════════════════════════════════════════════════════
    //  DỮ LIỆU LƯỚI (danh sách ô, lưu theo thứ tự x + y*width)
    // ══════════════════════════════════════════════════════════════════

    public List<GridCell> cells = new List<GridCell>();

    // ══════════════════════════════════════════════════════════════════
    //  GIỚI HẠN CHƠI
    // ══════════════════════════════════════════════════════════════════

    [Tooltip("Bật để có thời gian giới hạn")]
    public bool useTimeLimit = true;

    [Min(5f), Tooltip("Thời gian tối đa (giây). Chỉ dùng khi useTimeLimit = true.")]
    public float timeLimit = 60f;

    // ── Thông tin type ──────────────────────────────────────────────

    [Min(1), Tooltip("Số loại icon khác nhau cho Normal tile (type 1). Obstacle không tính.")]
    public int maxNormalTypes = 6;

    [Range(0, 4), Tooltip("Số cặp Boost (type 2) xuất hiện: 0–4 cặp (0–8 ô). Không tính vào maxNormalTypes.")]
    public int maxBoostPairs = 1;

    // ══════════════════════════════════════════════════════════════════
    //  ĐIỂM THƯỞNG & XẾP HẠNG SAO
    // ══════════════════════════════════════════════════════════════════

    [Tooltip("Nếu bật: phải đạt ít nhất mốc 1 sao mới qua level.\nNếu tắt: xóa hết ô là thắng, điểm chỉ để xếp sao.")]
    public bool requireStarToWin = true;

    // ── Điểm base mỗi match ─────────────────────────────────────────

    [Min(0), Tooltip("Điểm nhận được khi match 1 cặp Normal tile (type 1)")]
    public int scorePerNormalMatch = 100;

    [Min(0), Tooltip("Điểm nhận được khi match 1 cặp Boost tile (type 2). Thường cao hơn Normal.")]
    public int scorePerBoostMatch = 250;

    // ── Combo ───────────────────────────────────────────────────────

    [Tooltip("Bật/tắt hệ thống nhân điểm theo combo liên tiếp")]
    public bool useCombo = true;

    [Range(0f, 2f), Tooltip("Mỗi combo liên tiếp tăng thêm X lần điểm base.\n0.5 → x2 = +50%,  x3 = +100%,  x4 = +150%...")]
    public float comboMultiplierStep = 0.5f;

    [Min(0f), Tooltip("Combo reset về x1 nếu không match trong N giây. 0 = không reset theo thời gian.")]
    public float comboResetDelay = 3f;

    [Range(0, 20), Tooltip("Combo tối đa (cap). 0 = không giới hạn.")]
    public int maxComboCount = 0;

    // ── Mốc điểm xếp sao ───────────────────────────────────────────

    [Min(0), Tooltip("Điểm tối thiểu để đạt 1 sao (= điểm thắng tối thiểu nếu requireStarToWin = true)")]
    public int scoreStar1 = 500;

    [Min(0), Tooltip("Điểm tối thiểu để đạt 2 sao")]
    public int scoreStar2 = 1000;

    [Min(0), Tooltip("Điểm tối thiểu để đạt 3 sao")]
    public int scoreStar3 = 2000;

    // ══════════════════════════════════════════════════════════════════
    //  ĐỘ KHÓ — KIỂU DI CHUYỂN TILE (thay MovementType cũ)
    // ══════════════════════════════════════════════════════════════════

    [Tooltip("Kiểu di chuyển tile sau mỗi lần match.\nNone = đứng yên, GravityDown = rơi xuống, v.v.")]
    public BoardGravityType gravityType = BoardGravityType.None;

    [Range(0.5f, 20f), Tooltip("Tốc độ tile trượt về vị trí mới (units/giây). Chỉ dùng khi gravityType != None.")]
    public float gravitySpeed = 8f;

    [Tooltip("Nếu bật: tile tràn ra đầu kia thay vì dừng lại tại rìa.\nVD: ShiftLeft → tile bên trái nhất xuất hiện lại bên phải.")]
    public bool gravityLoop = false;

    // ══════════════════════════════════════════════════════════════════
    //  METHODS — GRID HELPERS
    // ══════════════════════════════════════════════════════════════════

    /// <summary>Đảm bảo danh sách cells đúng kích thước gridWidth × gridHeight.
    /// Gọi khi thay đổi kích thước lưới hoặc lần đầu tạo asset.</summary>
    public void EnsureGridSize()
    {
        int needed = gridWidth * gridHeight;

        // Thêm ô mới nếu thiếu
        while (cells.Count < needed)
            cells.Add(new GridCell());

        // Cắt bớt nếu thừa
        if (cells.Count > needed)
            cells.RemoveRange(needed, cells.Count - needed);
    }

    /// <summary>Trả về GridCell tại toạ độ (x, y).
    /// x = cột (0 = trái), y = hàng (0 = dưới cùng).
    /// Trả về null nếu out-of-bounds.</summary>
    public GridCell GetCell(int x, int y)
    {
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight)
            return null;

        int index = y * gridWidth + x;
        if (index < 0 || index >= cells.Count)
            return null;

        return cells[index];
    }

    /// <summary>Set type cho ô (x, y). An toàn — tự bỏ qua nếu out-of-bounds.</summary>
    public void SetCell(int x, int y, int type, int iconID = 0)
    {
        var cell = GetCell(x, y);
        if (cell == null) return;
        cell.type = type;
        cell.iconID = iconID;
    }

    // ══════════════════════════════════════════════════════════════════
    //  METHODS — SCORE HELPERS
    // ══════════════════════════════════════════════════════════════════

    /// <summary>Tính điểm thực tế cho 1 lần match, có tính combo.</summary>
    /// <param name="tileType">1 = Normal, 2 = Boost</param>
    /// <param name="comboCount">Số combo liên tiếp hiện tại (bắt đầu từ 1)</param>
    public int CalculateMatchScore(int tileType, int comboCount)
    {
        int baseScore = tileType == 2 ? scorePerBoostMatch : scorePerNormalMatch;

        if (!useCombo || comboCount <= 1)
            return baseScore;

        // Cap combo nếu cần
        int effectiveCombo = (maxComboCount > 0)
            ? Mathf.Min(comboCount, maxComboCount)
            : comboCount;

        float multiplier = 1f + comboMultiplierStep * (effectiveCombo - 1);
        return Mathf.RoundToInt(baseScore * multiplier);
    }

    /// <summary>Trả về số sao (0–3) tương ứng với điểm số.</summary>
    public int GetStarRating(int score)
    {
        if (score >= scoreStar3) return 3;
        if (score >= scoreStar2) return 2;
        if (score >= scoreStar1) return 1;
        return 0;
    }

    /// <summary>Kiểm tra người chơi có đủ điều kiện thắng không.</summary>
    public bool IsWinConditionMet(int score, int remainingMatchableTiles)
    {
        bool allCleared = remainingMatchableTiles <= 0;
        if (!allCleared) return false;
        if (requireStarToWin) return score >= scoreStar1;
        return true;
    }

    // ══════════════════════════════════════════════════════════════════
    //  PROPERTIES — QUICK INFO
    // ══════════════════════════════════════════════════════════════════

    public int TotalCells => gridWidth * gridHeight;
    public bool TotalIsEven => TotalCells % 2 == 0;
    public float StepSize => cellSize + spacing;
    public float GridWorldWidth => gridWidth * StepSize - spacing;
    public float GridWorldHeight => gridHeight * StepSize - spacing;

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Đảm bảo mốc sao tăng dần
        scoreStar2 = Mathf.Max(scoreStar2, scoreStar1 + 1);
        scoreStar3 = Mathf.Max(scoreStar3, scoreStar2 + 1);

        // Đảm bảo grid luôn đủ ô khi resize trong Inspector
        EnsureGridSize();
    }
#endif
}