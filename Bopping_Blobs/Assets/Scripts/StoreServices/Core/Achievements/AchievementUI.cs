using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementUI : MonoBehaviour {
    public Image achievementBackgroundImage;
    public TextMeshProUGUI achievementName;
    public TextMeshProUGUI achievementDescription;
    public TextMeshProUGUI achievementCompletion;

    public void UpdateAchievement(string _name, string _description, string _completion) {
        achievementName.text = _name;
        achievementDescription.text = _description;
        achievementCompletion.text = _completion;
    }
}
