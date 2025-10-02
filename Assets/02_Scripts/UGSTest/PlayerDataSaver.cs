using UnityEngine;
using Unity.Services.CloudSave;
using System.Collections.Generic;
using System.Threading.Tasks;

public static class PlayerDataSaver
{
    public static async Task SaveData(PlayerData data)
    {
        data.equippedSlotKeys.Clear();
        data.equippedItemValues.Clear();
        foreach (var pair in data.equippedItems)
        {
            data.equippedSlotKeys.Add(pair.Key);
            data.equippedItemValues.Add(pair.Value);
        }

        data.questKeys.Clear();
        data.questValues.Clear();
        foreach (var pair in data.questProgress)
        {
            data.questKeys.Add(pair.Key);
            data.questValues.Add(pair.Value);
        }

        string jsonData = JsonUtility.ToJson(data, true);

        var saveData = new Dictionary<string, object> { { "PLAYER_DATA",  jsonData } };
        await CloudSaveService.Instance.Data.Player.SaveAsync(saveData);
        Debug.Log($"Player Data Saved! \n{jsonData}");
    }

    public static async Task<PlayerData> LoadData()
    {
        var savedData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { "PLAYER_DATA"});
    
        if (savedData.TryGetValue("PLAYER_DATA", out var data))
        {
            string jsonData = data.Value.GetAs<string>();
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(jsonData);

            playerData.equippedItems = new Dictionary<string, string>();
            for (int i = 0; i < playerData.equippedSlotKeys.Count; i++)
            {
                playerData.equippedItems[playerData.equippedSlotKeys[i]] = playerData.equippedItemValues[i];
            }

            playerData.questProgress = new Dictionary<string, int>();
            for (int i = 0; i < playerData.questKeys.Count; i++)
            {
                playerData.questProgress[playerData.questKeys[i]] = playerData.questValues[i];
            }

            Debug.Log("Playrt Data Loaded!!");
            return playerData;
        }
        else
        {
            Debug.Log("No Data Found. . . Create new Player Data");
            return new PlayerData();
        }
    }
}
