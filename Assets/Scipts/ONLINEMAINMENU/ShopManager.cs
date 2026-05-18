using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;
using System.Collections.Generic;

public class ShopManager : SingletonBase<ShopManager>
{
    public PlayerData localPlayerData => PlayFabDataManager.Instance.playerData;
    // private const string DATA_KEY = "Player_Game_Data";

    [Header("Shop Settings")]
    [SerializeField] private int startingGold = 1000;

    // --- LOGIC MUA HÀNG CHUNG ---
    public void BuyItem(string itemId, int price, ItemType type, int amount = 1)
    {
        // 1. Kiểm tra tiền
        if (localPlayerData.gold < price)
        {
            Debug.LogError($"Không đủ vàng! Cần: {price}, Hiện có: {localPlayerData.gold}");
            return;
        }

        // 2. Logic riêng cho từng loại
        if (type == ItemType.Skin)
        {
            if (localPlayerData.HasItem(itemId))
            {
                Debug.LogWarning("Bạn đã sở hữu Skin/Line này rồi!");
                return;
            }
            amount = 1; // Skin luôn mua 1
        }

        // 3. Thực thi trừ tiền và thêm vật phẩm
        localPlayerData.gold -= price;
        localPlayerData.AddItem(itemId, amount, type);

        Debug.Log($"<color=green>Mua thành công!</color> {itemId}. Còn lại: {localPlayerData.gold} {localPlayerData.GetItemCount(itemId)}");

        PlayFabDataManager.Instance.SavePlayerData();

    }

    // --- CÁC HÀM ĐỂ GẮN VÀO BUTTON TRÊN UI ---

    // Gắn vào nút mua PowerUp (ví dụ: Bom, Gợi ý, Đóng băng...)
    public void PurchasePowerUp(string id)
    {
        // Giả sử giá cố định hoặc bạn có thể lấy từ một ScriptableObject
        int price = 100;
        BuyItem(id, price, ItemType.PowerUp, 1);
    }

    // Gắn vào nút mua Skin Tile hoặc Line
    public void PurchaseSkin(string id)
    {
        int price = 500;
        BuyItem(id, price, ItemType.Skin);
    }

    // Gắn vào nút "Sử dụng" (Equip) trong Shop
    public void EquipItem(string id, bool isTile)
    {
        bool success = false;
        if (isTile)
            success = localPlayerData.EquipTile(id);
        else
            success = localPlayerData.EquipLine(id);

        if (success)
        {
            Debug.Log($"Đã trang bị: {id}");
            PlayFabDataManager.Instance.SavePlayerData();
        }
    }

   
    //// --- PLAYFAB CLOUD SYNC ---

    //public void SaveDataToPlayFab()
    //{
    //    localPlayerData.ValidateData(); // Kiểm tra tính toàn vẹn trước khi lưu

    //    string jsonStr = JsonConvert.SerializeObject(localPlayerData);

    //    var request = new UpdateUserDataRequest
    //    {
    //        Data = new Dictionary<string, string> { { DATA_KEY, jsonStr } },
    //        Permission = UserDataPermission.Public
    //    };

    //    PlayFabClientAPI.UpdateUserData(request,
    //        result => Debug.Log("Dữ liệu đã được đồng bộ lên PlayFab!"),
    //        error => Debug.LogError("Lỗi đồng bộ: " + error.GenerateErrorReport())
    //    );
    //}

    //public void LoadDataFromPlayFab()
    //{
    //    PlayFabClientAPI.GetUserData(new GetUserDataRequest(), result => {
    //        if (result.Data != null && result.Data.ContainsKey(DATA_KEY))
    //        {
    //            localPlayerData = JsonConvert.DeserializeObject<PlayerData>(result.Data[DATA_KEY].Value);
    //            localPlayerData.ValidateData();
    //            Debug.Log("Tải dữ liệu thành công!");
    //        }
    //        else
    //        {
    //            // Khởi tạo cho người chơi mới
    //            localPlayerData = new PlayerData();
    //            localPlayerData.gold = startingGold;
    //            localPlayerData.ValidateData();
    //            SaveDataToPlayFab();
    //        }
    //    }, error => Debug.LogError("Lỗi tải dữ liệu: " + error.GenerateErrorReport()));
    //}
}