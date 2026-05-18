using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BoostBase : ScriptableObject
{
    [Header("Cấu hình ID trùng với ID trong PlayFab Inventory")]
    public string itemId;
    public ItemType itemType = ItemType.PowerUp;

    public void Use(GridManager grid)
    {
        // 1. Kiểm tra logic trên Grid (ví dụ: bàn chơi có trống không)
        // 2. Kiểm tra xem trong kho PlayFab còn item này không
        if (CanExecute(grid) && PlayFabDataManager.Instance.playerData.UseItem(itemId, 1))
        {
            // Thực thi logic game
            Execute(grid);

            // Tự động lưu lại dữ liệu mới sau khi đã trừ 1 item lên PlayFab
            PlayFabDataManager.Instance.SavePlayerData();
        }
        else
        {
            Debug.LogWarning($"Không đủ điều kiện hoặc đã hết lượt dùng item: {itemId}");
        }
    }

    protected virtual bool CanExecute(GridManager grid) => true;
    protected abstract void Execute(GridManager grid);
}
