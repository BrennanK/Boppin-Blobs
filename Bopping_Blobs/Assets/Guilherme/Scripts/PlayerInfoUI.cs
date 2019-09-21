using UnityEngine;

public class PlayerInfoUI : MonoBehaviour {
    public GameObject tagTextObject;
    private RectTransform m_rectTransform;

    private void Start() {
        m_rectTransform = GetComponent<RectTransform>();
    }

    /// <summary>
    /// <para>Update player information on the UI</para>
    /// </summary>
    /// <param name="_playerPosition">World Position to put the UI panel.</param>
    public void UpdateInfo(Vector3 _playerPosition) {
        m_rectTransform.position = Camera.main.WorldToScreenPoint(_playerPosition);
    }

    /// <summary>
    /// <para>Update player information on the UI and activate or deactivate TAG panel</para>
    /// </summary>
    /// <param name="_playerPosition">World Position to put in the UI panel.</param>
    /// <param name="_isTag">Whether is tag or not.</param>
    public void UpdateInfo(Vector3 _playerPosition, bool _isTag) {
        m_rectTransform.position = Camera.main.WorldToScreenPoint(_playerPosition);
        tagTextObject.SetActive(_isTag);
    }
}
