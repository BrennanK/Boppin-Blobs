using UnityEngine;

public class TagTextScript : MonoBehaviour {
    public GameObject taggingCanvas;
    private Camera m_mainCamera;

    private void Awake() {
        m_mainCamera = Camera.main;
    }

    private void Update() {
        if(taggingCanvas.activeSelf) {
            taggingCanvas.transform.rotation = Quaternion.LookRotation(new Vector3(m_mainCamera.transform.position.x, m_mainCamera.transform.position.y, m_mainCamera.transform.position.z));
        }
    }
}
