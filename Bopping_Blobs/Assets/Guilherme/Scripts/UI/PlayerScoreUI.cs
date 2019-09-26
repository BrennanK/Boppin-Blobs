using TMPro;
using UnityEngine;

public class PlayerScoreUI : MonoBehaviour {
    [Header("Text Components")]
    public TextMeshProUGUI positionText;
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI timeAsTagText;

    public void RefreshPlayerScore(int _position, string _playerName, float _timeAsTag) {
        positionText.text = GetStringPositionFromInt(_position);
        playerNameText.text = _playerName;
        timeAsTagText.text = GetTimeStringFromFloat(_timeAsTag);
    }

    private string GetStringPositionFromInt(int _position) {
        switch(_position) {
            case 0:
                return "1st";
            case 1:
                return "2nd";
            case 2:
                return "3rd";
            case 3:
                return "4th";
            case 4:
                return "5th";
            case 5:
                return "6th";
            case 6:
                return "7th";
            case 7:
                return "8th";
            default:
                return "nth";
        }
    }

    private string GetTimeStringFromFloat(float _time) {
        // Special case for 0
        if(_time == 0) {
            return "0:00";
        }

        string time = _time.ToString();
        string[] splittedTime = time.Split('.');

        // TODO improve this
        return $"{splittedTime[0]}:{splittedTime[1].Substring(0,2)}";
    }
}
