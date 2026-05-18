using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostBtn : ButtonBase
{
    // Kéo trực tiếp file Asset (Bomb hoặc Hint) từ thư mục Project vào đây
    [SerializeField] private BoostBase boostAsset;
    [SerializeField] private GridManager gridManager;

    public override void OnClick()
    {
        if (boostAsset != null)
        {
            // Gọi đích danh, không bao giờ sợ chạy nhầm sang hàm của class khác!
            boostAsset.Use(gridManager);
        }
        else
        {
            Debug.LogError("Bạn chưa kéo file SO Boost vào Button này!");
        }
    }
}
