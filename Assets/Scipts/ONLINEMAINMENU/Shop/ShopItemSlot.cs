using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItemSlot : MonoBehaviour
{
    private BuyItemBtn buyButton;
    private UseItemBtn useButton;

    private void Awake()
    {
        // Tự động tìm kiếm các script ở các nút con nằm bên trong nó
        buyButton = GetComponentInChildren<BuyItemBtn>(true);
        useButton = GetComponentInChildren<UseItemBtn>(true);
    }

    // --- ĐÂY LÀ NƠI XỬ LÝ KHI BẮT ĐẦU GAME / MỞ SHOP ---
    private void OnEnable()
    {
        StartCoroutine(CheckUIOnStart());
    }

    private IEnumerator CheckUIOnStart()
    {
        // Đợi đúng 1 khung hình để chắc chắn các hàm Awake/Start của các Singleton khác đã chạy xong
        yield return null;

        // Bây giờ check Singleton sẽ cực kỳ an toàn, không lo bị Null nữa
        if (buyButton != null && !string.IsNullOrEmpty(buyButton.itemId))
        {
            Debug.Log("khoi đầu của button"+buyButton);
            UpdateSlotUI(buyButton.itemType, buyButton.itemId);
        }
    }

    public void UpdateSlotUI(ItemType itemType, string itemId)
    {
        var playerData = ShopManager.Instance.localPlayerData;
        if (playerData == null) return;

        // Lấy TẤT CẢ các nút BuyBtn con nằm trong Slot này thay vì chỉ lấy 1 nút duy nhất
        BuyItemBtn[] allBuyButtons = GetComponentsInChildren<BuyItemBtn>(true);

        if (itemType == ItemType.Skin)
        {
            if (playerData.HasItem(itemId))
            {
                Debug.Log(itemId + " trong shop item ui (Đã mua từ PlayFab)");

                // --- SỬA Ở ĐÂY: Duyệt qua tất cả các nút Buy con để chỉ tắt nút Skin ---
                if (allBuyButtons != null)
                {
                    foreach (var btn in allBuyButtons)
                    {
                        // CHỈ tắt những nút nào có id trùng khớp và đúng kiểu Skin
                        if (btn != null && btn.itemId == itemId && btn.itemType == ItemType.Skin)
                        {
                            btn.gameObject.SetActive(false);
                        }
                    }
                }

                // Bật nút Use lên vì trang phục này đã sở hữu
                if (useButton != null) useButton.gameObject.SetActive(true);

                // Cập nhật trạng thái cho nút Use (Đang dùng hay Chưa dùng)
                if (useButton != null)
                {
                    bool isEquipped = false;
                    if (itemId.Contains("line"))
                        isEquipped = (itemId == playerData.currentLineId);
                    else if (itemId.Contains("tile"))
                        isEquipped = (itemId == playerData.currentTileId);
                    useButton.SetState(isEquipped);
                }
            }
            else
            {
                // Chưa mua Skin này: Hiện lại các nút Buy tương ứng của nó, ẩn nút Use
                if (allBuyButtons != null)
                {
                    foreach (var btn in allBuyButtons)
                    {
                        if (btn != null && btn.itemId == itemId && btn.itemType == ItemType.Skin)
                        {
                            btn.gameObject.SetActive(true);
                        }
                    }
                }
                if (useButton != null) useButton.gameObject.SetActive(false);
            }
        }
        else
        {
            // Vật phẩm tiêu hao (PowerUp): Chỉ bật lại những nút nào có kiểu khác Skin
            if (allBuyButtons != null)
            {
                foreach (var btn in allBuyButtons)
                {
                    if (btn != null && btn.itemType != ItemType.Skin)
                    {
                        btn.gameObject.SetActive(true);
                    }
                }
            }
            if (useButton != null) useButton.gameObject.SetActive(false);

            Debug.Log("Thực hiện mua boost thành công");
        }
    }
}