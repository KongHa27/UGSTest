using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

public class CloudSaveManager : MonoBehaviour
{
    public InputField levelInput;
    public InputField goldInput;

    public static async Task SavePlayerData(int level, int gold)
    {
        var data = new Dictionary<string, object>()
        {
            {"Level", level },
            {"Gold", gold},
        };

        try
        {
            await CloudSaveService.Instance.Data.Player.SaveAsync(data);
            Debug.Log($"Data Saved! Lv.{level}, Gold: {gold}");
        }
        catch (CloudSaveException ex)
        {
            Debug.LogError($"Save date failed : {ex.Message}");
        }
    }

    public static async Task LoadPlayerData()
    {
        try
        {
            var keys = new HashSet<string> { "Level", "Gold" };

            var loadedData = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);

            if (loadedData.TryGetValue("Level", out var levelData))
            {
                int level = levelData.Value.GetAs<int>();
                Debug.Log($"Player Level : {level}");
            }

            if (loadedData.TryGetValue("Gold", out var goldData))
            {
                int gold = goldData.Value.GetAs<int>();
                Debug.Log($"Player Level : {gold}");
            }
        }
        catch (CloudSaveException ex)
        {
            Debug.LogError($"Load Data failed : {ex.Message}");
        }
    }

    public void OnClickSaveBtn()
    {
        ClickSaveBtn();
    }

    public async Task ClickSaveBtn()
    {
        int level = int.Parse(levelInput.text);
        int gold = int.Parse(goldInput.text);

        await CloudSaveManager.SavePlayerData(level, gold);
        await CloudSaveManager.LoadPlayerData();
    }
}
