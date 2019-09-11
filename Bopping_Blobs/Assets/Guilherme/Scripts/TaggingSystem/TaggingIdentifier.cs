using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaggingIdentifier : MonoBehaviour {
    public enum ETaggingBehavior {
        Running,
        Tagging,
        TaggingAtacking
    }

    [Header("Hammer Bop")]
    public LayerMask attackLayer;
    public float attackDistance = 1f;
    public float attackTime = 1f;
    public float attackRadius = 1f;
    public Transform hammerTransform;
    public Transform hammerBopAim;

    [Header("Tagging Configuration")]
    public TaggingManager taggingManager;
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

        hammerBopAim.localPosition = new Vector3(0, -0.5f, attackDistance);
    }

    private void Update() {
        switch(m_currentTaggingState) {
            case ETaggingBehavior.Tagging:
                // Debug.Log($"{gameObject.name} is tagging!!");
                m_timeAsTag += Time.deltaTime;
                ProcessTaggingBehavior();
                break;
        }

        // Drawing Debug Rays
        Debug.DrawRay(transform.position, transform.forward * attackDistance, Color.red);
        Debug.DrawRay(hammerBopAim.position, Vector3.up, Color.blue);
    }

    private void ProcessTaggingBehavior() {
        if(m_boppableInterface.HasAttacked()) {
            TriggerAttackTransition();
        }
    }

    // TODO This is Temporary
    public void SetAsTag() {
        // TODO also activate hammer
        m_currentTaggingState = ETaggingBehavior.Tagging;
    }

    #region ATTACKING
    private void TriggerAttackTransition() {
        m_boppableInterface.TriggerAttackTransition();
        m_currentTaggingState = ETaggingBehavior.TaggingAtacking;

        m_characterRenderer.material.color = Color.red;
        m_attackWaitTime = attackTime;

        Collider[] bopCollision = Physics.OverlapSphere(hammerBopAim.position, attackRadius, attackLayer);
        if (bopCollision.Length > 0) {
            for (int i = 0; i < bopCollision.Length; i++) {
                TaggingIdentifier playerHitted = bopCollision[i].transform.gameObject.GetComponent<TaggingIdentifier>();
                if (playerHitted != null && playerHitted.PlayerIdentifier != playerHitted.PlayerIdentifier) {
                    // TODO the knockback force should never be towards player'
                    // TODO at this point we know another character was hit, transfer TAG to it
                    playerHitted.CharacterWasTagged(Color.magenta, new Vector3(Random.Range(.5f, 1f), 0f, Random.Range(.5f, 1f)).normalized);
                    break;
                }
            }
        }

        StartCoroutine(AttackAnimationRoutine());
    }

    private IEnumerator AttackAnimationRoutine() {
        Vector3 originalTransformLocalPosition = hammerTransform.localPosition;
        Vector3 originalLocalEulerAngles = hammerTransform.localEulerAngles;
        hammerTransform.localPosition = new Vector3(hammerBopAim.localPosition.x, hammerBopAim.localPosition.y + 0.25f, hammerBopAim.localPosition.z - 1f);
        hammerTransform.localEulerAngles = new Vector3(90, 0, 0);

        while (m_attackWaitTime > 0) {
            m_attackWaitTime -= Time.deltaTime;
            yield return null;
        }

        m_characterRenderer.material.color = Color.cyan;
        hammerTransform.localPosition = originalTransformLocalPosition;
        hammerTransform.localEulerAngles = originalLocalEulerAngles;

        m_boppableInterface.TriggerEndAttackTransition();

        // TODO Shouldn't return to tagging if hit someone
        m_currentTaggingState = ETaggingBehavior.Tagging;
    }
    #endregion

    #region TAGGING
    public void CharacterWasTagged(Color _colorToRender, Vector3 _knockbackDirection) {
        // TODO this character should be tagging now
        Debug.Log($"I ({gameObject.name}) was tagged :(");

        m_characterRenderer.material.color = _colorToRender;
        m_boppableInterface.DeactivateController();

        m_rigidbodyReference.isKinematic = false;
        m_rigidbodyReference.velocity = _knockbackDirection * 25f;
        StartCoroutine(KnockbackDelay());
    }

    private IEnumerator KnockbackDelay() {
        yield return new WaitForSeconds(knockbackDelay);
        GetComponent<Renderer>().material.color = Color.cyan;
        m_rigidbodyReference.isKinematic = true;
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
