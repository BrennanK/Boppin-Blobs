using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowToManager : MonoBehaviour
{
    public GameObject mainCanvus;
    public GameObject[] uiStuff;
    int currentTut = -1;
    AudioSource audioController;


    public void Exit()
    {
        for(int i = 0; i < uiStuff.Length; i++)
        {
            uiStuff[i].SetActive(false);
        }
        currentTut = -1;

        mainCanvus.SetActive(false);
    }

    public void Next()
    {
        if (currentTut == uiStuff.Length - 1)
        {
            Exit();
        }
        else
        {
            currentTut++;

            print(currentTut);
            uiStuff[currentTut].SetActive(true);

            if (currentTut < 0)
            {
                uiStuff[currentTut - 1].SetActive(false);
            }
        }
    }

    public void GoBack()
    {
        if (currentTut > 0)
        {
            currentTut--;

            print(currentTut);
            uiStuff[currentTut].SetActive(true);
            uiStuff[currentTut + 1].SetActive(false);
            
        }
    }
}
