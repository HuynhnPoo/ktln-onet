
using System;
using Unity.Burst.Intrinsics;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;

public class Board
{
    private int width;
    private int height;
    public int Height=> height;

    public int Width => width;
    private float cellSize;
    private float spacing;

    private LevelData levelData;
    public GridCell[,] gridCell;
  
    public Action<GridCell, Vector3> OnCellMoved;

    public Board(int wight, int height, float cellSize, float spacing, LevelData levelData)
    {
        this.width = wight;
        this.height = height;
        this.cellSize = cellSize;
        this.spacing = spacing;
        this.levelData = levelData;

        gridCell = new GridCell[this.width, this.height];
        for (int x = 0; x < this.width; x++)
        {
            for (int y = 0; y < this.height; y++)
            {
               GridCell cell =levelData.GetCell(x, y);

                if(cell !=null)
                {
                    gridCell[x, y] = new GridCell
                    {
                        x = cell.x,
                        y = cell.y,
                        iconID = (cell != null) ? cell.iconID : -1,
                        type = (cell != null) ? cell.type : 0
                    };
                }
            }
        }
    }

    // Vector3 chứa độ lệch âm để dịch chuyển tâm bàn về(0,0,0)
    public Vector3 GetCenterOffset()
    {
        float step = cellSize + spacing;
        float totalWidth = (width - 1) * step;
        float totalHeight = (height - 1) * step;
        return new Vector3(-totalWidth / 2f, -totalHeight / 2f, 0f);
    }

    //Lấy thông tin ô tại tọa độ (x, y). Có kiểm tra tràn mảng.
    public GridCell GetCell(int x, int y)
    {
        // Nếu tọa độ nằm ngoài mảng (ngoài biên)
        if (x < 0 || x >= this.width || y < 0 || y >= this.height)
        {
            // Trả về một GridCell "ảo" có type = 0 (ô trống) để đường nối đi qua được
            return new GridCell { x = x, y = y, type = 0 };
        }
        return gridCell[x, y];
    }


    // Đặt trạng thái ô tại (x, y) thành trống (type = 0).
    public void SetCellEmpty(int x, int y)
    {
        var cell = GetCell(x, y);
        if (cell != null)
        {
            cell.type = 0; // Chỉ thay đổi trên runtimeGrid
        }
    }

    // Kiểm tra xem ô tại vị trí (x, y) có trống hay không.
    public bool IsEmpty(int x, int y)
    {
        var cell = GetCell(x, y);
        return cell == null || cell.type == 0;
    }

    // Chuyển đổi tọa độ chỉ số (x, y) sang tọa độ thực tế trong Unity.
    public Vector3 GetPostionWorld(int x, int y)
    {
        float step = cellSize + spacing;
        float totalWidth = (width - 1) * step;
        float totalHeight = (height - 1) * step;
        float offsetX = -totalWidth / 2f;
        float offsetY = -totalHeight / 2f;
        return new Vector3(x * step + offsetX, y * step + offsetY, 0f);
    }

    public void CheckLevelProgress(int width, int height)
    {
        int remaining = 0;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var cell = this.GetCell(i, j);
                // Đếm các ô không phải là trống (0) và không phải vật cản cố định (giả định type 3 là vật cản)
                if (cell != null && cell.type != 3 && cell.type != 0)
                    remaining++;
            }
        }

        if (remaining == 0)
        {
            GameManager.Instance.GameWon();
        }

    }

    public void OnMatchSuccess(int x1, int y1, int x2, int y2)
    {
        // 1. Xóa dữ liệu logic (Set type = 0)
        SetCellEmpty(x1, y1);
        SetCellEmpty(x2, y2);

        // 2. Kiểm tra nếu level có yêu cầu di chuyển tile
        if (levelData.gravityType != BoardGravityType.None)
        {
         //  GameMechanics.ApplyGravity(levelData.gravityType);
        }

        // 3. Kiểm tra xem đã thắng chưa
        CheckLevelProgress(width, height);
    }
}


[System.Serializable]
public class GridCell
{
    public int x, y; // vị trị
    public int iconID; // id xác định
    public int type; //  kiểm nó normal,hay boost hay objsatcles

    public bool IsEmpty => type == 0;
    public bool IsNormal => type == 1;
    public bool IsBoost => type == 2;
    public bool IsObstacle => type == 3;
    public bool IsMatchable => type == 1 || type == 2;

    public ControlTile linkedTile;
}
