using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

[CreateAssetMenu(fileName = "ShuffleBoost", menuName = "Boosts/Shuffle")]
public class ShuffleBoost : BoostBase
{
    protected override bool CanExecute(GridManager grid)
    {
        return grid != null && grid.Board != null && grid.Board.gridCell != null;
    }

    protected override void Execute(GridManager gridManager)
    {
     
        List<GridCell> validCells =
            new List<GridCell>();

        List<int> iconIDs =
            new List<int>();

        // =========================
        // LẤY TOÀN BỘ TILE HỢP LỆ
        // =========================
        for (int x = 0; x < gridManager.Width; x++)
        {
            for (int y = 0; y < gridManager.Height; y++)
            {
                GridCell cell =
                    gridManager.Board.GetCell(x, y);

                if (cell == null || cell.IsEmpty)
                    continue;

                validCells.Add(cell);

                iconIDs.Add(cell.iconID);
            }
        }

        // =========================
        // RANDOM ICON
        // =========================
        for (int i = 0; i < iconIDs.Count; i++)
        {
            int randomIndex =
                Random.Range(i, iconIDs.Count);

            int temp = iconIDs[i];
            iconIDs[i] = iconIDs[randomIndex];
            iconIDs[randomIndex] = temp;
        }

        // =========================
        // GÁN LẠI ICON
        // =========================
        for (int i = 0; i < validCells.Count; i++)
        {
            GridCell cell = validCells[i];

            cell.iconID = iconIDs[i];

            // update visual
            ControlTile tile =
                cell.linkedTile;

            if (tile != null)
            {
                VisualTile visual =
                    tile.GetComponentInChildren<VisualTile>();

                visual.index = iconIDs[i];

                visual.SetSkin(gridManager.TileData);
            }
        }

        Debug.Log("SHUFFLE COMPLETE");
    }

}