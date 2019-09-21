using System.Collections;
using System.Collections.Generic;
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

    [Header("Necessary Dependencies")]
    public Transform hammerTransform;
    public Transform hammerBopAim;
    public Color blobOriginalColor;

    [HideInInspector]
    public TaggingManager taggingManager;
    [Header("Tagging Configuration")]
    public float knockbackDelay = 1.0f;

    // Rigidbody is used only for knockback, not for movement
    private Rigidbody m_rigidbodyReference;
    private PlayerInfoUI m_playerInfo;
    public PlayerInfoUI PlayerInfo {
        set {
            m_playerInfo = value;
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

    private ETaggingBehavior m_currentTaggingState;
    public ETaggingBehavior TaggingState {
        get {
            return m_currentTaggingState;
        }
    }

    private void Start() {
        m_boppableInterface = GetComponent<IBoppable>();
        m_rigidbodyReference = GetComponent<Rigidbody>();
        m_characterRenderer = GetComponent<Renderer>();
        m_rigidbodyReference.isKinematic = true;
        m_timeAsTag = 0;

        m_characterRenderer.material.color = blobOriginalColor;
        hammerBopAim.localPosition = new Vector3(0, -0.5f, attackDistance);
    }

    private void Update() {
        switch(m_currentTaggingState) {
            case ETaggingBehavior.Running:
                break;
            case ETaggingBehavior.Tagging:
                m_timeAsTag += Time.deltaTime;
                break;
        }

        if (m_boppableInterface.HasAttacked() && m_currentTaggingState != ETaggingBehavior.TaggingAtacking) {
            TriggerAttackTransition();
        }

        m_playerInfo?.UpdateInfo(transform.position);
    }

    public void SetAsTagging() {
        m_boppableInterface.ChangeSpeed(taggingManager.isTagSpeed);
        m_currentTaggingState = ETaggingBehavior.Tagging;
    }

    public void SetAsNotTag() {
        m_boppableInterface.ChangeSpeed(taggingManager.isNotTagSpeed);
        m_currentTaggingState = ETaggingBehavior.Running;
    }

    #region ATTACKING
    private void TriggerAttackTransition() {
        m_boppableInterface.TriggerAttackTransition();
        ETaggingBehavior previousState = m_currentTaggingState;
        m_currentTaggingState = ETaggingBehavior.TaggingAtacking;

        m_characterRenderer.material.color = Color.red;

        Collider[] bopCollision = Physics.OverlapSphere(hammerBopAim.position, attackRadius, attackLayer);
        if (bopCollision.Length > 0) {
            for (int i = 0; i < bopCollision.Length; i++) {
                TaggingIdentifier playerHitted = bopCollision[i].transform.gameObject.GetComponent<TaggingIdentifier>();
                if (playerHitted != null && m_playerIdentifier != playerHitted.PlayerIdentifier) {

                    Vector3 knockbackVector = playerHitted.transform.position - hammerBopAim.transform.position;
                    playerHitted.KnockbackPlayer(Color.magenta, knockbackVector.normalized);

                    if(taggingManager.WhoIsTag == playerHitted.PlayerIdentifier) {
                        playerHitted.SetAsNotTag();
                        taggingManager.PlayerWasTagged(this, true);
                    }

                    break;
                }
            }
        }

        StartCoroutine(AttackAnimationRoutine(previousState));
    }

    private IEnumerator AttackAnimationRoutine(ETaggingBehavior _previousTaggingState) {
        Vector3 originalTransformLocalPosition = hammerTransform.localPosition;
        Vector3 originalLocalEulerAngles = hammerTransform.localEulerAngles;
        hammerTransform.localPosition = new Vector3(hammerBopAim.localPosition.x, hammerBopAim.localPosition.y + 0.25f, hammerBopAim.localPosition.z - 1f);
        hammerTransform.localEulerAngles = new Vector3(90, 0, 0);

        for(float waitingTime = 0f; waitingTime < attackTime; waitingTime += Time.deltaTime) {
            yield return null;
        }

        m_characterRenderer.material.color = blobOriginalColor;
        hammerTransform.localPosition = originalTransformLocalPosition;
        hammerTransform.localEulerAngles = originalLocalEulerAngles;
        m_boppableInterface.TriggerEndAttackTransition();
        m_currentTaggingState = _previousTaggingState;
    }
    #endregion

    #region TAGGING
    /// <summary>
    /// Knockbacks the Player
    /// </summary>
    /// <param name="_knockbackColor">Feedback Color for knockbacked player</param>
    /// <param name="_knockbackDirection">vector indicating the direction where the player will be knockbacked</param>
    public void KnockbackPlayer(Color _knockbackColor, Vector3 _knockbackDirection) {
        m_characterRenderer.material.color = _knockbackColor;
        m_boppableInterface.DeactivateController();

        m_rigidbodyReference.isKinematic = false;
        m_rigidbodyReference.velocity = _knockbackDirection;
        StartCoroutine(KnockbackDelay());
    }

    private IEnumerator KnockbackDelay() {
        yield return new WaitForSeconds(knockbackDelay);
        GetComponent<Renderer>().material.color = blobOriginalColor;
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

    public bool AmITag() {
        return (PlayerIdentifier == taggingManager.WhoIsTag);
    }
    #endregion

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;

        if(hammerBopAim.gameObject.activeSelf) {
            Gizmos.DrawWireSphere(hammerBopAim.transform.position, attackRadius);
        }
    }
}
