using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class GameMechanics
{
    private static bool timeUp = false;
    private static int currentScore = 0;
    private static float currentTime=0;
    private static float maxTime=0;
    public static void Init(int time)
    {
        currentScore = 0;
        timeUp = false;
        currentTime=time;
        maxTime = time;

       // Debug.Log("hien thi"+currentTime);
    }

    public static void AddScore(int amout)
    {
        currentScore += amout;
        GameManager.Instance.Score= currentScore;
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
        for (int x = 0; x < GameManager.Instance.gridManager.Width; x++)
        {
            if (x == a.x) continue;
            if (!CheckLineX(a.y, a.x, x, board)) continue; // Đường từ A tới điểm quét phải trống

            Vector2Int p = new Vector2Int(x, a.y);
            if (board.GetCell(p.x, p.y).type != 0) continue;

            List<Vector2Int> rectPath = CheckRect(p, b, board);
            if (rectPath != null)
            {
                rectPath.Insert(0, a);
                return rectPath;
            }
        }

        // Dọc
        for (int y = 0; y < GameManager.Instance.gridManager.Height; y++)
        {
            if (y == a.y) continue;
            if (!CheckLineY(a.x, a.y, y, board)) continue;

            Vector2Int p = new Vector2Int(a.x, y);
            if (board.GetCell(p.x, p.y).type != 0) continue;

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
}