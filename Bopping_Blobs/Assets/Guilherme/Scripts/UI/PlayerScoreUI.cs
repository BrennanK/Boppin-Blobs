using TMPro;
using UnityEngine;

public class PlayerScoreUI : MonoBehaviour {
    [Header("Text Components")]
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI timeAsTagText;

    public void RefreshPlayerScore( string _playerName, float _playerScore) {
        playerNameText.text = _playerName;
        timeAsTagText.text = Mathf.Round(_playerScore).ToString();
    }
}
