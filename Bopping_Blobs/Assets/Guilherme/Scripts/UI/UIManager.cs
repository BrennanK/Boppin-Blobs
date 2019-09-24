using TMPro;
using System.Collections;
using UnityEngine;

// TODO DOTween

public class UIManager : MonoBehaviour {
    public TextMeshProUGUI centerScreenText;

    [Header("Scoreboard Scripts")]
    public PlayerScoreUI[] playerScores;

    [Header("Timer")]
    public TextMeshProUGUI timerText;

    /// <summary>
    /// <para>Update timer on screen</para>
    /// </summary>
    /// <param name="_time">Time (in seconds) to set</param>
    public void UpdateTimerText(float _time) {
        string time = _time.ToString();
        string[] splittedTime = time.Split('.');

        if(splittedTime.Length > 1) {
            time = $"{splittedTime[0]}:{splittedTime[1].Substring(0, 2)}";
        } else {
            time = $"{splittedTime[0]}:00";
        }

        timerText.text = time;
    }

    /// <summary>
    /// <para>Update the scoreboard on UI</para>
    /// </summary>
    /// <param name="_players">List of players</param>
    public void UpdateScoreboard(TaggingIdentifier[] _players) {
        for(int i = 0; i < _players.Length; i++) {
            playerScores[i].RefreshPlayerScore(i, _players[i].PlayerName, _players[i].TimeAsTag);
        }
    }

    /// <summary>
    /// <para>Updates the text that is shown in the center</para>
    /// </summary>
    /// <param name="_text">Text to be shown</param>
    public void ShowCenterText(string _text) {
        centerScreenText.gameObject.SetActive(true);
        centerScreenText.text = _text;
    }

    /// <summary>
    /// <para>Show on screen which player was tagged</para>
    /// </summary>
    /// <param name="_playerName">Name of the player that was tagged</param>
    /// <param name="_timeToShow">Time the message will stay on screen</param>
    public void ShowPlayerTaggedText(string _playerName, float _timeToShow) {
        StartCoroutine(ShowPlayerTaggedTextRoutine(_playerName, _timeToShow));
    }

    private IEnumerator ShowPlayerTaggedTextRoutine(string _playerName, float _timeToShow) {
        centerScreenText.gameObject.SetActive(true);
        centerScreenText.text = $"{_playerName} got TAG!";
        yield return new WaitForSecondsRealtime(_timeToShow);
        centerScreenText.gameObject.SetActive(false);
    }
}
