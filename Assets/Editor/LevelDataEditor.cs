using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelData))]
public class LevelDataEditor : Editor
{
    private static readonly Color[] movColors =
    {
        new Color(0.22f, 0.22f, 0.22f),
        new Color(0.25f, 0.55f, 1.00f),
        new Color(0.95f, 0.75f, 0.20f),
        new Color(0.30f, 0.85f, 0.50f),
    };
    private static readonly string[] movIcons = { "⬛", "🔵", "🟡", "🟢" };
    private static readonly string[] movNames =
    {
        "None\n(Không di chuyển)",
        "Circular / Spiral\n(Vòng tròn / Xoắn ốc)",
        "Row / Column Shift\n(Dịch hàng / cột)",
        "Fill to Center\n(Lấp đầy vào tâm)",
    };
    private static readonly string[] movDesc =
    {
        "Các ô đứng yên. Chế độ cơ bản không có hiệu ứng di chuyển.",
        "Các ô dịch chuyển theo vòng xoắn ốc từ ngoài vào hoặc từ trong ra.",
        "Cả hàng hoặc cả cột trượt sang trái/phải hoặc lên/xuống theo chu kỳ.",
        "Các ô trống bị lấp dần về phía tâm lưới, tạo áp lực cho người chơi.",
    };

    private const float PREVIEW_H = 110f;

    // ─────────────────────────────────────────────────────────────────
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        LevelData data = (LevelData)target;

        EditorGUILayout.LabelField("🐭  ONET LEVEL DATA", EditorStyles.boldLabel);
        EditorGUILayout.Space(4);

        SectionLabel("📋  THÔNG TIN CƠ BẢN");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("levelID"), new GUIContent("Level ID"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("levelName"), new GUIContent("Tên Level"));
        EditorGUILayout.Space(6);

        DrawBackgroundSection(data);
        EditorGUILayout.Space(6);

        DrawGridSizeSection(data);
        EditorGUILayout.Space(6);

        SectionLabel("⏱️  GIỚI HẠN CHƠI");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("useTimeLimit"), new GUIContent("Dùng Time Limit"));
        if (data.useTimeLimit)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("timeLimit"), new GUIContent("Thời gian (giây)"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("useMoveLimit"), new GUIContent("Dùng Move Limit"));
        if (data.useMoveLimit)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("maxMoves"), new GUIContent("Số nước đi tối đa"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("maxNormalTypes"), new GUIContent("Số loại icon match tối đa"));
        EditorGUILayout.Space(8);

        SectionLabel("🎯  ĐỘ KHÓ — KIỂU DI CHUYỂN");
        DrawMovementPicker(data);
        EditorGUILayout.Space(8);

        GUI.backgroundColor = new Color(0.4f, 0.8f, 1f);
        if (GUILayout.Button("🛠️  Mở Grid Editor", GUILayout.Height(40)))
            LevelGridEditorWindow.Open(data);
        GUI.backgroundColor = Color.white;

        serializedObject.ApplyModifiedProperties();
    }

    // ─── Grid Size Section ───────────────────────────────────────────
    private void DrawGridSizeSection(LevelData data)
    {
        SectionLabel("📐  KÍCH THƯỚC LƯỚI");
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        // ── Số ô ────────────────────────────────────────────────────
        EditorGUILayout.LabelField("Số ô", EditorStyles.miniBoldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("gridWidth"),
            new GUIContent("Width  (cột)", "Số ô theo chiều ngang"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("gridHeight"),
            new GUIContent("Height (hàng)", "Số ô theo chiều dọc"));
        EditorGUI.indentLevel--;

        EditorGUILayout.Space(4);

        // ── Cell Size & Spacing ──────────────────────────────────────
        EditorGUILayout.LabelField("Kích thước ô (World Units)", EditorStyles.miniBoldLabel);
        EditorGUI.indentLevel++;

        // cellSize — slider trực quan
        var csProp = serializedObject.FindProperty("cellSize");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(
            new GUIContent("Cell Size", "Kích thước 1 ô trong game (đơn vị Unity)"),
            GUILayout.Width(EditorGUIUtility.labelWidth));
        csProp.floatValue = EditorGUILayout.Slider(csProp.floatValue, 0.1f, 5f);
        EditorGUILayout.EndHorizontal();

        // spacing — slider trực quan
        var spProp = serializedObject.FindProperty("spacing");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(
            new GUIContent("Spacing", "Khoảng cách giữa các ô (đơn vị Unity). 0 = sát nhau."),
            GUILayout.Width(EditorGUIUtility.labelWidth));
        spProp.floatValue = EditorGUILayout.Slider(spProp.floatValue, 0f, 1f);
        EditorGUILayout.EndHorizontal();

        EditorGUI.indentLevel--;

        // Clamp giá trị không âm
        if (csProp.floatValue < 0.01f) csProp.floatValue = 0.01f;
        if (spProp.floatValue < 0f) spProp.floatValue = 0f;

        EditorGUILayout.Space(5);

        // ── Live info box ────────────────────────────────────────────
        float step = data.cellSize + data.spacing;
        float totalW = data.gridWidth * step - data.spacing;
        float totalH = data.gridHeight * step - data.spacing;
        int total = data.gridWidth * data.gridHeight;

        var infoStyle = new GUIStyle(EditorStyles.helpBox) { fontSize = 11, richText = true };
        EditorGUILayout.LabelField(
            $"<b>Tổng kích thước lưới:</b>  {totalW:F2} × {totalH:F2}  units\n" +
            $"<b>Tổng số ô:</b>  {total}  ({(total % 2 == 0 ? "✔ chẵn — OK" : "✘ lẻ — không random cặp được")})\n" +
            $"<b>Bước ô (cell + spacing):</b>  {step:F2}  units",
            infoStyle, GUILayout.MinHeight(52));

        EditorGUILayout.Space(4);

        // ── Mini grid preview ────────────────────────────────────────
        DrawMiniGridPreview(data);

        EditorGUILayout.Space(4);

        if (GUILayout.Button("🔄  Resize & Refresh Grid", GUILayout.Height(28)))
        {
            data.EnsureGridSize();
            EditorUtility.SetDirty(data);
        }

        EditorGUILayout.EndVertical();
    }

    // ── Mini visual preview của lưới (tỉ lệ thu nhỏ) ────────────────
    private void DrawMiniGridPreview(LevelData data)
    {
        int cols = data.gridWidth;
        int rows = data.gridHeight;
        float aspect = (float)cols / rows;

        float previewW = EditorGUIUtility.currentViewWidth - 36f;
        float previewH = Mathf.Clamp(previewW / aspect, 40f, 130f);

        Rect area = GUILayoutUtility.GetRect(
            GUIContent.none, GUIStyle.none,
            GUILayout.ExpandWidth(true),
            GUILayout.Height(previewH));

        // Nền
        EditorGUI.DrawRect(area, new Color(0.15f, 0.15f, 0.15f));

        float gapRatio = data.spacing / (data.cellSize + data.spacing);
        float cellPixelW = (area.width - (cols - 1) * gapRatio * (area.width / cols)) / cols;
        float cellPixelH = (area.height - (rows - 1) * gapRatio * (area.height / rows)) / rows;
        float gapPxW = cellPixelW * gapRatio;
        float gapPxH = cellPixelH * gapRatio;

        Color cellCol = new Color(0.45f, 0.72f, 1f, 0.85f);
        Color gapCol = new Color(0.08f, 0.08f, 0.08f);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                float px = area.x + col * (cellPixelW + gapPxW);
                float py = area.y + row * (cellPixelH + gapPxH);
                Rect cr = new Rect(px, py, cellPixelW, cellPixelH);
                EditorGUI.DrawRect(cr, cellCol);
            }
        }

        // Watermark kích thước
        var lbl = new GUIStyle(EditorStyles.miniLabel)
        {
            alignment = TextAnchor.LowerRight,
            normal = { textColor = new Color(1f, 1f, 1f, 0.55f) },
        };
        GUI.Label(new Rect(area.x, area.y, area.width - 3, previewH - 2),
                  $"{cols}×{rows}", lbl);

        // Viền
        DrawBorderRect(area, new Color(0.3f, 0.3f, 0.3f, 0.9f));
    }

    // ─── Background section ──────────────────────────────────────────
    private void DrawBackgroundSection(LevelData data)
    {
        SectionLabel("🖼️  BACKGROUND & GIAO DIỆN");
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("backgroundSprite"),
            new GUIContent("Background Sprite"));
        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("backgroundOverlay"),
            new GUIContent("Overlay Color"));

        EditorGUILayout.Space(4);

        Rect previewRect = GUILayoutUtility.GetRect(
            GUIContent.none, GUIStyle.none,
            GUILayout.ExpandWidth(true),
            GUILayout.Height(PREVIEW_H));

        DrawCheckerboard(previewRect);

        if (data.backgroundSprite != null)
        {
            Texture2D tex = AssetPreview.GetAssetPreview(data.backgroundSprite)
                         ?? data.backgroundSprite.texture;
            if (tex != null)
            {
                float texAR = (float)tex.width / tex.height;
                float boxAR = previewRect.width / PREVIEW_H;
                Rect drawR;
                if (texAR > boxAR)
                {
                    float h = previewRect.width / texAR;
                    drawR = new Rect(previewRect.x, previewRect.y + (PREVIEW_H - h) * .5f, previewRect.width, h);
                }
                else
                {
                    float w = PREVIEW_H * texAR;
                    drawR = new Rect(previewRect.x + (previewRect.width - w) * .5f, previewRect.y, w, PREVIEW_H);
                }
                GUI.DrawTexture(drawR, tex, ScaleMode.ScaleToFit);
            }
        }
        else
        {
            GUI.Label(previewRect, "[ Chưa chọn Background Sprite ]",
                new GUIStyle(EditorStyles.centeredGreyMiniLabel) { fontSize = 11 });
        }

        if (data.backgroundOverlay.a > 0.01f)
        {
            EditorGUI.DrawRect(previewRect, data.backgroundOverlay);
            GUI.Label(new Rect(previewRect.x, previewRect.y, previewRect.width - 4, PREVIEW_H - 4),
                $"Overlay α={data.backgroundOverlay.a:F2}",
                new GUIStyle(EditorStyles.miniLabel)
                { alignment = TextAnchor.LowerRight, normal = { textColor = Color.white } });
        }

        DrawBorderRect(previewRect, new Color(0.3f, 0.3f, 0.3f, 0.8f));

        EditorGUILayout.Space(2);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("✖ Xóa Sprite", GUILayout.Height(22))) { Undo.RecordObject(data, "Clear Sprite"); data.backgroundSprite = null; EditorUtility.SetDirty(data); }
        if (GUILayout.Button("🎨 Reset Overlay", GUILayout.Height(22))) { Undo.RecordObject(data, "Reset Overlay"); data.backgroundOverlay = new Color(0, 0, 0, 0); EditorUtility.SetDirty(data); }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    // ─── Movement picker ─────────────────────────────────────────────
    private void DrawMovementPicker(LevelData data)
    {
        int current = (int)data.movementType;
        EditorGUILayout.BeginHorizontal();
        for (int i = 0; i < 4; i++)
        {
            if (i == 2) { EditorGUILayout.EndHorizontal(); EditorGUILayout.BeginHorizontal(); }
            bool sel = (current == i);
            GUI.backgroundColor = sel ? movColors[i]
                : new Color(movColors[i].r * .55f, movColors[i].g * .55f, movColors[i].b * .55f);
            var s = new GUIStyle(GUI.skin.button)
            { fontStyle = sel ? FontStyle.Bold : FontStyle.Normal, fontSize = 11, fixedHeight = 58, wordWrap = true, alignment = TextAnchor.MiddleCenter };
            if (sel) s.normal.textColor = Color.white;
            if (GUILayout.Button(movIcons[i] + "  " + movNames[i], s))
            { Undo.RecordObject(data, "Change MovementType"); data.movementType = (MovementType)i; EditorUtility.SetDirty(data); }
        }
        EditorGUILayout.EndHorizontal();
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space(3);
        GUI.backgroundColor = Color.Lerp(movColors[current], Color.white, .65f);
        EditorGUILayout.LabelField($"{movIcons[current]}  {movDesc[current]}", EditorStyles.helpBox, GUILayout.MinHeight(30));
        GUI.backgroundColor = Color.white;

        if (data.movementType != MovementType.None)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("⚙️  Tham số di chuyển", EditorStyles.miniBoldLabel);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("movementSpeed"), new GUIContent("Tốc độ (ô/giây)"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("movementDirection"), new GUIContent("Hướng (+1 / -1)"));
            float sp = data.movementSpeed;
            string dif = sp <= .5f ? "🟢 Dễ" : sp <= 1.5f ? "🟡 Trung bình" : sp <= 3f ? "🟠 Khó" : "🔴 Rất khó";
            EditorGUILayout.LabelField($"Độ khó ước tính: {dif}", EditorStyles.miniLabel);
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
    }

    // ─── Draw helpers ─────────────────────────────────────────────────
    private void DrawCheckerboard(Rect r)
    {
        int size = 10;
        Color dark = new Color(.35f, .35f, .35f), light = new Color(.50f, .50f, .50f);
        int cols = Mathf.CeilToInt(r.width / size), rows = Mathf.CeilToInt(r.height / size);
        for (int row = 0; row < rows; row++)
            for (int col = 0; col < cols; col++)
            {
                Color c = ((row + col) % 2 == 0) ? dark : light;
                EditorGUI.DrawRect(new Rect(
                    r.x + col * size, r.y + row * size,
                    Mathf.Min(size, r.xMax - (r.x + col * size)),
                    Mathf.Min(size, r.yMax - (r.y + row * size))), c);
            }
    }

    private void DrawBorderRect(Rect r, Color c)
    {
        float w = 1;
        EditorGUI.DrawRect(new Rect(r.x, r.y, r.width, w), c);
        EditorGUI.DrawRect(new Rect(r.x, r.yMax - w, r.width, w), c);
        EditorGUI.DrawRect(new Rect(r.x, r.y, w, r.height), c);
        EditorGUI.DrawRect(new Rect(r.xMax - w, r.y, w, r.height), c);
    }

    private void SectionLabel(string text) =>
        EditorGUILayout.LabelField(text, new GUIStyle(EditorStyles.boldLabel) { fontSize = 11 });
}