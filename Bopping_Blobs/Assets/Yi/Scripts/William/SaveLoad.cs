using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoad : MonoBehaviour
{
    //---------------------total achievement type---------------------------------------------------------

    public TimeInvestment[] timeInvestments; // dynamically instantiate at run time

    //------------------------------------------------------------------------------
    public Dictionary<string, Ach> data = new Dictionary<string, Ach>();

    public static SaveLoad instance;

    // It was originally an Awake(), but we need to know the number of timeInvestment after it instantiate, so we wait for the AchievementManager to Instantiate first
    private void Start()
    {
        if (instance != null)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);
        }

        TimeInvestment[] temp = GameObject.FindObjectsOfType<TimeInvestment>();

        timeInvestments = new TimeInvestment[temp.Length];

        for (int i = 0; i < temp.Length; i++)
        {
            timeInvestments[i] = temp[temp.Length - i - 1];
        }

        // putting every achievement into a dictionary 
        for (int i = 0; i < timeInvestments.Length; i++)
        {
            data.Add(timeInvestments[i].gameObject.name, timeInvestments[i]);
        }

        foreach (var timeInvestment in timeInvestments)
        {
            if (PlayerPrefs.HasKey(timeInvestment.gameObject.name))
            {
                int currentState = PlayerPrefs.GetInt(timeInvestment.gameObject.name);
                switch (currentState)
                {
                    case 0:
                        timeInvestment.state = TimeInvestment.UIState.INCOMPLETE;
                        break;
                    case 1:
                        timeInvestment.state = TimeInvestment.UIState.COMPLETED_UNCLAIMED;
                        break;
                    case 2:
                        timeInvestment.state = TimeInvestment.UIState.COMPLETED_CLAIMED;
                        break;
                }
            }
        }
    }

}
