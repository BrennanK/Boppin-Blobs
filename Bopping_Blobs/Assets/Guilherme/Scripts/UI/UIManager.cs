using PowerUp;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
// TODO DOTween

public class UIManager : MonoBehaviour {
    public TextMeshProUGUI centerScreenText;

    [Header("Scoreboard Scripts")]
    public GameObject[] deactivateBeforeGameStart;

    [Header("Scoreboard Scripts")]
    public PlayerScoreUI[] playerScores;

    [Header("Timer")]
    public TextMeshProUGUI timerText;

    [Header("Power Up UI")]
    public Image slot1Image;
    public Image slot2Image;
    public TextMeshProUGUI slot1Text;
    public TextMeshProUGUI slot2Text;
    public Sprite emptyPowerUpSprite;
    public Sprite superSlamSprite;
    public Sprite superSpeedSprite;
    public Sprite backoffSprite;

    [Header("Game Over Screen")]
    public GameObject gameOverPanel;
    public PlayerScoreUI[] finalPlayerScores;
    public GameObject[] deactivateWhenGameIsOver;

    private void OnValidate() {
        centerScreenText.raycastTarget = false;
    }

    private void Start() {
        gameOverPanel.SetActive(false);
    }

    public void DeactivateAllGameObjectsOnBeginGame() {
        foreach(GameObject toDeactivate in deactivateBeforeGameStart) {
            toDeactivate.SetActive(false);
        }
    }

    public void ReactivateAllGameObjectsOnBeginGame() {
        foreach(GameObject toActivate in deactivateBeforeGameStart) {
            toActivate.SetActive(true);
        }
    }

    /// <summary>
    /// <para>Update timer on screen</para>
    /// </summary>
    /// <param name="_time">Time (in seconds) to set</param>
    public void UpdateTimerText(float _time) {
        if (_time == 0) {
            timerText.text = "0.00";
        }

        string time = _time.ToString();
        string[] splittedTime = time.Split('.');

        if(splittedTime.Length > 1) {
            time = $"{splittedTime[0]}.{splittedTime[1].Substring(0, 2)}";
        } else {
            time = $"{splittedTime[0]}.00";
        }

        timerText.text = time;
    }

    /// <summary>
    /// <para>Update the scoreboard on UI</para>
    /// </summary>
    /// <param name="_players">List of players</param>
    public void UpdateScoreboard(TaggingIdentifier[] _players) {
        for(int i = 0; i < _players.Length; i++) {
            playerScores[i].RefreshPlayerScore(_players[i].PlayerName, _players[i].PlayerScore);
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
        centerScreenText.text = $"{_playerName} is KING!";
        yield return new WaitForSecondsRealtime(_timeToShow);
        centerScreenText.gameObject.SetActive(false);
    }

    /// <summary>
    /// <para>Deactivate all UI elements and activates the game over panel</para>
    /// </summary>
    /// <param name="_finalPlayerArray">Final player array as of the end of the game</param>
    public void ShowGameOverPanel(TaggingIdentifier[] _finalPlayerArray) {
        foreach (GameObject deactivate in deactivateWhenGameIsOver) {
            deactivate.SetActive(false);
        }

        gameOverPanel.SetActive(true);

        for(int i = 0; i <_finalPlayerArray.Length; i++) {
            finalPlayerScores[i].RefreshPlayerScore(_finalPlayerArray[i].PlayerName, _finalPlayerArray[i].PlayerScore);
        }
    }

    /// <summary>
    /// <para>Update the Power Ups images on UI</para>
    /// </summary>
    /// <param name="_slot1">Power Up on Slot 1</param>
    /// <param name="_slot2">Power Up on Slot 2</param>
    public void UpdatePowerUpUI(PowerUpHolder _slot1, PowerUpHolder _slot2) {
        if(_slot1.powerUp != null) {
            switch(_slot1.powerUp.powerUp) {
                case EPowerUps.SUPER_SPEED:
                    slot1Image.sprite = superSpeedSprite;
                    break;
                case EPowerUps.SUPER_SLAM:
                    slot1Image.sprite = superSlamSprite;
                    break;
                case EPowerUps.BACK_OFF:
                    slot1Image.sprite = backoffSprite;
                    break;
            }
        } else {
            slot1Image.sprite = emptyPowerUpSprite;
        }

        if(_slot2.powerUp != null) {
            switch (_slot2.powerUp.powerUp) {
                case EPowerUps.SUPER_SPEED:
                    slot2Image.sprite = superSpeedSprite;
                    break;
                case EPowerUps.SUPER_SLAM:
                    slot2Image.sprite = superSlamSprite;
                    break;
                case EPowerUps.BACK_OFF:
                    slot2Image.sprite = backoffSprite;
                    break;
            }            
        } else {
            slot2Image.sprite = emptyPowerUpSprite;
        }
    }
}
