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

    [Header("Knockback")]
    public float knockbackForceMultiplier = 25f;

    [Header("Necessary Dependencies")]
    public Transform hammerTransform;
    public Transform hammerBopAim;
    public Color blobOriginalColor;
    public GameObject isTagCanvas;

    [HideInInspector]
    public TaggingManager taggingManager;
    [Header("Tagging Configuration")]
    public float knockbackDelay = 1.0f;

    // Rigidbody is used only for knockback, not for movement
    private Rigidbody m_rigidbodyReference;

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
    protected float m_attackWaitTime;

    /*
     * Things that Lin had that I don't (future reference)
     *      TagCanvas
     */

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
                // Debug.Log($"{gameObject.name} is tagging!!");
                m_timeAsTag += Time.deltaTime;
                break;
        }

        if (m_boppableInterface.HasAttacked() && m_currentTaggingState != ETaggingBehavior.TaggingAtacking) {
            TriggerAttackTransition();
        }

        // Drawing Debug Rays
        Debug.DrawRay(transform.position, transform.forward * attackDistance, Color.red);
        Debug.DrawRay(hammerBopAim.position, Vector3.up, Color.blue);
    }

    public void SetAsTagging() {
        m_boppableInterface.TriggerIsTagTransition();
        isTagCanvas.SetActive(true);
        m_currentTaggingState = ETaggingBehavior.Tagging;
    }

    public void SetAsNotTag() {
        // TODO IBoppable should have a behavior "NotTaggingTransition"
        isTagCanvas.SetActive(false);
        m_currentTaggingState = ETaggingBehavior.Running;
    }

    #region ATTACKING
    private void TriggerAttackTransition() {
        bool hitAnotherPlayer = false;
        m_boppableInterface.TriggerAttackTransition();
        ETaggingBehavior previousState = m_currentTaggingState;
        m_currentTaggingState = ETaggingBehavior.TaggingAtacking;
        m_characterRenderer.material.color = Color.red;
        m_attackWaitTime = attackTime;

        Collider[] bopCollision = Physics.OverlapSphere(hammerBopAim.position, attackRadius, attackLayer);
        if (bopCollision.Length > 0) {
            for (int i = 0; i < bopCollision.Length; i++) {
                TaggingIdentifier playerHitted = bopCollision[i].transform.gameObject.GetComponent<TaggingIdentifier>();
                if (playerHitted != null && m_playerIdentifier != playerHitted.PlayerIdentifier) {
                    hitAnotherPlayer = true;

                    Vector3 knockbackVector = playerHitted.transform.position - hammerBopAim.transform.position;
                    playerHitted.KnockbackPlayer(Color.magenta, knockbackVector.normalized);

                    if(taggingManager.WhoIsTag == playerHitted.PlayerIdentifier) {
                        playerHitted.SetAsNotTag();
                        Tag();
                    }

                    /*
                    if(taggingManager.WhoIsTag == this.m_playerIdentifier) {
                        // TODO maybe this could go directly to the hitted character
                        // If we are tag, then we tag the player
                        playerHitted.Tag();
                        // and we are not tag anymore
                        SetAsNotTag();
                    } 
                    */

                    break;
                }
            }
        }

        StartCoroutine(AttackAnimationRoutine(hitAnotherPlayer, previousState));
    }

    private IEnumerator AttackAnimationRoutine(bool _hitAnotherPlayer, ETaggingBehavior _previousTaggingState) {
        Vector3 originalTransformLocalPosition = hammerTransform.localPosition;
        Vector3 originalLocalEulerAngles = hammerTransform.localEulerAngles;
        hammerTransform.localPosition = new Vector3(hammerBopAim.localPosition.x, hammerBopAim.localPosition.y + 0.25f, hammerBopAim.localPosition.z - 1f);
        hammerTransform.localEulerAngles = new Vector3(90, 0, 0);

        while (m_attackWaitTime > 0) {
            m_attackWaitTime -= Time.deltaTime;
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
    public void Tag() {
        Debug.Log($"I ({gameObject.name}) was tagged =D");
        taggingManager.PlayerWasTagged(this);
    }

    /// <summary>
    /// Knockbacks the Player
    /// </summary>
    /// <param name="_knockbackColor">Feedback Color for knockbacked player</param>
    /// <param name="_knockbackDirection">Normalized vector indicating the direction where the player will be knockbacked</param>
    public void KnockbackPlayer(Color _knockbackColor, Vector3 _knockbackDirection) {
        m_characterRenderer.material.color = _knockbackColor;
        m_boppableInterface.DeactivateController();

        m_rigidbodyReference.isKinematic = false;
        m_rigidbodyReference.velocity = _knockbackDirection * knockbackForceMultiplier;
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
