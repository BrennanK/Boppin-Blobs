using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoad : MonoBehaviour
{
    //---------------------total achievement type---------------------------------------------------------

    public TimeInvestment[] timeInvestments; // manually drag & drop into the field

    //------------------------------------------------------------------------------
    public Dictionary<string, Ach> data = new Dictionary<string, Ach>();

    public static SaveLoad instance;

    private void Awake()
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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
