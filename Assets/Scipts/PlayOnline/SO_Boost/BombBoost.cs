using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "BombBoost", menuName = "Boosts/Bomb")]
public class BombBoost : BoostBase
{
    protected override bool CanExecute(GridManager grid)
    {
        // Điều kiện riêng của Bomb: Bàn chơi phải còn vật phẩm để nổ
        return grid != null; // Bạn có thể thêm logic grid.GetTotalItems() > 0 ở đây
    }
    protected override void Execute(GridManager gridManager)
    {
        List<Vector2Int> validTiles =
            new List<Vector2Int>();

        // =========================
        // LẤY TILE HỢP LỆ
        // =========================
        for (int x = 0; x < gridManager.Width; x++)
        {
            for (int y = 0; y < gridManager.Height; y++)
            {
                GridCell cell =
                    gridManager.Board.GetCell(x, y);

                if (cell == null || cell.IsEmpty)
                    continue;

                validTiles.Add(new Vector2Int(x, y));
            }
        }

        // =========================
        // RANDOM TILE
        // =========================
        Shuffle(validTiles);

        // =========================
        // TÌM 2 TILE CÙNG ICON
        // =========================
        for (int i = 0; i < validTiles.Count; i++)
        {
            for (int j = i + 1; j < validTiles.Count; j++)
            {
                Vector2Int a = validTiles[i];
                Vector2Int b = validTiles[j];

                GridCell cellA =
                    gridManager.Board.GetCell(a.x, a.y);

                GridCell cellB =
                    gridManager.Board.GetCell(b.x, b.y);

                // khác icon -> bỏ
                if (cellA.iconID != cellB.iconID)
                    continue;

                Debug.Log("REMOVE RANDOM PAIR");

                RemoveTile(gridManager, a);

                RemoveTile(gridManager, b);

                return;
            }
        }

        Debug.Log("Không tìm thấy cặp");
    }

    // =========================
    // REMOVE TILE
    // =========================
    void RemoveTile(GridManager gridManager, Vector2Int pos)
    {
        GridCell cell =
            gridManager.Board.GetCell(pos.x, pos.y);

        if (cell == null || cell.IsEmpty)
            return;

        ControlTile tile =
            cell.linkedTile;

        if (tile != null)
        {
            tile.DestroyTile();
        }

        gridManager.Board.SetCellEmpty(pos.x, pos.y);
    }

    // =========================
    // SHUFFLE LIST
    // =========================
    void Shuffle(List<Vector2Int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex =
                Random.Range(i, list.Count);

            Vector2Int temp = list[i];

            list[i] = list[randomIndex];

            list[randomIndex] = temp;
        }
    }
}
