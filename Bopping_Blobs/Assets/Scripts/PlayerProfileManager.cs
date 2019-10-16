using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerProfileManager : MonoBehaviour {
    public GameObject playerProfileCanvas;
    public TextMeshProUGUI[] profileEntryTexts;

    private void Start() {
        HidePlayerProfile();
    }

    private void UpdatePlayerProfile() {
        // TODO currently this is hard coded and it is kinda bad
        if(profileEntryTexts.Length != 5) {
            return;
        }

        SaveData saveData = SaveGameManager.instance.SaveDataInfo;
        profileEntryTexts[0].text = $"<color=#814F2F>Games Played:</color> {saveData.playerProfile.gamesPlayed}";
        profileEntryTexts[1].text = $"<color=#814F2F>Time Played:</color> {saveData.playerProfile.timePlayed} seconds";
        profileEntryTexts[2].text = $"<color=#814F2F>Times King:</color> {saveData.playerProfile.timesKing}";
        profileEntryTexts[3].text = $"<color=#814F2F>Time as King:</color> {Mathf.Round(saveData.playerProfile.timeAsKing)} seconds";
        profileEntryTexts[4].text = $"<color=#814F2F>Time attacked Blobs:</color> {saveData.playerProfile.timesAttackedBlobs}";
    }

    public void ShowPlayerProfile() {
        UpdatePlayerProfile();
        playerProfileCanvas.SetActive(true);
    }

    public void HidePlayerProfile() {
        playerProfileCanvas.SetActive(false);
    }
}
