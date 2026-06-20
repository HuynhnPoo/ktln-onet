
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileDatabase", menuName = "Onet/TileDatabase")]
public class TileDatabaseList : ScriptableObject
{
    public List<TileData> allSkinPacks = new List<TileData>();  // Danh sách các bộ skin để bán

    public TileData GetSelectedSkinPack()
    {
        foreach (var pack in allSkinPacks)
            if (pack.isSelected) return pack;
        return allSkinPacks.Count > 0 ? allSkinPacks[0] : null;  // Default bộ đầu nếu chưa chọn
    }

    public void SelectSkinPack(TileData selectedPack)
    {
        foreach (var pack in allSkinPacks)
            pack.isSelected = (pack == selectedPack);
    }

    public void SetSelectedSkinPack(string itemId)
    {
        foreach (var pack in allSkinPacks)
        {
            if (pack.name == itemId)
            {
                SelectSkinPack(pack); // ✅ Dùng lại hàm có sẵn
                return;
            }
        }
        Debug.LogWarning($"[TileDatabase] Không tìm thấy skin: {itemId}");
    }
}
