using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="tile",menuName ="Onet/tiles")]
public class TileData : ScriptableObject
{


    [Header("Thông tin cơ bản")]
    public string skinName = "Skin Animals";   // Tên skin (hiển thị UI)
    public int price = 100;                     // Giá mua (coins)
    public bool isUnlocked = true;              // Đã mua/unlock chưa
    public bool isSelected = false;             // Skin này đang được chọn?

    [Header("Hình ảnh - 60 loại tile")]
    public Sprite[] tileSprites = new Sprite[15];  // Index 0 → type 0, index 1 → type 1, ...

    [Header("Thông số áp dụng cho tất cả tile")]
    [Range(0.01f, 0.5f)] public float scaleMultiplier = 1f;   // Kích thước (scale)
    public Color tintColor = Color.white;                  // Màu tint
    [Range(-30f, 30f)] public float rotationOffset = 0f;   // Xoay thêm (độ)
      
}

[CreateAssetMenu(fileName = "TileDatabase", menuName = "Onet/TileDatabase")]
public class TileDatabase : ScriptableObject
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

    // Hàm mua (gọi từ UI Shop)
    public bool BuySkinPack(TileData packToBuy, int playerCoins)
    {
        if (packToBuy.isUnlocked) return true;  // Đã mua rồi

        if (playerCoins >= packToBuy.price)
        {
            // Trừ coins (bạn tự implement hệ thống coins)
            // playerCoins -= packToBuy.price;

            packToBuy.isUnlocked = true;
            SelectSkinPack(packToBuy);  // Mua xong auto chọn luôn (hoặc để người chơi chọn sau)
            return true;
        }
        return false;  // Không đủ tiền
    }
}
