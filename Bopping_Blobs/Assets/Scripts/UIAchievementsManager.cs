using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIAchievementsManager : MonoBehaviour {
    [Header("UI Integration")]
    public Transform entriesParent;
    public GameObject prefabEntry;
    public GameObject prefabCanvas;

    private void Start() {
        StoreServices.Core.Achievements.AchievementInstance[] achievementInstances = StoreServices.AchievementManager.instance.AchievementInstances;

        for(int i = 0; i < achievementInstances.Length; i++) {
            GameObject achievementEntry = Instantiate(prefabEntry, entriesParent);
            float percentageProgress = achievementInstances[i].ProgressInPercentage;
            percentageProgress *= 100;
            achievementEntry.GetComponentInChildren<TextMeshProUGUI>().text = $"{achievementInstances[i].AchievementName}\n{Mathf.Round(percentageProgress)}%";
        }

        prefabCanvas.SetActive(false);
    }

    public void ShowAchievements() {
        prefabCanvas.SetActive(true);
    }

    public void HideAchievements() {
        prefabCanvas.SetActive(false);
    }
}
