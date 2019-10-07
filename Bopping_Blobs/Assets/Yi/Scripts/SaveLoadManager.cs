// Name: Yi Li
// Date: 09/26/2019
// Purpose: Manage Save and Load Functionalities
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveLoadManager : MonoBehaviour
{
    // References
    private SampleInventory sampleInventory;
    private AchievementManager achievementManager;

    // Start is called before the first frame update
    void Start()
    {
        achievementManager = AchievementManager._instance;
        sampleInventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<SampleInventory>();
        LoadGameFunction();
        sampleInventory.UpdateCoins();
    }

    // Create a new save data class
    private SaveData CreateSaveInfo()
    {
        SaveData saveData = new SaveData();

        // Store game info into save data
        if (achievementManager)
            saveData.AchievementInfo = new List<string[]>(achievementManager.Achievements);
        else
            Debug.LogWarning("Can not find achievement manager!");

        if (sampleInventory)
            saveData.MoneyInfo = sampleInventory.CurrentCoins1;
        else
            Debug.LogWarning("Can not find inventory manager!");

        return saveData;
    }

    // Save and Load button functionalities

    public void SaveGameFunction()
    {
        SaveData saveData = CreateSaveInfo();

        // Translate to Binary and create a local file
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        FileStream fileStream = File.Create(Application.persistentDataPath + "/SaveData.AWSL");
        binaryFormatter.Serialize(fileStream, saveData);
        fileStream.Close();

        Debug.Log("<color=red> Game Saved! </color>");
    }

    public void LoadGameFunction()
    {
        if (sampleInventory) {
            // TODO TEMP Setting Current Coins to whatever is saved on player prefs; the string where it is saved should be a variable
            sampleInventory.CurrentCoins1 = PlayerPrefs.GetInt("SAVED_AMOUNT_OF_COINS");
        }

        if (File.Exists(Application.persistentDataPath + "/SaveData.AWSL"))
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = File.Open(Application.persistentDataPath + "/SaveData.AWSL", FileMode.Open);
            SaveData saveData = (SaveData)binaryFormatter.Deserialize(fileStream);
            fileStream.Close();

            // Load save file's data into game
            if (achievementManager)
            {
                // The list may need to be cleared before loading
                //achievementManager.Achievements.Clear();

                achievementManager.Achievements = saveData.AchievementInfo.ToList();
            }
            else
                Debug.LogWarning("Can not find achievement manager!");

            if (sampleInventory)
            {
                // sampleInventory.CurrentCoins1 = saveData.MoneyInfo;
            }
            else
                Debug.LogWarning("Can not find inventory manager!");

            Debug.Log("<color=red> Game Loaded! </color>");
        }
        else
            Debug.LogError("No save data file found!");
    }

    // Autosave when closing the game
    private void OnApplicationQuit()
    {
        //SaveGameFunction();
    }
}
