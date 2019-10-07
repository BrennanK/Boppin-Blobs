using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TimeInvestment : Ach
{
    public int requiredPlayCount;

    public int currentPlayCount;

    // Start is called before the first frame update
    void Start()
    {
        state = SaveLoad.instance.data[gameObject.name].state;

        switch (state) // state will be decide from saveload and pass it to here
        {
            case UIState.INCOMPLETE:
                GetComponent<Button>().interactable = false;

                break;
            case UIState.COMPLETED_UNCLAIMED:
                GetComponent<Button>().interactable = true;
                GetComponent<Image>().color = Color.green;

                break;
            case UIState.COMPLETED_CLAIMED:
                GetComponent<Button>().interactable = false;
                GetComponent<Image>().color = Color.yellow;

                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ChangeState(int num)
    {
        if (num == 1)
        {
            state = UIState.COMPLETED_UNCLAIMED;

            if (PlayerPrefs.GetInt(gameObject.name) == 1)
            {
                return;
            }
            PlayerPrefs.SetInt(gameObject.name, 1);
        }
        else if (num == 2)
        {
            state = UIState.COMPLETED_CLAIMED;

            if (PlayerPrefs.GetInt(gameObject.name) == 2)
            {
                return;
            }
            PlayerPrefs.SetInt(gameObject.name, 2);
        }
        else
        {
            Debug.LogWarning("parameter wrong");
        }
    }
}
