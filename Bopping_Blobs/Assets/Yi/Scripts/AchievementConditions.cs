using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementConditions : MonoBehaviour
{
    // Required Conditions
    [SerializeField] int PlaceInMatch, NumbersOfTagged, NumbersOfPowerups, NumbersOfBeingTagged;
    // Saved Info
    
    public int winorlose, place, numbersoftagged, numbersofpowerups, numbersofbeingtagged;

    private AchievementManager achievementManager;

    // Start is called before the first frame update
    void Start()
    {
        achievementManager = AchievementManager._instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MeetConditions(int number)
    {
        switch (number) {
            case 1: // Check win or lose
                if (winorlose == 1) // Serial number starts from 1
                {
                    achievementManager.CompleteAchievement(1); 
                    Debug.Log("The player wins the game.");
                }
                else if (winorlose == 0) // 1 = win, 0 = lose
                {
                    achievementManager.CompleteAchievement(2);
                    Debug.Log("The player loses the game.");
                }
                break;
            case 2:
                if (place == PlaceInMatch)
                    achievementManager.CompleteAchievement(3);
                Debug.Log("The player got a " + PlaceInMatch + " place in the game!");
                break;
            case 3:
                if (numbersoftagged == NumbersOfTagged)
                    achievementManager.CompleteAchievement(4);
                Debug.Log("The player tagged " + numbersoftagged + " people in one game.");
                break;
            case 4:
                if (numbersofpowerups == NumbersOfPowerups)
                    achievementManager.CompleteAchievement(5);
                Debug.Log("The player used " + numbersofpowerups + " powerups in one game.");
                break;
            case 5:
                if (numbersofbeingtagged == NumbersOfBeingTagged)
                    achievementManager.CompleteAchievement(6);
                Debug.Log("The player was tagged " + numbersofbeingtagged + " times in one game.");
                break;
            default:
                Debug.Log("Nothing happened.");
                break;
                }
    }
}
