using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class LevelGridEditorWindow : EditorWindow
{
    // ── State ─────────────────────────────────────────────────────────
    private LevelData level;
    private Vector2 scrollPos;
    private int selectedType = 1;

    // ── Drag ─────────────────────────────────────────────────────────
    private bool isDragging = false;
    private int dragPaintType = -1;

    // ── Display options ───────────────────────────────────────────────
    private enum IndexMode { Off, FlatIndex, XY2D }
    private IndexMode indexMode = IndexMode.XY2D;
    private int cellPx = 52;

    // ── Palette — thêm màu cho type 4,5 nếu cần mở rộng ─────────────
    private static readonly Color[] TypeColors =
    {
        new Color(0.18f, 0.18f, 0.18f), // 0 Empty
        new Color(0.45f, 0.72f, 1.00f), // 1 Normal
        new Color(1.00f, 0.82f, 0.25f), // 2 Boost
        new Color(0.90f, 0.28f, 0.28f), // 3 Obstacle
        new Color(0.35f, 0.88f, 0.48f), // 4 (mở rộng)
        new Color(0.78f, 0.48f, 1.00f), // 5 (mở rộng)
    };

    private static readonly string[] TypeLabels =
        { "Erase(0)", "Normal(1)", "Boost(2)", "Obstacle(3)", "TypeD(4)", "TypeE(5)" };

    // ─────────────────────────────────────────────────────────────────
    public static void Open(LevelData levelData)
    {
        if (levelData == null) return;
        var w = GetWindow<LevelGridEditorWindow>("Grid: " + levelData.levelName);
        w.level = levelData;
        w.minSize = new Vector2(920, 760);
        levelData.EnsureGridSize();
    }

    // ─────────────────────────────────────────────────────────────────
    private void OnGUI()
    {
        if (level == null) { Close(); return; }

        // Header
        EditorGUILayout.LabelField(
            $"🐭  GRID EDITOR — {level.levelName}   ({level.gridWidth} × {level.gridHeight})   " +
            $"[{level.TotalCells} ô {(level.TotalIsEven ? "✔ chẵn" : "✘ lẻ")}]",
            EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        DrawPalette();
        EditorGUILayout.Space(5);
        DrawOptionsBar();
        EditorGUILayout.Space(3);
        DrawToolbar();
        EditorGUILayout.Space(5);

        EditorGUILayout.LabelField(
            "Drag trái = vẽ   |   Drag phải = xóa (Erase)   |   Scroll = cuộn lưới",
            EditorStyles.centeredGreyMiniLabel);
        EditorGUILayout.Space(3);

        DrawGrid();

        DrawStatBar();

        if (GUI.changed) EditorUtility.SetDirty(level);
    }

    // ─── Palette ──────────────────────────────────────────────────────
    private void DrawPalette()
    {
        EditorGUILayout.LabelField("Chọn Type:", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        for (int t = 0; t < TypeLabels.Length; t++)
        {
            bool sel = (selectedType == t);
            GUI.backgroundColor = TypeColors[t];
            var s = new GUIStyle(GUI.skin.button)
            {
                fontStyle = sel ? FontStyle.Bold : FontStyle.Normal,
                fontSize = sel ? 12 : 11,
            };
            if (sel) s.normal.textColor = Color.black;
            string lbl = TypeLabels[t] + (sel ? "\n◀ active" : "\n");
            if (GUILayout.Button(lbl, s, GUILayout.Width(100), GUILayout.Height(50)))
                selectedType = t;
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();

        // Thanh hiển thị type đang chọn
        int si = Mathf.Clamp(selectedType, 0, TypeColors.Length - 1);
        GUI.backgroundColor = TypeColors[si];
        EditorGUILayout.LabelField(
            $"  ✏️  Đang vẽ: {TypeLabels[si]}",
            EditorStyles.helpBox, GUILayout.Height(22));
        GUI.backgroundColor = Color.white;
    }

    // ─── Options bar ─────────────────────────────────────────────────
    private void DrawOptionsBar()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Index hiển thị:", GUILayout.Width(102));
        if (GUILayout.Toggle(indexMode == IndexMode.Off, "Ẩn", "Button", GUILayout.Width(52))) indexMode = IndexMode.Off;
        if (GUILayout.Toggle(indexMode == IndexMode.FlatIndex, "Flat [i]", "Button", GUILayout.Width(68))) indexMode = IndexMode.FlatIndex;
        if (GUILayout.Toggle(indexMode == IndexMode.XY2D, "[x , y]", "Button", GUILayout.Width(68))) indexMode = IndexMode.XY2D;

        GUILayout.Space(20);
        EditorGUILayout.LabelField("Cell px:", GUILayout.Width(52));
        cellPx = EditorGUILayout.IntSlider(cellPx, 30, 90, GUILayout.Width(170));

        EditorGUILayout.EndHorizontal();
    }

    // ─── Toolbar ─────────────────────────────────────────────────────
    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("🧹  Clear All", GUILayout.Height(28), GUILayout.Width(110)))
        {
            if (EditorUtility.DisplayDialog("Clear All",
                    $"Xóa toàn bộ {level.TotalCells} ô về Empty?", "Xóa", "Hủy"))
                ClearAll();
        }

        if (GUILayout.Button("🎲  Random (cân bằng)", GUILayout.Height(28), GUILayout.Width(175)))
            RandomEvenTypes();

        if (GUILayout.Button("🔄  Resize & Refresh", GUILayout.Height(28), GUILayout.Width(138)))
        {
            level.EnsureGridSize();
            EditorUtility.SetDirty(level);
        }

        if (GUILayout.Button("📋  Copy từ Level khác", GUILayout.Height(28), GUILayout.Width(155)))
            ShowCopyFromDialog();

        EditorGUILayout.EndHorizontal();
    }

    // ─── Grid (manual Rect với scroll) ───────────────────────────────
    private void DrawGrid()
    {
        Event e = Event.current;
        int gap = 2;
        int cs = cellPx;
        int cols = level.gridWidth;
        int rows = level.gridHeight;

        float gridW = cols * (cs + gap);
        float gridH = rows * (cs + gap);
        float maxH = Mathf.Max(80, Mathf.Min(gridH + 20, position.height - 300));

        Rect scrollRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none,
            GUILayout.ExpandWidth(true), GUILayout.Height(maxH));

        scrollPos = GUI.BeginScrollView(scrollRect, scrollPos,
                        new Rect(0, 0, gridW + 10, gridH + 10));

        var styleCenter = new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = Mathf.Max(11, cs / 4),
            fontStyle = FontStyle.Bold,
        };
        var styleCorner = new GUIStyle(EditorStyles.miniLabel)
        {
            alignment = TextAnchor.UpperLeft,
            fontSize = Mathf.Max(7, cs / 6),
        };

        for (int row = 0; row < rows; row++)
        {
            // row 0 vẽ ở đáy (y=0), row cao nhất vẽ ở trên
            int y = rows - 1 - row;

            for (int x = 0; x < cols; x++)
            {
                GridCell cell = level.GetCell(x, y);
                if (cell == null) continue;

                int ct = Mathf.Clamp(cell.type, 0, TypeColors.Length - 1);
                Rect rect = new Rect(x * (cs + gap), row * (cs + gap), cs, cs);
                Color col = TypeColors[ct];

                EditorGUI.DrawRect(rect, col);
                DrawCellBorder(rect,
                    cell.type == 0
                        ? new Color(0.45f, 0.45f, 0.45f, 0.45f)
                        : new Color(0f, 0f, 0f, 0.25f), 1);

                // Type label ở giữa ô (chỉ khi không phải Empty)
                if (cell.type != 0)
                {
                    styleCenter.normal.textColor = Color.black;
                    GUI.Label(rect, cell.type.ToString(), styleCenter);
                }

                // Index label ở góc trên trái
                if (indexMode != IndexMode.Off)
                {
                    string idx = indexMode == IndexMode.XY2D
                        ? $"{x},{y}"
                        : $"{y * cols + x}";
                    Rect cornerRect = new Rect(rect.x + 2, rect.y + 1, rect.width - 3, rect.height / 2f);
                    styleCorner.normal.textColor = cell.type == 0
                        ? new Color(0.60f, 0.60f, 0.60f)
                        : new Color(0.10f, 0.10f, 0.10f, 0.70f);
                    GUI.Label(cornerRect, idx, styleCorner);
                }

                // ── Xử lý chuột ──────────────────────────────────
                if (e.isMouse && rect.Contains(e.mousePosition))
                {
                    if (e.type == EventType.MouseDown)
                    {
                        isDragging = true;
                        dragPaintType = (e.button == 1) ? 0 : selectedType;
                        PaintCell(x, y, dragPaintType);
                        e.Use(); Repaint();
                    }
                    else if (e.type == EventType.MouseDrag && isDragging)
                    {
                        PaintCell(x, y, dragPaintType);
                        e.Use(); Repaint();
                    }
                }
            }
        }

        GUI.EndScrollView();

        if (e.type == EventType.MouseUp && isDragging)
        {
            isDragging = false; dragPaintType = -1; Repaint();
        }
        if (isDragging) Repaint();
    }

    // ─── Stat bar dưới lưới ──────────────────────────────────────────
    private void DrawStatBar()
    {
        EditorGUILayout.Space(4);

        var count = new Dictionary<int, int>();
        foreach (var c in level.cells)
        {
            if (!count.ContainsKey(c.type)) count[c.type] = 0;
            count[c.type]++;
        }

        int normal = count.ContainsKey(1) ? count[1] : 0;
        int boost = count.ContainsKey(2) ? count[2] : 0;
        int obstacle = count.ContainsKey(3) ? count[3] : 0;
        int empty = count.ContainsKey(0) ? count[0] : 0;

        bool normalOk = normal % 2 == 0;
        bool boostOk = boost % 2 == 0;
        bool allMatchOk = normalOk && boostOk;

        var style = new GUIStyle(EditorStyles.helpBox) { fontSize = 11, richText = true };
        EditorGUILayout.LabelField(
            $"<b>Normal  (1):</b> {normal} ô {(normalOk ? "✔" : "✘ lẻ!")}   " +
            $"<b>Boost   (2):</b> {boost} ô {(boostOk ? "✔" : "✘ lẻ!")}   " +
            $"<b>Obstacle(3):</b> {obstacle} ô   " +
            $"<b>Empty   (0):</b> {empty} ô   " +
            $"{(allMatchOk ? "✅ Có thể match" : "⚠️ Tile lẻ — cần chẵn để match!")}",
            style, GUILayout.MinHeight(26));
    }

    // ─────────────────────────────────────────────────────────────────
    //  HELPERS
    // ─────────────────────────────────────────────────────────────────
    private void DrawCellBorder(Rect r, Color c, float w)
    {
        EditorGUI.DrawRect(new Rect(r.x, r.y, r.width, w), c);
        EditorGUI.DrawRect(new Rect(r.x, r.yMax - w, r.width, w), c);
        EditorGUI.DrawRect(new Rect(r.x, r.y, w, r.height), c);
        EditorGUI.DrawRect(new Rect(r.xMax - w, r.y, w, r.height), c);
    }

    private void PaintCell(int x, int y, int type)
    {
        var cell = level.GetCell(x, y);
        if (cell != null && cell.type != type)
        {
            Undo.RecordObject(level, "Paint Cell");
            cell.type = type;
            EditorUtility.SetDirty(level);
        }
    }

    private void ClearAll()
    {
        Undo.RecordObject(level, "Clear Grid");
        foreach (var c in level.cells) c.type = 0;
        EditorUtility.SetDirty(level);
        Repaint();
    }

    // ─── Copy grid từ level khác ─────────────────────────────────────
    private void ShowCopyFromDialog()
    {
        string path = EditorUtility.OpenFilePanel("Chọn LevelData để copy grid", "Assets", "asset");
        if (string.IsNullOrEmpty(path)) return;

        // Chuyển absolute → relative path
        if (path.StartsWith(Application.dataPath))
            path = "Assets" + path.Substring(Application.dataPath.Length);

        var src = AssetDatabase.LoadAssetAtPath<LevelData>(path);
        if (src == null)
        {
            EditorUtility.DisplayDialog("Lỗi", "Không load được LevelData từ path đã chọn.", "OK");
            return;
        }
        if (src.gridWidth != level.gridWidth || src.gridHeight != level.gridHeight)
        {
            EditorUtility.DisplayDialog("Lỗi",
                $"Kích thước lưới không khớp!\n" +
                $"  Source: {src.gridWidth}×{src.gridHeight}\n" +
                $"  Target: {level.gridWidth}×{level.gridHeight}", "OK");
            return;
        }

        Undo.RecordObject(level, "Copy Grid from Level");
        for (int i = 0; i < Mathf.Min(level.cells.Count, src.cells.Count); i++)
        {
            level.cells[i].type = src.cells[i].type;
            level.cells[i].iconID = src.cells[i].iconID;
        }
        EditorUtility.SetDirty(level);
        Repaint();
        Debug.Log($"✅ Copy grid từ [{src.levelName}] sang [{level.levelName}] thành công.");
    }

    // ─── Random cân bằng ─────────────────────────────────────────────
    // Obstacle(3) giữ nguyên vị trí
    // Normal(1) phân bổ đều theo maxNormalTypes
    // Boost(2) theo maxBoostPairs
    private void RandomEvenTypes()
    {
        int total = level.gridWidth * level.gridHeight;

        // 1. Ghi nhớ vị trí obstacle
        var obstacleIdx = new HashSet<int>();
        for (int i = 0; i < level.cells.Count; i++)
            if (level.cells[i].type == 3)
                obstacleIdx.Add(i);

        int freeCells = total - obstacleIdx.Count;

        // 2. Số cặp Boost — clamp theo maxBoostPairs
        int boostPairs = Mathf.Clamp(level.maxBoostPairs, 0, 4);
        int boostCount = boostPairs * 2;

        // 3. Kiểm tra đủ ô
        int normalCells = freeCells - boostCount;
        if (normalCells < 2 || normalCells % 2 != 0)
        {
            EditorUtility.DisplayDialog("Không thể Random",
                $"Không đủ ô cho Normal!\n" +
                $"  Ô trống: {freeCells}\n" +
                $"  Boost cần: {boostCount} ô ({boostPairs} cặp)\n" +
                $"  Còn lại cho Normal: {normalCells} ô\n\n" +
                "Hãy giảm số cặp Boost, giảm Obstacle, hoặc tăng kích thước grid.", "OK");
            return;
        }

        // 4. Tạo pool Normal theo maxNormalTypes
        int numTypes = Mathf.Clamp(level.maxNormalTypes, 1, 999);
        int normalPairs = normalCells / 2;
        if (normalPairs < numTypes) numTypes = normalPairs;

        int basePairs = normalPairs / numTypes;
        int extraPairs = normalPairs % numTypes;

        var pool = new List<int>(freeCells);
        for (int t = 1; t <= numTypes; t++)
        {
            int pairs = basePairs + (t <= extraPairs ? 1 : 0);
            for (int i = 0; i < pairs * 2; i++) pool.Add(1); // Normal
        }

        // 5. Thêm Boost
        for (int i = 0; i < boostCount; i++) pool.Add(2);

        // 6. Shuffle Fisher-Yates
        for (int i = pool.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (pool[i], pool[j]) = (pool[j], pool[i]);
        }

        // 7. Ghi vào cells, bỏ qua obstacle
        Undo.RecordObject(level, "Random Grid");
        int poolIdx = 0;
        for (int i = 0; i < level.cells.Count; i++)
        {
            if (obstacleIdx.Contains(i)) continue;
            level.cells[i].type = poolIdx < pool.Count ? pool[poolIdx++] : 0;
        }

        EditorUtility.SetDirty(level);
        Repaint();

        // 8. Log kết quả
        var map = new Dictionary<int, int>();
        foreach (var c in level.cells) { if (!map.ContainsKey(c.type)) map[c.type] = 0; map[c.type]++; }
        Debug.Log(
            $"✅ Random xong — {total} ô tổng\n" +
            $"  Normal   (1): {(map.ContainsKey(1) ? map[1] : 0)} ô → {numTypes} loại  ✔ chẵn\n" +
            $"  Boost    (2): {(map.ContainsKey(2) ? map[2] : 0)} ô → {boostPairs} cặp  ✔ chẵn\n" +
            $"  Obstacle (3): {obstacleIdx.Count} ô → giữ nguyên\n" +
            $"  Empty    (0): {(map.ContainsKey(0) ? map[0] : 0)} ô");
    }
}