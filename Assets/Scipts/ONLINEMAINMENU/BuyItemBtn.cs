using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyItemBtn : ButtonBase
{
    [SerializeField]private ShopManager shopManager;
    public ItemType ItemType;
    public int price;
    public string itemId;

    public override void OnClick()
    {
        shopManager.BuyItem(itemId,price,ItemType);
    }
}
