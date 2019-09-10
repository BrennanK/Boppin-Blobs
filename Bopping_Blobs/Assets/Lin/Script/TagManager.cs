using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.ThirdPerson;

public class TagManager : MonoBehaviour
{
    public GameObject[] allPlayers;
    public GameObject player;
    private int scoreNumber;
    public Text scoreText;
    private int oldTag = 10;

    void Start()
    {
        for (int i = 0; i < allPlayers.Length; i++)
        {
            allPlayers[i].GetComponent<PlayerTag>().tagManager = this;
            allPlayers[i].GetComponent<PlayerTag>().playerNumber = i;
        }

        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void ChangeTag(int num)
    {
        player.GetComponent<ThirdPersonCharacter>().BouncingBack();
        if (num != oldTag)
        {
            oldTag = num;
            scoreNumber++;
            scoreText.text = scoreNumber.ToString();
            for (int i = 0; i < allPlayers.Length; i++)
            {
                if (i == num)
                {
                    allPlayers[i].GetComponent<PlayerTag>().ActivateTag();
                }
                else
                {
                    allPlayers[i].GetComponent<PlayerTag>().DisableTag();
                }
            }
        }
    }
}
