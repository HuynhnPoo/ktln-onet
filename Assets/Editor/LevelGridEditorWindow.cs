using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class LevelGridEditorWindow : EditorWindow
{
    private LevelData level;
    private Vector2 scrollPosGrid;
    private int selectedType = 1;

    // ── Drag state ────────────────────────────────────────────────────
    private bool isDragging = false;
    private int dragPaintType = -1;

    // ── Display options ───────────────────────────────────────────────
    private enum IndexMode { Off, FlatIndex, XY2D }
    private IndexMode indexMode = IndexMode.XY2D;
    private int cellSize = 52;

    // ── Palette ───────────────────────────────────────────────────────
    private readonly Color[] typeColors =
    {
        new Color(0.20f, 0.20f, 0.20f), // 0 Empty
        new Color(0.45f, 0.72f, 1.00f), // 1 Normal
        new Color(1.00f, 0.82f, 0.25f), // 2 Boost
        new Color(0.90f, 0.28f, 0.28f), // 3 Obstacle
        new Color(0.35f, 0.88f, 0.48f), // 4 Custom A
        new Color(0.78f, 0.48f, 1.00f), // 5 Custom B
    };
    private readonly string[] typeLabels = { "Erase", "Normal", "Boost", "Obstacle", "TypeD", "TypeE" };

    // ─────────────────────────────────────────────────────────────────
    public static void Open(LevelData levelData)
    {
        if (levelData == null) return;
        var w = GetWindow<LevelGridEditorWindow>("Grid: " + levelData.levelName);
        w.level = levelData;
        w.minSize = new Vector2(900, 760);
        levelData.EnsureGridSize();
    }

    // ─────────────────────────────────────────────────────────────────
    private void OnGUI()
    {
        if (level == null) { Close(); return; }

        EditorGUILayout.LabelField(
            $"🐭  GRID EDITOR — {level.levelName}   ({level.gridWidth} × {level.gridHeight})",
            EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        DrawPalette();
        EditorGUILayout.Space(5);
        DrawOptionsBar();
        EditorGUILayout.Space(3);
        DrawToolbar();
        EditorGUILayout.Space(5);

        EditorGUILayout.LabelField(
            "Drag trái = vẽ   |   Drag phải = xóa",
            EditorStyles.centeredGreyMiniLabel);
        EditorGUILayout.Space(3);

        DrawGridManual();

        if (GUI.changed) EditorUtility.SetDirty(level);
    }

    // ─── Palette ─────────────────────────────────────────────────────
    private void DrawPalette()
    {
        EditorGUILayout.LabelField("Chọn Type:", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        int count = Mathf.Min(typeLabels.Length, typeColors.Length);
        for (int t = 0; t < count; t++)
        {
            bool sel = (selectedType == t);
            GUI.backgroundColor = typeColors[t];
            var s = new GUIStyle(GUI.skin.button)
            {
                fontStyle = sel ? FontStyle.Bold : FontStyle.Normal,
                fontSize = sel ? 12 : 11,
            };
            if (sel) s.normal.textColor = Color.black;
            string lbl = $"{typeLabels[t]}\n({t})" + (sel ? " ◀" : "");
            if (GUILayout.Button(lbl, s, GUILayout.Width(95), GUILayout.Height(54)))
                selectedType = t;
        }
        GUI.backgroundColor = Color.white;
        EditorGUILayout.EndHorizontal();

        GUI.backgroundColor = typeColors[Mathf.Clamp(selectedType, 0, typeColors.Length - 1)];
        EditorGUILayout.LabelField(
            $"  ✏️  Đang vẽ: {typeLabels[Mathf.Clamp(selectedType, 0, typeLabels.Length - 1)]}  (type {selectedType})",
            EditorStyles.helpBox);
        GUI.backgroundColor = Color.white;
    }

    // ─── Options bar ─────────────────────────────────────────────────
    private void DrawOptionsBar()
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Index hiển thị:", GUILayout.Width(100));
        if (GUILayout.Toggle(indexMode == IndexMode.Off, "Ẩn", "Button", GUILayout.Width(55))) indexMode = IndexMode.Off;
        if (GUILayout.Toggle(indexMode == IndexMode.FlatIndex, "Flat [i]", "Button", GUILayout.Width(72))) indexMode = IndexMode.FlatIndex;
        if (GUILayout.Toggle(indexMode == IndexMode.XY2D, "[x , y]", "Button", GUILayout.Width(72))) indexMode = IndexMode.XY2D;

        GUILayout.Space(20);
        EditorGUILayout.LabelField("Cell px:", GUILayout.Width(52));
        cellSize = EditorGUILayout.IntSlider(cellSize, 32, 90, GUILayout.Width(170));

        EditorGUILayout.EndHorizontal();
    }

    // ─── Toolbar ─────────────────────────────────────────────────────
    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("🧹 Clear All", GUILayout.Height(27), GUILayout.Width(115))) ClearGrid();
        if (GUILayout.Button("🎲 Random (chẵn từng type)", GUILayout.Height(27), GUILayout.Width(205))) RandomEvenTypes();
        if (GUILayout.Button("🔄 Resize & Refresh", GUILayout.Height(27), GUILayout.Width(140))) level.EnsureGridSize();
        EditorGUILayout.EndHorizontal();
    }

    // ─── Grid (manual Rect) ──────────────────────────────────────────
    private void DrawGridManual()
    {
        Event e = Event.current;
        int gap = 2;
        int cs = cellSize;
        int cols = level.gridWidth;
        int rows = level.gridHeight;

        float gridW = cols * (cs + gap);
        float gridH = rows * (cs + gap);
        float maxH = Mathf.Max(80, Mathf.Min(gridH + 20, position.height - 285));

        Rect scrollRect = GUILayoutUtility.GetRect(
            GUIContent.none, GUIStyle.none,
            GUILayout.ExpandWidth(true),
            GUILayout.Height(maxH));

        scrollPosGrid = GUI.BeginScrollView(scrollRect, scrollPosGrid,
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
            fontStyle = FontStyle.Normal,
        };

        for (int row = 0; row < rows; row++)
        {
            int y = rows - 1 - row;

            for (int x = 0; x < cols; x++)
            {
                GridCell cell = level.GetCell(x, y);
                if (cell == null) continue;

                int ct = Mathf.Clamp(cell.type, 0, typeColors.Length - 1);
                Rect rect = new Rect(x * (cs + gap), row * (cs + gap), cs, cs);

                EditorGUI.DrawRect(rect, typeColors[ct]);

                DrawBorder(rect,
                    cell.type == 0
                        ? new Color(0.45f, 0.45f, 0.45f, 0.55f)
                        : new Color(0f, 0f, 0f, 0.30f), 1);

                if (cell.type != 0)
                {
                    styleCenter.normal.textColor = Color.black;
                    GUI.Label(rect, cell.type.ToString(), styleCenter);
                }

                if (indexMode != IndexMode.Off)
                {
                    string idxLabel = indexMode == IndexMode.XY2D
                        ? $"{x},{y}"
                        : $"{y * cols + x}";

                    Rect cornerRect = new Rect(rect.x + 2, rect.y + 1, rect.width - 3, rect.height / 2);
                    styleCorner.normal.textColor = cell.type == 0
                        ? new Color(0.60f, 0.60f, 0.60f)
                        : new Color(0.10f, 0.10f, 0.10f, 0.75f);
                    GUI.Label(cornerRect, idxLabel, styleCorner);
                }

                // ── Mouse ──────────────────────────────────────────
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

    // ─── Helpers ─────────────────────────────────────────────────────
    private void DrawBorder(Rect r, Color c, float w)
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
            cell.type = type;
            EditorUtility.SetDirty(level);
        }
    }

    private void ClearGrid()
    {
        foreach (var c in level.cells) c.type = 0;
        EditorUtility.SetDirty(level); Repaint();
    }

    // ─── Random — Normal(1) theo maxNormalTypes, Boost(2) 2–4 cặp ───
    // Obstacle(3) giữ nguyên, không tính match
    private void RandomEvenTypes()
    {
        int total = level.gridWidth * level.gridHeight;

        // 1. Ghi nhớ vị trí obstacle, giữ nguyên
        var obstacleIdx = new HashSet<int>();
        for (int i = 0; i < level.cells.Count; i++)
            if (level.cells[i].type == 3)
                obstacleIdx.Add(i);

        int freeCells = total - obstacleIdx.Count;

        // 2. Boost: clamp cứng 2–4 cặp
        int boostPairs = Mathf.Clamp(level.maxBoostPairs, 2, 4);
        int boostCount = boostPairs * 2;

        // 3. Kiểm tra đủ ô
        int normalCells = freeCells - boostCount;
        if (normalCells <= 0 || normalCells % 2 != 0)
        {
            EditorUtility.DisplayDialog("Lỗi",
                $"Không đủ ô cho Normal!\n" +
                $"  Ô trống: {freeCells}\n" +
                $"  Boost cần: {boostCount} ô ({boostPairs} cặp)\n" +
                $"  Còn lại cho Normal: {normalCells} ô\n\n" +
                "Hãy tăng kích thước grid hoặc giảm số cặp Boost / Obstacle.", "OK");
            return;
        }

        // 4. Tạo pool Normal theo maxNormalTypes
        int numTypes = Mathf.Max(1, level.maxNormalTypes);
        int normalPairs = normalCells / 2;
        if (normalPairs < numTypes) numTypes = normalPairs;

        int basePairs = normalPairs / numTypes;
        int extraPairs = normalPairs % numTypes;

        var pool = new List<int>(freeCells);
        for (int t = 1; t <= numTypes; t++)
        {
            int pairs = basePairs + (t <= extraPairs ? 1 : 0);
            for (int i = 0; i < pairs * 2; i++) pool.Add(1); // Normal = type 1
        }

        // 5. Thêm Boost vào pool
        for (int i = 0; i < boostCount; i++) pool.Add(2);

        // 6. Shuffle Fisher-Yates
        for (int i = pool.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (pool[i], pool[j]) = (pool[j], pool[i]);
        }

        // 7. Ghi vào cells, bỏ qua obstacle
        int poolIdx = 0;
        for (int i = 0; i < level.cells.Count; i++)
        {
            if (obstacleIdx.Contains(i)) continue; // giữ obstacle nguyên
            level.cells[i].type = poolIdx < pool.Count ? pool[poolIdx++] : 0;
        }

        EditorUtility.SetDirty(level);
        Repaint();

        // 8. Log kết quả
        var map = new Dictionary<int, int>();
        foreach (var c in level.cells)
        {
            if (!map.ContainsKey(c.type)) map[c.type] = 0;
            map[c.type]++;
        }
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"✅ Random xong — {total} ô tổng:");
        sb.AppendLine($"  🔵 Normal   (1): {(map.ContainsKey(1) ? map[1] : 0)} ô  → {numTypes} loại match  ✔ chẵn");
        sb.AppendLine($"  🟡 Boost    (2): {(map.ContainsKey(2) ? map[2] : 0)} ô  → {boostPairs} cặp  ✔ chẵn");
        sb.AppendLine($"  🔴 Obstacle (3): {obstacleIdx.Count} ô  → không tính match, giữ nguyên");
        Debug.Log(sb.ToString());
    }
}