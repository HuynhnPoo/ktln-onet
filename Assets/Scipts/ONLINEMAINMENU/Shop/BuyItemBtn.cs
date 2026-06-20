using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyItemBtn : ButtonBase
{
    [SerializeField] private ShopManager shopManager;
    public ItemType itemType; // kieu item
    public int price; // gia tien
    public string itemId; // ten item

    ShopItemSlot itemSlot;

    private void Awake()
    {
        itemSlot = GetComponentInParent<ShopItemSlot>(true);

    }


    public override void OnClick()
    {
        shopManager.BuyItem(itemId, price, itemType);
        itemSlot.UpdateSlotUI(itemType, itemId);
    }

}
