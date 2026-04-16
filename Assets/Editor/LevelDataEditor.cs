using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelData))]
public class LevelDataEditor : Editor
{
    // ── Gravity picker data ───────────────────────────────────────────
    private static readonly (BoardGravityType type, string icon, string label, Color color, string desc)[] _gravities =
    {
        (BoardGravityType.None,          "■",  "Không",      new Color(0.22f, 0.22f, 0.22f),
            "Tile đứng yên. Chế độ cơ bản, không có hiệu ứng di chuyển."),

        (BoardGravityType.GravityDown,   "▼",  "Rơi xuống",  new Color(0.25f, 0.55f, 1.00f),
            "Tile phía trên rơi xuống lấp chỗ trống sau mỗi match. Phổ biến nhất."),

        (BoardGravityType.GravityUp,     "▲",  "Bay lên",    new Color(0.30f, 0.75f, 0.60f),
            "Tile phía dưới bay lên sau match. Ngược chiều với Rơi xuống."),

        (BoardGravityType.ShiftLeft,     "◀",  "Dồn trái",   new Color(0.65f, 0.40f, 1.00f),
            "Toàn bộ hàng dồn sang trái lấp chỗ trống. Tile bên phải di chuyển lại."),

        (BoardGravityType.ShiftRight,    "▶",  "Dồn phải",   new Color(0.65f, 0.40f, 1.00f),
            "Toàn bộ hàng dồn sang phải. Ngược chiều với Dồn trái."),

        (BoardGravityType.CenterCollapse,"◀▶", "Vào giữa",   new Color(0.95f, 0.70f, 0.15f),
            "Tile từ 2 bên dồn vào tâm lưới. Khó vì board bị nén lại."),

        (BoardGravityType.SplitOutward,  "▶◀", "Ra ngoài",   new Color(0.95f, 0.45f, 0.20f),
            "Tile tách ra 2 rìa sau match. Board bị 'xé' — mất liên kết nhanh."),

        (BoardGravityType.Mixed,         "↕",  "Xen kẽ",     new Color(0.30f, 0.85f, 0.50f),
            "Cột chẵn rơi xuống ▼, cột lẻ bay lên ▲. Khó nhất vì khó đoán."),
    };

    private const float PREVIEW_H = 110f;

    // ── Foldout states ────────────────────────────────────────────────
    private bool _foldScore = true;
    private bool _foldCombo = true;
    private bool _foldStar = true;

    // ══════════════════════════════════════════════════════════════════
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        LevelData data = (LevelData)target;

        HeaderLabel("🐭  ONET LEVEL DATA");
        EditorGUILayout.Space(4);

        // ── Thông tin cơ bản ─────────────────────────────────────────
        SectionLabel("📋  THÔNG TIN CƠ BẢN");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("levelID"), new GUIContent("Level ID"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("levelName"), new GUIContent("Tên Level"));
        EditorGUILayout.Space(6);

        DrawBackgroundSection(data);
        EditorGUILayout.Space(6);

        DrawGridSizeSection(data);
        EditorGUILayout.Space(6);

        DrawLimitSection(data);
        EditorGUILayout.Space(6);

        DrawScoreSection(data);
        EditorGUILayout.Space(8);

        SectionLabel("🎯  ĐỘ KHÓ — KIỂU DI CHUYỂN TILE");
        DrawGravityPicker(data);
        EditorGUILayout.Space(10);

        // ── Nút mở Grid Editor ───────────────────────────────────────
        GUI.backgroundColor = new Color(0.4f, 0.8f, 1f);
        if (GUILayout.Button("🛠️  Mở Grid Editor", GUILayout.Height(42)))
            LevelGridEditorWindow.Open(data);
        GUI.backgroundColor = Color.white;

        serializedObject.ApplyModifiedProperties();
    }

    // ══════════════════════════════════════════════════════════════════
    //  SECTION: Background
    // ══════════════════════════════════════════════════════════════════
    private void DrawBackgroundSection(LevelData data)
    {
        SectionLabel("🖼️  BACKGROUND & GIAO DIỆN");
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("backgroundSprite"), new GUIContent("Background Sprite"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("backgroundOverlay"), new GUIContent("Overlay Color"));

        EditorGUILayout.Space(4);

        Rect previewRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none,
            GUILayout.ExpandWidth(true), GUILayout.Height(PREVIEW_H));

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
            GUI.Label(
                new Rect(previewRect.x, previewRect.y, previewRect.width - 4, PREVIEW_H - 4),
                $"Overlay α={data.backgroundOverlay.a:F2}",
                new GUIStyle(EditorStyles.miniLabel)
                { alignment = TextAnchor.LowerRight, normal = { textColor = Color.white } });
        }

        DrawBorderRect(previewRect, new Color(0.3f, 0.3f, 0.3f, 0.8f));

        EditorGUILayout.Space(2);
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("✖ Xóa Sprite", GUILayout.Height(22)))
        {
            Undo.RecordObject(data, "Clear Sprite");
            data.backgroundSprite = null;
            EditorUtility.SetDirty(data);
        }
        if (GUILayout.Button("🎨 Reset Overlay", GUILayout.Height(22)))
        {
            Undo.RecordObject(data, "Reset Overlay");
            data.backgroundOverlay = Color.clear;
            EditorUtility.SetDirty(data);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
    }

    // ══════════════════════════════════════════════════════════════════
    //  SECTION: Grid Size
    // ══════════════════════════════════════════════════════════════════
    private void DrawGridSizeSection(LevelData data)
    {
        SectionLabel("📐  KÍCH THƯỚC LƯỚI");
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.LabelField("Số ô", EditorStyles.miniBoldLabel);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("gridWidth"),
            new GUIContent("Width  (cột)", "Số ô theo chiều ngang"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("gridHeight"),
            new GUIContent("Height (hàng)", "Số ô theo chiều dọc"));
        EditorGUI.indentLevel--;

        EditorGUILayout.Space(4);
        EditorGUILayout.LabelField("Kích thước ô (World Units)", EditorStyles.miniBoldLabel);
        EditorGUI.indentLevel++;

        var csProp = serializedObject.FindProperty("cellSize");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Cell Size", "Kích thước 1 ô trong game (đơn vị Unity)"),
            GUILayout.Width(EditorGUIUtility.labelWidth));
        csProp.floatValue = EditorGUILayout.Slider(csProp.floatValue, 0.1f, 5f);
        EditorGUILayout.EndHorizontal();

        var spProp = serializedObject.FindProperty("spacing");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Spacing", "Khoảng cách giữa các ô. 0 = sát nhau."),
            GUILayout.Width(EditorGUIUtility.labelWidth));
        spProp.floatValue = EditorGUILayout.Slider(spProp.floatValue, 0f, 1f);
        EditorGUILayout.EndHorizontal();

        EditorGUI.indentLevel--;

        EditorGUILayout.Space(5);

        bool isEven = data.TotalIsEven;
        var infoStyle = new GUIStyle(EditorStyles.helpBox) { fontSize = 11, richText = true };
        EditorGUILayout.LabelField(
            $"<b>Tổng kích thước:</b>  {data.GridWorldWidth:F2} × {data.GridWorldHeight:F2}  units\n" +
            $"<b>Tổng số ô:</b>  {data.TotalCells}  ({(isEven ? "✔ chẵn — OK" : "✘ lẻ — không random cặp được")})\n" +
            $"<b>Bước ô (cell + spacing):</b>  {data.StepSize:F2}  units",
            infoStyle, GUILayout.MinHeight(52));

        EditorGUILayout.Space(4);
        DrawMiniGridPreview(data);
        EditorGUILayout.Space(4);

        if (GUILayout.Button("🔄  Resize & Refresh Grid", GUILayout.Height(28)))
        {
            data.EnsureGridSize();
            EditorUtility.SetDirty(data);
        }

        EditorGUILayout.EndVertical();
    }

    // ── Mini visual preview lưới ─────────────────────────────────────
    private void DrawMiniGridPreview(LevelData data)
    {
        int cols = data.gridWidth;
        int rows = data.gridHeight;
        float aspect = (float)cols / rows;
        float previewW = EditorGUIUtility.currentViewWidth - 36f;
        float previewH = Mathf.Clamp(previewW / aspect, 40f, 130f);

        Rect area = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none,
            GUILayout.ExpandWidth(true), GUILayout.Height(previewH));

        EditorGUI.DrawRect(area, new Color(0.15f, 0.15f, 0.15f));

        float gapRatio = data.spacing / (data.cellSize + data.spacing);
        float cellPixW = (area.width - (cols - 1) * gapRatio * (area.width / cols)) / cols;
        float cellPixH = (area.height - (rows - 1) * gapRatio * (area.height / rows)) / rows;
        float gapPxW = cellPixW * gapRatio;
        float gapPxH = cellPixH * gapRatio;

        for (int row = 0; row < rows; row++)
            for (int col = 0; col < cols; col++)
            {
                int y = rows - 1 - row;
                var cell = data.GetCell(col, y);
                Color cellCol = cell != null && cell.type > 0
                    ? TileTypeColor(cell.type)
                    : new Color(0.45f, 0.72f, 1f, 0.55f);

                float px = area.x + col * (cellPixW + gapPxW);
                float py = area.y + row * (cellPixH + gapPxH);
                EditorGUI.DrawRect(new Rect(px, py, cellPixW, cellPixH), cellCol);
            }

        var lbl = new GUIStyle(EditorStyles.miniLabel)
        {
            alignment = TextAnchor.LowerRight,
            normal = { textColor = new Color(1f, 1f, 1f, 0.55f) },
        };
        GUI.Label(new Rect(area.x, area.y, area.width - 3, previewH - 2), $"{cols}×{rows}", lbl);
        DrawBorderRect(area, new Color(0.3f, 0.3f, 0.3f, 0.9f));
    }

    private Color TileTypeColor(int type) => type switch
    {
        1 => new Color(0.45f, 0.72f, 1.00f, 0.85f),  // Normal — xanh
        2 => new Color(1.00f, 0.82f, 0.25f, 0.85f),  // Boost  — vàng
        3 => new Color(0.90f, 0.28f, 0.28f, 0.85f),  // Obstacle — đỏ
        _ => new Color(0.20f, 0.20f, 0.20f, 0.85f),  // Empty
    };

    // ══════════════════════════════════════════════════════════════════
    //  SECTION: Giới hạn chơi
    // ══════════════════════════════════════════════════════════════════
    private void DrawLimitSection(LevelData data)
    {
        SectionLabel("⏱️  GIỚI HẠN CHƠI");
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("useTimeLimit"),
            new GUIContent("Dùng Time Limit"));
        if (data.useTimeLimit)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("timeLimit"),
                new GUIContent("Thời gian (giây)"));

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("🎴  Thông tin type", EditorStyles.miniBoldLabel);
        EditorGUI.indentLevel++;

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("maxNormalTypes"),
            new GUIContent("Số loại icon match tối đa",
                "Chỉ áp dụng cho Normal (type 1). Obstacle không tính."));

        var boostProp = serializedObject.FindProperty("maxBoostPairs");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(
            new GUIContent("Số cặp Boost (0–4)",
                "Boost (type 2) xuất hiện tối đa 0–4 cặp (0–8 ô).\nKhông tính vào maxNormalTypes."),
            GUILayout.Width(EditorGUIUtility.labelWidth));
        boostProp.intValue = EditorGUILayout.IntSlider(boostProp.intValue, 0, 4);
        EditorGUILayout.EndHorizontal();

        int boostCount = boostProp.intValue * 2;
        var infoStyle = new GUIStyle(EditorStyles.helpBox) { fontSize = 11, richText = true };
        EditorGUILayout.LabelField(
            $"<b>Normal  (1):</b> match theo {data.maxNormalTypes} loại icon  ✔\n" +
            $"<b>Boost   (2):</b> {boostProp.intValue} cặp = {boostCount} ô  ✔\n" +
            $"<b>Obstacle(3):</b> không tính match, giữ nguyên khi Random  ✔",
            infoStyle, GUILayout.MinHeight(52));

        EditorGUI.indentLevel--;
        EditorGUILayout.EndVertical();
    }

    // ══════════════════════════════════════════════════════════════════
    //  SECTION: Điểm & Sao
    // ══════════════════════════════════════════════════════════════════
    private void DrawScoreSection(LevelData data)
    {
        SectionLabel("🏆  ĐIỂM THƯỞNG & XẾP HẠNG SAO");
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("requireStarToWin"),
            new GUIContent("Cần đủ điểm để thắng level",
                "Nếu bật: phải đạt ít nhất mốc 1 sao mới qua level.\nNếu tắt: xóa hết ô là thắng."));

        EditorGUILayout.Space(5);

        // ── Điểm base ─────────────────────────────────────────────────
        _foldScore = EditorGUILayout.Foldout(_foldScore, "💰  Điểm base mỗi match", true, FoldoutBoldStyle());
        if (_foldScore)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("scorePerNormalMatch"),
                new GUIContent("Normal tile (+điểm)", "Type 1 — tile thường"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("scorePerBoostMatch"),
                new GUIContent("Boost tile  (+điểm)", "Type 2 — tile đặc biệt"));

            var infoStyle = new GUIStyle(EditorStyles.helpBox) { fontSize = 11, richText = true };
            EditorGUILayout.LabelField(
                $"<b>Normal:</b> +{data.scorePerNormalMatch}đ / match  ·  <b>Boost:</b> +{data.scorePerBoostMatch}đ / match",
                infoStyle);
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space(3);

        // ── Combo ──────────────────────────────────────────────────────
        _foldCombo = EditorGUILayout.Foldout(_foldCombo, "🔥  Hệ thống Combo", true, FoldoutBoldStyle());
        if (_foldCombo)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(serializedObject.FindProperty("useCombo"),
                new GUIContent("Bật Combo nhân điểm"));

            if (data.useCombo)
            {
                var stepProp = serializedObject.FindProperty("comboMultiplierStep");
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(
                    new GUIContent("Hệ số tăng mỗi combo",
                        "Mỗi combo liên tiếp tăng thêm X lần điểm base.\n0.5 → x2=+50%,  x3=+100%..."),
                    GUILayout.Width(EditorGUIUtility.labelWidth));
                stepProp.floatValue = EditorGUILayout.Slider(stepProp.floatValue, 0f, 2f);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.PropertyField(serializedObject.FindProperty("comboResetDelay"),
                    new GUIContent("Reset sau (giây)", "Combo reset nếu không match trong N giây. 0 = không reset."));

                var maxComboProp = serializedObject.FindProperty("maxComboCount");
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(new GUIContent("Combo tối đa (cap)", "0 = không giới hạn"),
                    GUILayout.Width(EditorGUIUtility.labelWidth));
                maxComboProp.intValue = EditorGUILayout.IntSlider(maxComboProp.intValue, 0, 20);
                EditorGUILayout.EndHorizontal();

                DrawComboPreviewTable(data);
            }
            else
            {
                EditorGUILayout.LabelField("Combo đang tắt — điểm luôn cố định.", EditorStyles.miniLabel);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.Space(3);

        // ── Mốc sao ───────────────────────────────────────────────────
        _foldStar = EditorGUILayout.Foldout(_foldStar, "⭐  Mốc điểm xếp hạng sao", true, FoldoutBoldStyle());
        if (_foldStar)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel++;

            var s1 = serializedObject.FindProperty("scoreStar1");
            var s2 = serializedObject.FindProperty("scoreStar2");
            var s3 = serializedObject.FindProperty("scoreStar3");

            EditorGUILayout.PropertyField(s1,
                new GUIContent("⭐  1 Sao" + (data.requireStarToWin ? "  (= điểm thắng tối thiểu)" : "")));
            EditorGUILayout.PropertyField(s2, new GUIContent("⭐⭐  2 Sao"));
            EditorGUILayout.PropertyField(s3, new GUIContent("⭐⭐⭐  3 Sao"));

            bool orderOk = s1.intValue < s2.intValue && s2.intValue < s3.intValue;
            if (!orderOk)
            {
                EditorGUILayout.HelpBox("⚠️ Mốc sao phải tăng dần: 1 Sao < 2 Sao < 3 Sao!", MessageType.Warning);
            }
            else
            {
                DrawStarPreviewBar(data);
                DrawStarEstimate(data);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndVertical();
    }

    // ── Bảng xem trước điểm combo ────────────────────────────────────
    private void DrawComboPreviewTable(LevelData data)
    {
        int cap = data.maxComboCount > 0 ? data.maxComboCount : 6;
        int count = Mathf.Min(cap, 6);

        var sb = new System.Text.StringBuilder();
        sb.Append("<b>Xem trước điểm Normal / Boost theo combo:</b>\n");

        for (int i = 1; i <= count; i++)
        {
            int sn = data.CalculateMatchScore(1, i);
            int sb2 = data.CalculateMatchScore(2, i);
            float mult = 1f + data.comboMultiplierStep * (i - 1);
            string cap2 = (data.maxComboCount > 0 && i == data.maxComboCount) ? "  ← MAX" : "";
            sb.Append($"  Combo x{i}  (×{mult:F1})  →  Normal: <b>{sn}đ</b>  |  Boost: <b>{sb2}đ</b>{cap2}\n");
        }
        if (data.maxComboCount == 0 || data.maxComboCount > 6) sb.Append("  ...");

        var tableStyle = new GUIStyle(EditorStyles.helpBox) { fontSize = 11, richText = true };
        EditorGUILayout.LabelField(sb.ToString(), tableStyle, GUILayout.MinHeight(20 + count * 16));
    }

    // ── Thanh visual 3 mốc sao ───────────────────────────────────────
    private void DrawStarPreviewBar(LevelData data)
    {
        EditorGUILayout.Space(3);
        float barH = 24f;
        Rect area = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none,
            GUILayout.ExpandWidth(true), GUILayout.Height(barH));

        int maxScore = Mathf.RoundToInt(data.scoreStar3 * 1.25f);
        EditorGUI.DrawRect(area, new Color(0.18f, 0.18f, 0.18f));

        DrawBarFill(area, barH, (float)data.scoreStar3 / maxScore, new Color(0.28f, 0.78f, 0.40f, 0.75f));
        DrawBarFill(area, barH, (float)data.scoreStar2 / maxScore, new Color(0.95f, 0.80f, 0.15f, 0.85f));
        DrawBarFill(area, barH, (float)data.scoreStar1 / maxScore, new Color(0.95f, 0.40f, 0.20f, 0.95f));

        var lbl = new GUIStyle(EditorStyles.miniLabel)
        {
            alignment = TextAnchor.MiddleLeft,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white },
            fontSize = 10,
        };
        float w = area.width;
        PlaceStarLabel(area, barH, (float)data.scoreStar1 / maxScore, w, "★", data.scoreStar1, lbl);
        PlaceStarLabel(area, barH, (float)data.scoreStar2 / maxScore, w, "★★", data.scoreStar2, lbl);
        PlaceStarLabel(area, barH, (float)data.scoreStar3 / maxScore, w, "★★★", data.scoreStar3, lbl);

        DrawBorderRect(area, new Color(0.3f, 0.3f, 0.3f, 0.9f));
    }

    private void DrawBarFill(Rect area, float barH, float ratio, Color color)
    {
        ratio = Mathf.Clamp01(ratio);
        EditorGUI.DrawRect(new Rect(area.x, area.y, area.width * ratio, barH), color);
    }

    private void PlaceStarLabel(Rect area, float barH, float ratio,
                                float w, string stars, int score, GUIStyle lbl)
    {
        float x = area.x + w * Mathf.Clamp01(ratio) + 2f;
        GUI.Label(new Rect(x, area.y, 72, barH), $"{stars}\n{score}đ", lbl);
    }

    // ── Ước tính match cần đạt sao ──────────────────────────────────
    private void DrawStarEstimate(LevelData data)
    {
        int baseN = Mathf.Max(1, data.scorePerNormalMatch);
        int m1 = Mathf.CeilToInt((float)data.scoreStar1 / baseN);
        int m2 = Mathf.CeilToInt((float)data.scoreStar2 / baseN);
        int m3 = Mathf.CeilToInt((float)data.scoreStar3 / baseN);

        var style = new GUIStyle(EditorStyles.helpBox) { fontSize = 11, richText = true };
        EditorGUILayout.LabelField(
            $"<b>Ước tính match Normal cần (không combo):</b>\n" +
            $"  ★ 1 Sao: ~{m1} match   |   ★★ 2 Sao: ~{m2} match   |   ★★★ 3 Sao: ~{m3} match",
            style, GUILayout.MinHeight(40));
    }

    // ══════════════════════════════════════════════════════════════════
    //  SECTION: Gravity Picker (thay DrawMovementPicker cũ)
    //  Hiện 8 nút theo 2 hàng x 4 cột
    // ══════════════════════════════════════════════════════════════════
    private void DrawGravityPicker(LevelData data)
    {
        int current = (int)data.gravityType;

        // 2 hàng, mỗi hàng 4 nút
        for (int row = 0; row < 2; row++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int col = 0; col < 4; col++)
            {
                int i = row * 4 + col;
                if (i >= _gravities.Length) break;

                var (gtype, icon, label, color, _) = _gravities[i];
                bool sel = current == i;

                GUI.backgroundColor = sel
                    ? color
                    : new Color(color.r * 0.5f, color.g * 0.5f, color.b * 0.5f);

                var s = new GUIStyle(GUI.skin.button)
                {
                    fontStyle = sel ? FontStyle.Bold : FontStyle.Normal,
                    fontSize = 11,
                    fixedHeight = 54,
                    wordWrap = true,
                    alignment = TextAnchor.MiddleCenter,
                };
                if (sel) s.normal.textColor = Color.white;

                if (GUILayout.Button($"{icon}\n{label}", s))
                {
                    Undo.RecordObject(data, "Change GravityType");
                    data.gravityType = gtype;
                    EditorUtility.SetDirty(data);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space(3);

        // Mô tả kiểu hiện tại
        var (_, curIcon, curLabel, curColor, curDesc) = _gravities[current];
        GUI.backgroundColor = Color.Lerp(curColor, Color.white, 0.65f);
        EditorGUILayout.LabelField($"{curIcon}  {curLabel}: {curDesc}",
            EditorStyles.helpBox, GUILayout.MinHeight(30));
        GUI.backgroundColor = Color.white;

        // Tham số chỉ hiện khi có gravity
        if (data.gravityType != BoardGravityType.None)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("⚙️  Tham số di chuyển", EditorStyles.miniBoldLabel);
            EditorGUI.indentLevel++;

            var speedProp = serializedObject.FindProperty("gravitySpeed");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(
                new GUIContent("Tốc độ trượt (units/s)", "Tile trượt về vị trí mới nhanh hay chậm"),
                GUILayout.Width(EditorGUIUtility.labelWidth));
            speedProp.floatValue = EditorGUILayout.Slider(speedProp.floatValue, 0.5f, 20f);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("gravityLoop"),
                new GUIContent("Loop (tràn ra đầu kia)",
                    "Nếu bật: tile tràn sang đầu kia thay vì dừng tại rìa.\nVD: ShiftLeft → tile bên trái xuất hiện bên phải."));

            // Nhãn độ khó theo tốc độ
            float sp = data.gravitySpeed;
            string dif = sp <= 2f ? "🟢 Dễ"
                       : sp <= 6f ? "🟡 Trung bình"
                       : sp <= 12f ? "🟠 Khó"
                       : "🔴 Rất khó";
            EditorGUILayout.LabelField($"Độ khó ước tính: {dif}", EditorStyles.miniLabel);

            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
        }
    }

    // ══════════════════════════════════════════════════════════════════
    //  DRAW HELPERS
    // ══════════════════════════════════════════════════════════════════
    private void DrawCheckerboard(Rect r)
    {
        int size = 10;
        Color dark = new Color(.35f, .35f, .35f);
        Color light = new Color(.50f, .50f, .50f);
        int cols = Mathf.CeilToInt(r.width / size);
        int rows = Mathf.CeilToInt(r.height / size);
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
        EditorGUI.DrawRect(new Rect(r.x, r.y, r.width, 1), c);
        EditorGUI.DrawRect(new Rect(r.x, r.yMax - 1, r.width, 1), c);
        EditorGUI.DrawRect(new Rect(r.x, r.y, 1, r.height), c);
        EditorGUI.DrawRect(new Rect(r.xMax - 1, r.y, 1, r.height), c);
    }

    private void HeaderLabel(string text) =>
        EditorGUILayout.LabelField(text, new GUIStyle(EditorStyles.boldLabel) { fontSize = 13 });

    private void SectionLabel(string text) =>
        EditorGUILayout.LabelField(text, new GUIStyle(EditorStyles.boldLabel) { fontSize = 11 });

    private GUIStyle FoldoutBoldStyle() =>
        new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold, fontSize = 11 };
}