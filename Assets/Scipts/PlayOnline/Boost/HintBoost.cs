using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HintBoost", menuName = "Boosts/Hint")]
public class HintBoost : BoostBase
{
    // Hàm này giữ nguyên theo cấu trúc cha: Chỉ kiểm tra xem GridManager có tồn tại không
    protected override bool CanExecute(GridManager grid)
    {
        return grid != null && grid.Board != null;
    }

    // Hàm Execute kiểu void - đúng kiến trúc hệ thống bạn yêu cầu
    protected override void Execute(GridManager gridManager)
    {
        if (!PlayFabDataManager.Instance.playerData.HasItem(itemId) ||
            PlayFabDataManager.Instance.playerData.GetItemCount(itemId) <= 0)
        {
            Debug.LogWarning($"[PlayFab] Đã hết lượt dùng vật phẩm gợi ý: {itemId}");
            return; 
        }

        for (int x1 = 0; x1 < gridManager.Width; x1++)
        {
            for (int y1 = 0; y1 < gridManager.Height; y1++)
            {
                GridCell cellA = gridManager.Board.GetCell(x1, y1);

                // Nếu ô trống hoặc không tồn tại, bỏ qua để tìm ô tiếp theo
                if (cellA == null || cellA.IsEmpty)
                    continue;

                for (int x2 = 0; x2 < gridManager.Width; x2++)
                {
                    for (int y2 = 0; y2 < gridManager.Height; y2++)
                    {
                        // Không tự so sánh ô đó với chính nó
                        if (x1 == x2 && y1 == y2)
                            continue;

                        GridCell cellB = gridManager.Board.GetCell(x2, y2);

                        if (cellB == null || cellB.IsEmpty)
                            continue;

                        // Nếu 2 ô không trùng loại hình ảnh (iconID) thì bỏ qua
                        if (cellA.iconID != cellB.iconID)
                            continue;

                        // Kiểm tra đường đi Pikachu (I, L, Z) giữa cellA và cellB
                        var path = GameMechanics.GetPath(
                            new Vector2Int(x1, y1),
                            new Vector2Int(x2, y2),
                            gridManager.Board);

                        if (path != null)
                        {
                            Debug.Log($"[Hint] Tìm thấy cặp hợp lệ trùng ID: {cellA.iconID}. Tiến hành hiển thị!");

                            gridManager.HighlightTwoCells(cellA, cellB); // thực hiện high nhấp nhấy tile 

                           // PlayFabDataManager.Instance.playerData.UseItem(itemId, 1); //giảm số lượng
                            PlayFabDataManager.Instance.SavePlayerData(); // lưu dữ lên playfabs

                            return;
                        }
                    }
                }
            }
        }

      
        Debug.LogWarning("[Hint] Bàn chơi hiện tại không còn nước đi hợp lệ để gợi ý! (Không bị trừ đạo cụ)");
    }
}