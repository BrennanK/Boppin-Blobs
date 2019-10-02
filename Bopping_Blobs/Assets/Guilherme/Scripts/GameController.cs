using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
    [Header("General Game Configurations")]
    public float gameStartDelayTime = 3f;
    public float gameTime = 60f;

    private bool m_isGameRunning = false;
    public bool IsGameRunning {
        get {
            return m_isGameRunning;
        }
    }

    private float m_currentGameTime;
    private UIManager m_UIManager;
    private TaggingManager m_taggingManager;

    private void Awake() {
        m_UIManager = FindObjectOfType<UIManager>();
        m_taggingManager = FindObjectOfType<TaggingManager>();
    }

    private void Start() {
        m_currentGameTime = gameTime;
        InvokeRepeating("UpdateScoreboard", 0f, .25f);
        m_taggingManager.FreezeAllPlayers();
        StartCoroutine(StartGameRoutine());
    }

    private IEnumerator StartGameRoutine() {
        m_UIManager.UpdateTimerText(gameTime);
        m_UIManager.ShowCenterText("Ready?");
        yield return new WaitForSecondsRealtime(gameStartDelayTime);
        m_taggingManager.EnableAllPlayers();
        m_taggingManager.StartTagging();
        m_isGameRunning = true;
    }

    private void Update() {
        if(!m_isGameRunning) {
            return;
        }

        m_currentGameTime -= Time.deltaTime;

        if(m_currentGameTime <= 0f) {
            EndGame();
        }

        m_UIManager.UpdateTimerText(m_currentGameTime);
    }

    private void EndGame() {
        m_currentGameTime = 0f;
        m_isGameRunning = false;
        m_taggingManager.FreezeAllPlayers();
        m_UIManager.ShowGameOverPanel(m_taggingManager.Players.ToArray());
        // TODO Handle Score to Money here
    }

    private void UpdateScoreboard() {
        m_UIManager.UpdateScoreboard(m_taggingManager.Players.ToArray());
    }

    public void GoToMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }
}
