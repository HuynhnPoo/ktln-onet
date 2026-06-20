using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UseItemBtn : ButtonBase
{
    public ItemType itemType; // Vẫn giữ nguyên là ItemType.Skin
    public string itemId;
    private ShopItemSlot slot; // Cha trực tiếp

    private void Awake()
    {
        slot = GetComponentInParent<ShopItemSlot>(true);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UpdateVisualState();
    }

    public void UpdateVisualState()
    {
        if (PlayFabDataManager.Instance == null || PlayFabDataManager.Instance.playerData == null)
        {
            if (button == null) button = GetComponent<Button>();
            if (button != null) button.interactable = false;
            return;
        }

        PlayerData data = PlayFabDataManager.Instance.playerData;
        bool isEquipped = false;

        if (itemType == ItemType.Skin)
        {
            if (itemId.Contains("line"))
            {
                isEquipped = (data.currentLineId == itemId);
            }
            else if (itemId.Contains("tile"))
            {
                isEquipped = (data.currentTileId == itemId);
            }
        }

        SetState(isEquipped);
    }

    public void SetState(bool isEquipped)
    {
        if (button == null) button = GetComponent<Button>();
        if (button != null)
        {
            // Nếu đã trang bị thì tắt nút (không cho bấm nữa)
            button.interactable = !isEquipped;
        }
    }

    public override void OnClick()
    {
        if (PlayFabDataManager.Instance == null || PlayFabDataManager.Instance.playerData == null) return;

        PlayerData data = PlayFabDataManager.Instance.playerData;
        bool equipSuccess = false;

        if (itemId.Contains("line"))
        {
            equipSuccess = data.EquipLine(itemId);
        }
        else if (itemId.Contains("tile"))
        {
            equipSuccess = data.EquipTile(itemId);
        }

        if (equipSuccess)
        {
            Debug.Log($"[Equip] Đã trang bị thành công: {itemId}");
            PlayFabDataManager.Instance.SavePlayerData();

            // Áp dụng skin vào bàn chơi
            if (itemId.Contains("line"))
            {
                LineController lineCtrl = FindObjectOfType<LineController>();
                if (lineCtrl != null) lineCtrl.ApplyCurrentSkin();
            }
            else if (itemId.Contains("tile"))
            {
                GridManager gridManager = FindObjectOfType<GridManager>();
                if (gridManager != null) gridManager.ApplyNewSkinPack(itemId);
            }

            // --- ĐOẠN TỰ ĐỘNG PHÂN TÁCH PANEL KHI CẬP NHẬT UI ---
            // Tìm tất cả các slot trong Shop
            ShopItemSlot[] allSlots = FindObjectsOfType<ShopItemSlot>(true);
            foreach (var currentSlot in allSlots)
            {
                UseItemBtn useBtnInSlot = currentSlot.GetComponentInChildren<UseItemBtn>(true);
                if (useBtnInSlot != null)
                {
                    // TỰ ĐỘNG LỌC: 
                    // Nếu nút vừa bấm là "line" và nút đang xét cũng là "line" -> Cập nhật
                    // Nếu nút vừa bấm là "tile" và nút đang xét cũng là "tile" -> Cập nhật
                    bool isSamePanel = (this.itemId.Contains("line") && useBtnInSlot.itemId.Contains("line")) ||
                                       (this.itemId.Contains("tile") && useBtnInSlot.itemId.Contains("tile"));

                    if (isSamePanel)
                    {
                        // 1. Cập nhật trạng thái bật/tắt (interactable) của nút đó
                        useBtnInSlot.UpdateVisualState();

                        // 2. CẬP NHẬT QUAN TRỌNG: Truyền "this.itemId" (ID của món đồ vừa ĐƯỢC CHỌN) 
                        // vào hàm UpdateSlotUI để text hiển thị đúng (ví dụ: chuyển thành chữ "Đang dùng")
                        currentSlot.UpdateSlotUI(useBtnInSlot.itemType, useBtnInSlot.itemId);
                    }
                }
            }
        }
    }
}