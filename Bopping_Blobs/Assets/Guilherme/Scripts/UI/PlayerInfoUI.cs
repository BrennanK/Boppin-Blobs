using TMPro;
using UnityEngine;

public class PlayerInfoUI : MonoBehaviour {
    public TextMeshProUGUI playerNameText;
    private RectTransform m_rectTransform;

    private void Awake() {
        m_rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// <para>Update player name on the UI.</para>
    /// </summary>
    /// <param name="_playerName">Player's name.</param>
    public void UpdateInfo(string _playerName) {
        playerNameText.text = _playerName;
    }

    /// <summary>
    /// <para>Update player information on the UI</para>
    /// </summary>
    /// <param name="_playerPosition">World Position to put the UI panel.</param>
    public void UpdateInfo(Vector3 _playerPosition) {
        m_rectTransform.position = Camera.main.WorldToScreenPoint(_playerPosition);
    }
}
