using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoUI : MonoBehaviour {
    public GameObject tagTextObject;
    private RectTransform m_rectTransform;

    private void Start() {
        m_rectTransform = GetComponent<RectTransform>();
    }

    public void UpdateInfo(Vector3 _playerPosition) {
        m_rectTransform.position = Camera.main.WorldToScreenPoint(_playerPosition);
    }

    public void UpdateInfo(Vector3 _playerPosition, bool _isTag) {
        m_rectTransform.position = Camera.main.WorldToScreenPoint(_playerPosition);
        tagTextObject.SetActive(_isTag);
    }
}
