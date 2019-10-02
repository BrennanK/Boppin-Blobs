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
    public float attackDistance = 1f;
    public float attackTime = 1f;
    public float attackRadius = 1f;
    private Vector3 m_originalHammerLocalPosition;
    private Vector3 m_originalHammerLocalEulerAngles;

    [Header("Necessary Dependencies")]
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
            return m_playerName;
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

    private float m_timeAsTag;
    public float TimeAsTag {
        get {
            return m_timeAsTag;
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

    private void Awake() {
        m_boppableInterface = GetComponent<IBoppable>();
        m_rigidbodyReference = GetComponent<Rigidbody>();
        m_characterRenderer = GetComponentInChildren<Renderer>();

        m_rigidbodyReference.isKinematic = true;
        m_timeAsTag = 0;

        m_characterRenderer.material.color = blobOriginalColor;
        hammerBopAim.localPosition = new Vector3(0, -0.25f, attackDistance);
        m_originalHammerLocalPosition = hammerTransform.localPosition;
        m_originalHammerLocalEulerAngles = hammerTransform.localEulerAngles;
    }

    private void Update() {
        switch(m_currentTaggingState) {
            case ETaggingBehavior.Running:
                break;
            case ETaggingBehavior.Tagging:
                m_timeAsTag += Time.deltaTime;
                break;
        }

        // TODO I don't like this here
        if (m_boppableInterface.HasAttacked() && m_currentTaggingState != ETaggingBehavior.TaggingAtacking) {
            TriggerAttackTransition();
        }

        m_playerInfo?.UpdateInfo(transform.position);
    }

    /// <summary>
    /// <para>Set this player as TAG</para>
    /// </summary>
    public void SetAsKing() {
        m_boppableInterface.ChangeSpeed(taggingManager.baseSpeed * taggingManager.kingSpeedMultiplier);

        m_isImmune = true;
        m_characterRenderer.material.color = Color.green;
        StartCoroutine(SpeedUpRoutine(1.75f));
        StartCoroutine(TurnOffImmunityRoutine(2.0f));
        
        m_currentTaggingState = ETaggingBehavior.Tagging;
    }

    private IEnumerator TurnOffImmunityRoutine(float _delay) {
        yield return new WaitForSecondsRealtime(_delay);
        m_characterRenderer.material.color = blobOriginalColor;
        m_isImmune = false;
    }

    private IEnumerator SpeedUpRoutine(float _timeToDecay) {
        Debug.Log($"Current Speed: {m_boppableInterface.GetSpeed()}");
        float currentSpeed = m_boppableInterface.GetSpeed();
        float speedBoost = currentSpeed * 2.0f;

        for(float i = 0; i < _timeToDecay; i += Time.deltaTime) {
            float speedToSet = Mathf.Lerp(speedBoost, currentSpeed, (i / _timeToDecay));
            Debug.Log($"Speed To Set: {speedToSet}");
            m_boppableInterface.ChangeSpeed(speedToSet);
            yield return null;
        }
    }

    /// <summary>
    /// <para>Set this player as NOT TAG</para>
    /// </summary>
    public void SetAsNotKing() {
        m_boppableInterface.ChangeSpeed(taggingManager.baseSpeed);
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
        m_boppableInterface.TriggerAttackTransition();
        ETaggingBehavior currentTaggingState = m_currentTaggingState;
        m_currentTaggingState = ETaggingBehavior.TaggingAtacking;
        bool returnToOriginalColor = true;

        m_characterRenderer.material.color = Color.red;

        Collider[] bopCollision = Physics.OverlapSphere(hammerBopAim.position, attackRadius * _attackSizeMultiplier, attackLayer);
        if (bopCollision.Length > 0) {
            for (int i = 0; i < bopCollision.Length; i++) {
                TaggingIdentifier playerHitted = bopCollision[i].transform.gameObject.GetComponent<TaggingIdentifier>();
                if (playerHitted != null && playerHitted.CanBeTagged && m_playerIdentifier != playerHitted.PlayerIdentifier) {

                    if(taggingManager.WhoIsTag == playerHitted.PlayerIdentifier) {
                        playerHitted.SetAsNotKing();
                        taggingManager.PlayerWasTagged(this, true);

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

        yield return new WaitForSecondsRealtime(attackTime);

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
        m_playerInfo.UpdateInfo(transform.position, AmITag());
        m_boppableInterface.UpdateWhoIsTag(_identifier.transform);
    }

    /// <summary>
    /// <para>Returns whether or not this player is TAG</para>
    /// </summary>
    /// <returns>true if player is TAG, false otherwise</returns>
    public bool AmITag() {
        return (PlayerIdentifier == taggingManager.WhoIsTag);
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
            Gizmos.DrawWireSphere(hammerBopAim.transform.position, attackRadius);
        }
    }
}
