using TMPro;
using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour {
    public TextMeshProUGUI playerWasTaggedText;

    [Header("Scoreboard Scripts")]
    public PlayerScoreUI[] playerScores;

    private void Start() {
        playerWasTaggedText.gameObject.SetActive(false);
    }

    public void UpdateScoreboard(TaggingIdentifier[] _players) {
        for(int i = 0; i < _players.Length; i++) {
            playerScores[i].RefreshPlayerScore(i, _players[i].PlayerName, _players[i].TimeAsTag);
        }
    }

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
