using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using Newtonsoft.Json;

public enum ItemType
{
    Skin = 0,
    PowerUp = 1
}
[System.Serializable]
public class InventoryItem
{
    public string id;
    public int quantity; // Số lượng (PowerUp dùng > 1, Skin dùng 1)
    public ItemType type;
}

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public int gold;
    public int score;

    // CÁI ĐANG DÙNG
    public string currentTileId = "tile_default";
    public string currentLineId = "line_default";

    // CÁI ĐÃ SỞ HỮU (Chứa cả Skin đã mua và PowerUp đang có)
    public List<InventoryItem> inventory = new List<InventoryItem>();

    // Tiến trình game
    public int highestLevel = 0;

    // lấy ietm
    public InventoryItem GetItem(string idItem)
    {
        return inventory.Find(i => i.id == idItem);
    }


    // hàm lấy số lượng
    public int GetItemCount(string idItem) 
    { 
        var item = GetItem(idItem);
        if (item == null) return 0;
        return item.quantity;
    }
    public bool HasItem(string idItem)
    {
        return inventory.Exists(i => i.id == idItem);
    }

    public void AddItem(string idItem, int amount, ItemType type)
    {
        // Kiểm tra xem trong túi đồ đã có món này chưa
        var item = inventory.Find(i => i.id == idItem);

        if (item == null)
        {
            // Nếu chưa có thì mới thêm mới vào List
            inventory.Add(new InventoryItem { id = idItem, quantity = amount, type = type });
            Debug.Log("Thêm mới item vào inventory");
        }
        else
        {
            // Nếu có rồi thì chỉ CỘNG DỒN số lượng (quantity)
            item.quantity += amount;
            Debug.Log("Cộng dồn số lượng cho item đã có");
        }
    }

    // Dùng item (power-up)
    public bool UseItem(string id, int amount = 1)
    {
        var item = GetItem(id);

        if (item == null || item.quantity < amount)
            return false;

        item.quantity -= amount;

        // nếu hết thì xóa khỏi inventory
        if (item.quantity <= 0)
        {
            inventory.Remove(item);
        }

        return true;
    }

    // Equip tile
    public bool EquipTile(string id)
    {
        var item = GetItem(id);

        if (item == null || item.type != ItemType.Skin)
            return false;

        currentTileId = id;
        return true;
    }

    // Equip line
    public bool EquipLine(string id)
    {
        var item = GetItem(id);

        if (item == null || item.type != ItemType.Skin)
            return false;

        currentLineId = id;
        return true;
    }

    // Check data hợp lệ (QUAN TRỌNG)
    public void ValidateData()
    {
        // đảm bảo luôn có default
        if (!HasItem("tile_default"))
        {
            AddItem("tile_default", 1, ItemType.Skin);
        }

        if (!HasItem("line_default"))
        {
            AddItem("line_default", 1, ItemType.Skin);
        }

        // fix equip nếu bị lỗi
        if (!HasItem(currentTileId))
        {
            currentTileId = "tile_default";
        }

        if (!HasItem(currentLineId))
        {
            currentLineId = "line_default";
        }
    }
}

    public class PlayFabDataManager : SingletonBase<PlayFabDataManager>
    {

        public PlayerData playerData;


    // ================= LOAD =================
    public void LoadPlayerData(System.Action onDone = null)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(),
            result =>
            {
                if (result.Data != null && result.Data.ContainsKey("PlayerData"))
                {
                    // ✅ Có data
                    string json = result.Data["PlayerData"].Value;
                    playerData = JsonUtility.FromJson<PlayerData>(json);

                    // 🔥 Fix data lỗi / thiếu
                    playerData.ValidateData();

                   GameManager.Instance.TotalCoinOnline = playerData.gold;
                    Debug.Log("vàng là"+ playerData.gold);
                   GameManager.Instance.HighScoreOnline = playerData.score;
                    Debug.Log("Load data thành công");
                }
                else
                {
                    // ❗ User mới → tạo data
                    Debug.Log("User mới → tạo PlayerData");

                    playerData = CreateDefaultData();

                    // 🔥 GẮN DATA VÀO ACCOUNT
                   // SavePlayerData();
                }

                onDone?.Invoke();
            },
            error => Debug.LogError(error.GenerateErrorReport()));
    }


    // ================= SAVE =================
    public void SavePlayerData()
        {
            string json = JsonUtility.ToJson(playerData);

            var request = new UpdateUserDataRequest()
            {
                Data = new Dictionary<string, string>()
            {
                { "PlayerData", json }
            }
            };

            PlayFabClientAPI.UpdateUserData(request,
                result => Debug.Log("Save thành công"),
                error => Debug.LogError(error.GenerateErrorReport()));
        }

        // ================= DEFAULT =================
        PlayerData CreateDefaultData()
        {
            return new PlayerData()
            {
                playerName = "",
                gold = 100,
                score = 0,
                highestLevel = 1,

                currentTileId = "tile_default",
                currentLineId = "line_default",

                inventory = new List<InventoryItem>()
        {
            new InventoryItem { id = "tile_default", quantity = 1,type=ItemType.Skin },
            new InventoryItem { id = "line_default", quantity = 1, type =ItemType.Skin }
        }
            };
        }

   
}