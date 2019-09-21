using TMPro;
using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour {
    public TextMeshProUGUI playerWasTaggedText;

    private void Start() {
        playerWasTaggedText.gameObject.SetActive(false);
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
