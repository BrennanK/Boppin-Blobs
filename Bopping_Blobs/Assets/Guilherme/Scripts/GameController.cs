using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
    public static GameController instance;

    [Header("General Game Configurations")]
    public float gameStartDelayTime = 3f;
    public float gameTime = 60f;

    [Header("Countdown Sounds")]
    public AudioClip readySound;
    public AudioClip goGoGoSound;
    public AudioClip threeSound;
    public AudioClip twoSound;
    public AudioClip oneSound;

    [Header("Game Sound Effects")]
    public AudioClip[] blobGotKingSounds;
    public AudioClip[] blobAttackSounds;
    public AudioClip[] blobHitSounds;
    public AudioClip[] powerUpCollectedSound;
    public AudioClip[] superSpeedSounds;
    public AudioClip[] backOffSounds;
    public AudioClip[] superSlamSounds;

    [Header("Particle Effects")]
    public ParticleSystem blobGotKingParticle;
    public ParticleSystem blobAttackParticle;
    public ParticleSystem blobGotPowerUpParticle;
    public ParticleSystem superSpeedParticle;
    public ParticleSystem superSlamParticle;
    public ParticleSystem backOffParticle;

    [Header("Decals")]
    public GameObject backOffDecal;
    public GameObject superSlamDecal;

    private bool m_isGameRunning = false;
    public bool IsGameRunning {
        get {
            return m_isGameRunning;
        }
    }

    private float m_currentGameTime;
    private UIManager m_UIManager;
    private TaggingManager m_taggingManager;
    private PowerUp.PowerUpBox[] m_allPowerBoxes;
    public PowerUp.PowerUpBox[] PowerupBoxes {
        get {
            return m_allPowerBoxes;
        }
    }

    private bool m_isCountdownRunning;

    private void Awake() {
        instance = this;
        m_allPowerBoxes = FindObjectsOfType<PowerUp.PowerUpBox>();
        m_UIManager = FindObjectOfType<UIManager>();
        m_taggingManager = FindObjectOfType<TaggingManager>();
        m_taggingManager.InitializeTaggingManager();
        m_isCountdownRunning = false;
    }

    private void Start() {
        m_currentGameTime = gameTime;
        m_taggingManager.FreezeAllPlayers();
        StartCoroutine(StartGameRoutine());
    }

    private IEnumerator StartGameRoutine() {
        m_UIManager.UpdateTimerText(gameTime);
        m_UIManager.DeactivateAllGameObjectsOnBeginGame();
        m_UIManager.ShowCenterText("Ready?");
        PausedMenuManager._instance.PlaySFX(readySound);
        yield return new WaitForSecondsRealtime(gameStartDelayTime);
        PausedMenuManager._instance.PlaySFX(goGoGoSound);
        m_UIManager.ReactivateAllGameObjectsOnBeginGame();
        m_taggingManager.EnableAllPlayers();
        m_taggingManager.StartTagging();
        m_isGameRunning = true;
        InvokeRepeating("UpdateScoreboard", 0f, .25f);
    }

    private void Update() {
        if(!m_isGameRunning) {
            return;
        }

        if(!m_isCountdownRunning) {
            m_currentGameTime -= Time.deltaTime;

            if (m_currentGameTime <= 3.0f) {
                m_UIManager.DeactivateTimerObject();
                m_isCountdownRunning = true;
                StartCountdown();
            }
        }

        m_UIManager.UpdateTimerText(m_currentGameTime);
    }

    private void StartCountdown() {
        StartCoroutine(StartCountdownRoutine());
    }

    private IEnumerator StartCountdownRoutine() {
        PausedMenuManager._instance.PlaySFX(threeSound);
        m_UIManager.ShowCenterText("3");
        yield return new WaitForSecondsRealtime(1.0f);
        PausedMenuManager._instance.PlaySFX(twoSound);
        m_UIManager.ShowCenterText("2");
        yield return new WaitForSecondsRealtime(1.0f);
        PausedMenuManager._instance.PlaySFX(oneSound);
        m_UIManager.ShowCenterText("1");
        yield return new WaitForSecondsRealtime(1.0f);

        EndGame();
    }

    private void EndGame() {
        TaggingIdentifier[] finalPlayersArray = m_taggingManager.Players.ToArray();
        m_currentGameTime = 0f;
        m_isGameRunning = false;
        m_taggingManager.FreezeAllPlayers();
        m_UIManager.ShowGameOverPanel(finalPlayersArray);
        PausedMenuManager._instance.DeactivateOptionsButton();

        int finalPlayerPosition = 7;
        TaggingIdentifier playerIdentifier;
        for(int i = 0; i < finalPlayersArray.Length; i++) {
            if(finalPlayersArray[i].IsUserPlayer) {
                playerIdentifier = finalPlayersArray[i];
                finalPlayerPosition = i + 1;
            }
        }

        // TEMP TODO the saved_amount_of_coins string should be on save load manager
        Debug.Log($"Saving Money...");
        int amountOfMoneyPlayerEarned = GetMoneyFromPosition(finalPlayerPosition);
        int currentAmountOfMoneyPlayerHas = PlayerPrefs.GetInt("SAVED_AMOUNT_OF_COINS");
        PlayerPrefs.SetInt("SAVED_AMOUNT_OF_COINS", currentAmountOfMoneyPlayerHas + amountOfMoneyPlayerEarned);

        /*
         * TODO
         * Save Data Stuff
         * Games Played => +1
         * Times In Each Position => add finalPlayerPosition
         * Time Played += gameTime
         * Times King in Round => playerIdentifier.TimesAsKing;
         * Times King => Add playerIdentifier.TimeAsKing
         * TimesAttacked => playerIdentifier.PlayersBopped;
         * Money Earned => amountOfMoneyPlayerEarned
         */
    }


    /// <summary>
    /// Return the amount of money player earned for ending on that position
    /// </summary>
    /// <param name="_finalPosition">A Position between 1 and 7 (inclusive)</param>
    /// <returns>Amount of money that position was rewarded</returns>
    private int GetMoneyFromPosition(int _finalPosition) {
        switch(_finalPosition) {
            case 1:
                return 500;
            case 2:
                return 250;
            case 3:
                return 100;
            default:
                // Thanks for trying
                return 50;
        }
    }

    private void UpdateScoreboard() {
        m_UIManager.UpdateScoreboard(m_taggingManager.Players.ToArray());
    }

    public void GoToMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }

    private void OnApplicationFocus(bool _focusStatus) {
        // we just want to pause in case we lose focus, we don't want to unpause
        if(!_focusStatus) {
            PausedMenuManager._instance?.EnablePausedMenu(true);
        }
    }

    private void OnApplicationPause(bool _pauseStatus) {
        if(_pauseStatus) {
            PausedMenuManager._instance?.EnablePausedMenu(true);
        }
    }
}
