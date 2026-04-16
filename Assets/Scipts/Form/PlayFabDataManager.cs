using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;

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

    public bool HasItem(string idItem)
    {
        return inventory.Exists(i => i.id == idItem);
    }

    public void AddItem(string idItem, int amount, ItemType type)
    {
        var item = GetItem(idItem);
        if (item == null)
        {
            inventory.Add(new InventoryItem { id = idItem, quantity = amount, type = type });
        }
        else
        {
            item.quantity += amount;
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

                    Debug.Log("Load data thành công");
                }
                else
                {
                    // ❗ User mới → tạo data
                    Debug.Log("User mới → tạo PlayerData");

                    playerData = CreateDefaultData();

                    // 🔥 GẮN DATA VÀO ACCOUNT
                    SavePlayerData();
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