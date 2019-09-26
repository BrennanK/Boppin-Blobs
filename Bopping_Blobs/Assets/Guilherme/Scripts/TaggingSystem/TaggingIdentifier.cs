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
    public void SetAsTagging() {
        m_boppableInterface.ChangeSpeed(taggingManager.isTagSpeed);
        m_currentTaggingState = ETaggingBehavior.Tagging;
    }

    /// <summary>
    /// <para>Set this player as NOT TAG</para>
    /// </summary>
    public void SetAsNotTag() {
        m_boppableInterface.ChangeSpeed(taggingManager.isNotTagSpeed);
        m_currentTaggingState = ETaggingBehavior.Running;
    }

    #region ATTACKING
    private void TriggerAttackTransition() {
        m_boppableInterface.TriggerAttackTransition();
        ETaggingBehavior currentTaggingState = m_currentTaggingState;
        m_currentTaggingState = ETaggingBehavior.TaggingAtacking;

        m_characterRenderer.material.color = Color.red;

        Collider[] bopCollision = Physics.OverlapSphere(hammerBopAim.position, attackRadius, attackLayer);
        if (bopCollision.Length > 0) {
            for (int i = 0; i < bopCollision.Length; i++) {
                TaggingIdentifier playerHitted = bopCollision[i].transform.gameObject.GetComponent<TaggingIdentifier>();
                if (playerHitted != null && m_playerIdentifier != playerHitted.PlayerIdentifier) {

                    Vector3 knockbackVector = playerHitted.transform.position - hammerBopAim.transform.position;
                    playerHitted.KnockbackPlayer(Color.magenta, knockbackVector.normalized * taggingManager.knockbackForce);

                    if(taggingManager.WhoIsTag == playerHitted.PlayerIdentifier) {
                        playerHitted.SetAsNotTag();
                        taggingManager.PlayerWasTagged(this, true);

                        // Updating tagging state because we are tag now.
                        currentTaggingState = m_currentTaggingState;
                    }

                    break;
                }
            }
        }

        StartCoroutine(AttackAnimationRoutine(currentTaggingState));
    }

    private IEnumerator AttackAnimationRoutine(ETaggingBehavior _nextTaggingState) {
        hammerTransform.localPosition = new Vector3(hammerBopAim.localPosition.x, hammerBopAim.localPosition.y + 0.25f, hammerBopAim.localPosition.z - 1f);
        hammerTransform.localEulerAngles = new Vector3(90, 0, 0);

        yield return new WaitForSecondsRealtime(attackTime);

        m_characterRenderer.material.color = blobOriginalColor;
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
    public void KnockbackPlayer(Color _knockbackColor, Vector3 _knockbackIntensity) {
        m_characterRenderer.material.color = _knockbackColor;
        m_boppableInterface.DeactivateController();

        m_rigidbodyReference.isKinematic = false;
        m_rigidbodyReference.velocity = _knockbackIntensity;
        StartCoroutine(KnockbackDelay());
    }

    private IEnumerator KnockbackDelay() {
        yield return new WaitForSeconds(taggingManager.knockbackDelayTime);
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
