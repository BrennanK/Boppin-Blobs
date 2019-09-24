using TMPro;
using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour {
    public TextMeshProUGUI playerWasTaggedText;

    [Header("Scoreboard Scripts")]
    public PlayerScoreUI[] playerScores;

    [Header("Timer")]
    public TextMeshProUGUI timerText;

    private void Start() {
        playerWasTaggedText.gameObject.SetActive(false);
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
    /// <para>Show on screen which player was tagged</para>
    /// </summary>
    /// <param name="_playerName">Name of the player that was tagged</param>
    /// <param name="_timeToShow">Time the message will stay on screen</param>
    public void ShowPlayerTaggedText(string _playerName, float _timeToShow) {
        StartCoroutine(ShowPlayerTaggedTextRoutine(_playerName, _timeToShow));
    }

    private IEnumerator ShowPlayerTaggedTextRoutine(string _playerName, float _timeToShow) {
        playerWasTaggedText.gameObject.SetActive(true);
        playerWasTaggedText.text = $"{_playerName} got TAG!";
        yield return new WaitForSecondsRealtime(_timeToShow);
        playerWasTaggedText.gameObject.SetActive(false);
    }
}
