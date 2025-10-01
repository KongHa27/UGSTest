using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UGSTest_JS : MonoBehaviour
{
    public static UGSTest_JS instance { get; private set; }

    public PlayerData curPlayerData;

    public Text playerLevelText;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public async void AfterLogIn()
    {
        curPlayerData = await PlayerDataSaver.LoadData();

        UpdateUI();
    }

    public void UpdateData()
    {
        curPlayerData.level++;
        curPlayerData.curEXP = 0;
        curPlayerData.stat.statPoint += 5;

        UpdateUI();
        SaveGameData();
    }

    public async void SaveGameData()
    {
        curPlayerData.lastPos = Vector3.zero;

        await PlayerDataSaver.SaveData(curPlayerData);
    }

    void UpdateUI()
    {
        playerLevelText.text = curPlayerData.level.ToString();
    }
}
