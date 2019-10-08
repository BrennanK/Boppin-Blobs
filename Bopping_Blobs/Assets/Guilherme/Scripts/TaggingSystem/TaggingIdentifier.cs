using System.Collections;
using UnityEngine;

public class TaggingIdentifier : MonoBehaviour {
    public enum ETaggingBehavior {
        Running,
        Tagging,
        TaggingAtacking
    }

    // TODO Maybe all these could be a ScriptableObject
    [Header("Hammer Bop")]
    public LayerMask attackLayer;

    private const float km_attackDistance = 1.5f;
    private const float km_attackTime = .1f;
    private const float km_attackRadius = 1f;
    private Vector3 m_originalHammerLocalPosition;
    private Vector3 m_originalHammerLocalEulerAngles;

    [Header("Necessary Dependencies")]
    public GameObject kingCrown;
    public Transform hammerTransform;
    public Transform hammerBopAim;
    public Color blobOriginalColor;

    [HideInInspector]
    public TaggingManager taggingManager;

    // Rigidbody is used only for knockback, not for movement
    private Rigidbody m_rigidbodyReference;
    private PlayerInfoUI m_playerInfo;
    public PlayerInfoUI PlayerInfo {
        set {
            m_playerInfo = value;
        }
    }

    private string m_playerName;
    public string PlayerName {
        get {
            if(m_isUserPlayer) {
                return m_playerName + " (you)";
            } else {
                return m_playerName;
            }
        }
        set {
            m_playerName = value;

            if(m_playerInfo) {
                m_playerInfo.UpdateInfo(m_playerName);
            }
        }
    }

    // IBoppable should be implemented by PlayerController and AI Controller so we can handle attacking and knockback
    private IBoppable m_boppableInterface;
    private Renderer m_characterRenderer;

    private int m_playerIdentifier;
    public int PlayerIdentifier {
        get {
            return m_playerIdentifier;
        }
        set {
            m_playerIdentifier = value;
        }
    }

    private int m_playersBopped;
    public int PlayersBopped {
        get {
            return m_playersBopped;
        }
    }

    private int m_timesAsKing;
    public float TimesAsKing {
        get {
            return m_timesAsKing;
        }
    }

    private float m_amountOfTimeAsKing;
    public float AmountOfTimeAsKing {
        get {
            return m_amountOfTimeAsKing;
        }
    }

    private bool m_isImmune;
    public bool CanBeTagged {
        get {
            return !m_isImmune;
        }
    }

    private ETaggingBehavior m_currentTaggingState;
    public ETaggingBehavior TaggingState {
        get {
            return m_currentTaggingState;
        }
    }

    public int PlayerScore {
        get {
            return (Mathf.RoundToInt(Mathf.Round(m_amountOfTimeAsKing) + m_playersBopped) * 10);
        }
    }

    private bool m_isUserPlayer = false;
    public bool IsUserPlayer {
        get {
            return m_isUserPlayer;
        }
    }

    public float BaseSpeed {
        get {
            if(AmIKing()) {
                return taggingManager.baseSpeed * taggingManager.kingSpeedMultiplier;
            } else {
                return taggingManager.baseSpeed;
            }
        }
    }

    private float m_externalSpeedBoost = 0;
    public float ExternalSpeedBoost {
        get {
            return m_externalSpeedBoost;
        }
        set {
            m_externalSpeedBoost = value;
            m_boppableInterface.ChangeSpeed(BaseSpeed, m_tempSpeedBoost, m_externalSpeedBoost);
        }
    }

    private float m_tempSpeedBoost;

    private void Awake() {
        if(this.tag == "Player") {
            m_isUserPlayer = true;
        }

        m_boppableInterface = GetComponent<IBoppable>();
        m_rigidbodyReference = GetComponent<Rigidbody>();
        m_characterRenderer = GetComponentInChildren<Renderer>();
        kingCrown.SetActive(false);

        m_rigidbodyReference.isKinematic = true;
        m_amountOfTimeAsKing = 0;
        m_playersBopped = 0;
        m_timesAsKing = 0;

        m_characterRenderer.material.color = blobOriginalColor;
        hammerBopAim.localPosition = new Vector3(0, -0.25f, km_attackDistance);
        m_originalHammerLocalPosition = hammerTransform.localPosition;
        m_originalHammerLocalEulerAngles = hammerTransform.localEulerAngles;
        m_tempSpeedBoost = 0f;
    }

    private void Update() {
        switch(m_currentTaggingState) {
            case ETaggingBehavior.Running:
                break;
            case ETaggingBehavior.Tagging:
                m_amountOfTimeAsKing += Time.deltaTime;
                break;
        }

        // TODO I don't like this here
        if (m_boppableInterface.HasAttacked() && m_currentTaggingState != ETaggingBehavior.TaggingAtacking) {
            TriggerAttackTransition();
        }

        m_playerInfo?.UpdateInfo(transform.position);
    }

    /// <summary>
    /// <para>Set this player as KING</para>
    /// </summary>
    public void SetAsKing() {
        if(GameController.instance) {
            Instantiate(GameController.instance.blobGotKingParticle, transform.position, Quaternion.identity).Play();
            PausedMenuManager._instance?.PlaySFX(GameController.instance.blobGotKingSounds[Random.Range(0, GameController.instance.blobGotKingSounds.Length)]);
        }

        m_boppableInterface.ChangeSpeed(BaseSpeed, 0f, m_externalSpeedBoost);
        kingCrown.SetActive(true);
        m_timesAsKing++;

        m_isImmune = true;
        m_characterRenderer.material.color = Color.green;
        StartCoroutine(SpeedUpRoutine(3f));
        StartCoroutine(TurnOffImmunityRoutine(3f));
        
        m_currentTaggingState = ETaggingBehavior.Tagging;
    }

    private IEnumerator TurnOffImmunityRoutine(float _delay) {
        yield return new WaitForSecondsRealtime(_delay);
        m_characterRenderer.material.color = blobOriginalColor;
        m_isImmune = false;
    }

    private IEnumerator SpeedUpRoutine(float _timeToDecay) {
        float startingBoostSpeed = BaseSpeed;

        for(float i = 0; i < _timeToDecay; i += Time.deltaTime) {
            m_tempSpeedBoost = Mathf.Lerp(startingBoostSpeed, 0, (i / _timeToDecay));
            m_boppableInterface.ChangeSpeed(BaseSpeed, m_tempSpeedBoost, m_externalSpeedBoost);
            yield return null;
        }

        m_tempSpeedBoost = 0f;
        m_boppableInterface.ChangeSpeed(BaseSpeed, m_tempSpeedBoost, m_externalSpeedBoost);
    }

    /// <summary>
    /// <para>Set this player as NOT TAG</para>
    /// </summary>
    public void SetAsNotKing() {
        kingCrown.SetActive(false);
        m_boppableInterface.ChangeSpeed(BaseSpeed, 0f, m_externalSpeedBoost);
        m_currentTaggingState = ETaggingBehavior.Running;
    }

    #region ATTACKING
    public bool ForceAttackWithMultiplier(float _attackSizeMultiplier) {
        if(m_currentTaggingState != ETaggingBehavior.TaggingAtacking) {
            TriggerAttackTransition(_attackSizeMultiplier);
            return true;
        }

        return false;
    }

    private void TriggerAttackTransition(float _attackSizeMultiplier = 1.0f) {
        if(GameController.instance) {
            PausedMenuManager._instance?.PlaySFX(GameController.instance.blobAttackSounds[Random.Range(0, GameController.instance.blobAttackSounds.Length)]);
        }

        if(GameController.instance) {
            Instantiate(GameController.instance.blobAttackParticle, hammerBopAim.transform.position, Quaternion.identity).Play();
        }

        m_boppableInterface.TriggerAttackTransition();
        ETaggingBehavior currentTaggingState = m_currentTaggingState;
        m_currentTaggingState = ETaggingBehavior.TaggingAtacking;
        bool returnToOriginalColor = true;

        m_characterRenderer.material.color = Color.red;

        Collider[] bopCollision = Physics.OverlapSphere(hammerBopAim.position, km_attackRadius * _attackSizeMultiplier, attackLayer);
        if (bopCollision.Length > 0) {
            for (int i = 0; i < bopCollision.Length; i++) {
                TaggingIdentifier playerHitted = bopCollision[i].transform.gameObject.GetComponent<TaggingIdentifier>();
                if (playerHitted != null && playerHitted.CanBeTagged && m_playerIdentifier != playerHitted.PlayerIdentifier) {
                    // We hit someone, so we bopped them!

                    if (GameController.instance) {
                        PausedMenuManager._instance?.PlaySFX(GameController.instance.blobHitSounds[Random.Range(0, GameController.instance.blobHitSounds.Length)]);
                    }

                    m_playersBopped++;

                    if(taggingManager.WhoIsKing == playerHitted.PlayerIdentifier) {
                        taggingManager.PlayerWasTagged(this, true);
                        playerHitted.SetAsNotKing();

                        // Updating tagging state because we are tag now.
                        currentTaggingState = m_currentTaggingState;
                        returnToOriginalColor = false;
                    } else {
                        // Just Knockback the Player
                        // TODO Delay here is a magic number
                        // TODO knockback force is also a magic number
                        playerHitted.KnockbackPlayer(Color.magenta, (playerHitted.transform.position - transform.position).normalized * taggingManager.knockbackForce * 3f, 0.5f);
                    }

                    break;
                }
            }
        }

        StartCoroutine(AttackAnimationRoutine(currentTaggingState, returnToOriginalColor));
    }

    private IEnumerator AttackAnimationRoutine(ETaggingBehavior _nextTaggingState, bool _returnToOriginalColor = true) {
        hammerTransform.localPosition = new Vector3(hammerBopAim.localPosition.x, hammerBopAim.localPosition.y + 0.25f, hammerBopAim.localPosition.z - 1f);
        hammerTransform.localEulerAngles = new Vector3(90, 0, 0);

        yield return new WaitForSecondsRealtime(km_attackTime);

        if(_returnToOriginalColor) {
            m_characterRenderer.material.color = blobOriginalColor;
        }

        hammerTransform.localPosition = m_originalHammerLocalPosition;
        hammerTransform.localEulerAngles = m_originalHammerLocalEulerAngles;
        m_boppableInterface.TriggerEndAttackTransition();
        m_currentTaggingState = _nextTaggingState;
    }
    #endregion

    #region TAGGING
    /// <summary>
    /// <para>Knockbacks the Player</para>
    /// </summary>
    /// <param name="_knockbackColor">Feedback Color for knockbacked player</param>
    /// <param name="_knockbackIntensity">Direction and intensity player will be knocked back</param>
    public void KnockbackPlayer(Color _knockbackColor, Vector3 _knockbackIntensity, float _delayTime) {
        m_characterRenderer.material.color = _knockbackColor;
        m_boppableInterface.DeactivateController();

        m_rigidbodyReference.isKinematic = false;
        m_rigidbodyReference.velocity = _knockbackIntensity;
        StartCoroutine(KnockbackDelay(_delayTime));
    }

    private IEnumerator KnockbackDelay(float _delayTime) {
        yield return new WaitForSeconds(_delayTime / 2.0f);
        m_rigidbodyReference.velocity = Vector3.zero;
        yield return new WaitForSeconds(_delayTime / 2.0f);
        m_characterRenderer.material.color = blobOriginalColor;
        m_rigidbodyReference.isKinematic = true;
        m_boppableInterface.ReactivateController();
    }

    /// <summary>
    /// Update who the Character Controller identifies as Who Is Tag
    /// </summary>
    /// <param name="_identifier">Player who is currently tag</param>
    public void UpdateWhoIsTag(TaggingIdentifier _identifier) {
        m_boppableInterface.UpdateWhoIsTag(_identifier.transform);
    }

    /// <summary>
    /// <para>Returns whether or not this player is TAG</para>
    /// </summary>
    /// <returns>true if player is TAG, false otherwise</returns>
    public bool AmIKing() {
        return (PlayerIdentifier == taggingManager.WhoIsKing);
    }
    #endregion

    #region Helpers
    public void DeactivatePlayer() {
        m_boppableInterface.DeactivateController();
    }

    public void ActivatePlayer() {
        m_boppableInterface.ReactivateController();
    }
    #endregion

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;

        if(hammerBopAim.gameObject.activeSelf) {
            Gizmos.DrawWireSphere(hammerBopAim.transform.position, km_attackRadius);
        }
    }
}
