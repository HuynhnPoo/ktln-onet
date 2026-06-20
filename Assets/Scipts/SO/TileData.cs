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

