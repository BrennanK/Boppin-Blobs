// Author: Yi Li
// Date: 08.26.2019
// Purpose: Controller achievement system
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class AchievementManager : MonoBehaviour
{
    #region Variables
    // Serial#, Name, Completed, Needed, Reward, Status, Achievement Type
    public List<string[]> Achievements = new List<string[]>
    {
        new string[]{"1", "Win a game", "0", "1", "100", "0", ""},
        new string[]{"2", "Lose a game", "0", "1", "100", "0", ""},
        new string[]{"3", "Place 7th in a game", "0", "1", "100", "0", ""},
        new string[]{"4", "Tag (3) people in one game", "0", "1", "100", "0", ""},
        new string[]{"5", "Use (3) powerups in one game", "0", "1", "100", "0", ""},
        new string[]{"6", "Play a game without being tagged", "0", "1", "100", "0", ""},
        new string[]{"7", "Play a game", "0", "1", "100", "0", "Time Investment"},
        new string[]{"8", "Play 10 games", "0", "1", "100", "0", "Time Investment"},
        new string[]{"9", "Play 50 games", "0", "1", "100", "0", "Time Investment"},
        new string[]{"10", "Play 100 games", "0", "1", "100", "0", "Time Investment"},
        new string[]{"11", "Play 500 games", "0", "1", "100", "0", "Time Investment"},
        new string[]{"12", "Place 1st", "0", "1", "100", "0", "Instance"},
    };
    // Enable or Not
    public bool achievementActivated = false;
    // References
    public GameObject AchievementPrefab;
    public GameObject AchievementUI;
    public Transform NotCompletedContent;
    public Transform CompletedContent;
    public Sprite CompletedImage;

    // Singleton
    public static AchievementManager _instance;
    // Reference of inventory
    private SampleInventory sampleInventory;
    #endregion
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }

        DontDestroyOnLoad(gameObject);

        // Heads Up: these two originally belongs to Start()
        sampleInventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<SampleInventory>();
        CreateAchievementPrefabs();
    }
    
    #region Generate Achievements
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Generate Achievements in the Canvas
    private void CreateAchievementPrefabs()
    {
        for (int i = 0; i < Achievements.Count; i++)
        {
            GameObject achievementprefab = Instantiate(AchievementPrefab, NotCompletedContent);
            achievementprefab.name = "AchievementTab" + Achievements[i][0];
            achievementprefab.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = Achievements[i][1];
            achievementprefab.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Achievements[i][2] + " / " + Achievements[i][3];
            achievementprefab.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "$" + Achievements[i][4];
            achievementprefab.GetComponent<Button>().interactable = false;
            switch (Achievements[i][5])
            {
                case "0":
                    achievementprefab.GetComponent<Image>().color = Color.white;
                    break;
                case "1":
                    int coins = int.Parse(Achievements[i][4]);
                    achievementprefab.GetComponent<Image>().color = Color.green;
                    achievementprefab.GetComponent<Button>().onClick.AddListener(() => GetCoins(coins));
                    break;
                case "2":
                    achievementprefab.transform.SetParent(CompletedContent);
                    break;
            }

            switch (Achievements[i][6])
            {
                case "Time Investment":
                    TimeInvestment TI = achievementprefab.AddComponent<TimeInvestment>();

                    switch (Achievements[i][0])
                    {
                        case "7":
                            TI.requiredGamePlayed = 1;
                            break;
                        case "8":
                            TI.requiredGamePlayed = 10;
                            break;
                        case "9":
                            TI.requiredGamePlayed = 50;
                            break;
                        case "10":
                            TI.requiredGamePlayed = 100;
                            break;
                        case "11":
                            TI.requiredGamePlayed = 500;
                            break;
                    }
                    break;
                case "Instance":
                    achievementprefab.AddComponent<Instance>();
                    break;
            }

           
        }

    }
    #endregion
    #region Complete Method
    // Receiving message from other manager to complete achievement
    public void CompleteAchievement(int serialnumber)
    {
        Debug.Log("Completing the achievement!");
        for (int i = 0; i < Achievements.Count; i++)
        {
            if (int.Parse(Achievements[i][0]) == serialnumber)
            {
                Achievements[i][5] = "1";
                GameObject[] achievementObject = GameObject.FindGameObjectsWithTag("AchievementTab");
                foreach (GameObject achievement in achievementObject)
                {
                    if (achievement.name.Contains(Achievements[i][0]))
                    {
                        int coins = int.Parse(Achievements[i][4]);
                        achievement.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = Achievements[i][3] + " / " + Achievements[i][3];
                        achievement.GetComponent<Image>().color = Color.green;
                        achievement.GetComponent<Button>().onClick.AddListener(() => GetCoins(coins));
                        achievement.GetComponent<Button>().interactable = true;
                    }
                }
            }
        }
    }

    // Reward the player after clicking the completed achievement
    public void GetCoins(int number)
    {
        Debug.Log("Getting coins!");
        // Inventory Script
        sampleInventory.ModifiyCoins(number);

        // Play Audio
        PausedMenuManager._instance.PlaySFX(0);

        // Change Button to Compeleted UI
        GameObject ButtonObject = EventSystem.current.currentSelectedGameObject;
        ButtonObject.GetComponent<Image>().color = Color.grey;
        ButtonObject.GetComponent<Button>().onClick.RemoveAllListeners();
        ButtonObject.GetComponent<Button>().interactable = false;
        ButtonObject.GetComponent<Image>().sprite = CompletedImage;
        //ButtonObject.transform.SetParent(CompletedContent);
        string ButtonNumber = ButtonObject.name.Split(' ').Last();

        for (int i = 0; i < Achievements.Count; i++)
        {
            if (Achievements[i][0] == ButtonNumber)
            {
                Achievements[i][5] = "2";
            }
        }
    }
    #endregion
    #region UI Staff
    // Enable the UI
    public void OpenAchievement()
    {
        PausedMenuManager._instance.PlaySFX(0);
        AchievementUI.GetComponent<Animator>().SetBool("Open", true);
        achievementActivated = true;
        //NotCompletedContent.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        //NotCompletedContent.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        //NotCompletedContent.GetComponent<RectTransform>().position = Vector2.zero;
    }
    // Close the UI
    public void CloseAchievement()
    {
        PausedMenuManager._instance.PlaySFX(0);
        AchievementUI.GetComponent<Animator>().SetBool("Open", false);
        achievementActivated = false;
    }
    #endregion
}
