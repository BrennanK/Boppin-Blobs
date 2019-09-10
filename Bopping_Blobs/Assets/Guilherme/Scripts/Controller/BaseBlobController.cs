using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseBlobController : MonoBehaviour {
    // TODO separate the hammer/tag/it logic from the base controller
    public enum ECharacterState {
        Moving,
        Attacking
    }

    [Header("Ground Movement")]
    public float characterSpeed;
    [Range(0, 1)]
    public float groundDamping;
    public float fallingGravityMultiplier = 3f;

    [Header("Hammer Bop")]
    public LayerMask attackLayer;
    public float attackDistance = 1f;
    public float attackTime = 1f;
    public float attackRadius = 1f;
    public Transform hammerTransform;
    public Transform hammerBopAim;

    [Header("Feedback Colors (temp")]
    public Color regularBlobColor;
    public Color attackingBlobColor;
    public Color taggedColor;

    // Cached References
    protected CharacterController m_characterControllerReference;
    protected Vector3 m_movementVector;
    protected Renderer m_characterRenderer;

    // Tracking Current State
    protected ECharacterState m_currentState;
    protected float m_waitingTime;

    private void Start() {
        InitializeController();
        hammerBopAim.localPosition = new Vector3(0, -0.5f, attackDistance);
    }

    private void Update() {
        switch (m_currentState) {
            case ECharacterState.Moving:
                HandleMovement();
                break;
            case ECharacterState.Attacking:
                HandleAttack();
                break;
        }

        // TODO maybe this could be a class variable
        float dampingMultiplier = 1f;
        if (m_currentState == ECharacterState.Attacking) {
            dampingMultiplier *= 2;
        }

        m_movementVector.x = Mathf.Lerp(m_characterControllerReference.velocity.x, m_movementVector.x, groundDamping * dampingMultiplier);
        m_movementVector.y = 0;
        m_movementVector.z = Mathf.Lerp(m_characterControllerReference.velocity.z, m_movementVector.z, groundDamping * dampingMultiplier);

        // Drawing Debug Rays
        Debug.DrawRay(transform.position, transform.forward * attackDistance, Color.red);
        Debug.DrawRay(transform.position, new Vector3(m_movementVector.x, 0f, m_movementVector.z).normalized * attackDistance, Color.green);
        Debug.DrawRay(hammerBopAim.position, Vector3.up, Color.blue);


        // Updating the character position
        m_characterControllerReference.SimpleMove(m_movementVector);
    }

    protected virtual void InitializeController() {
        m_characterControllerReference = GetComponent<CharacterController>();
        m_characterRenderer = GetComponent<Renderer>();
        m_currentState = ECharacterState.Moving;
    }

    protected abstract void HandleMovement();

    protected void TriggerAttackTransition() {
        m_currentState = ECharacterState.Attacking;
        m_characterRenderer.material.color = attackingBlobColor;
        m_waitingTime = attackTime;

        // TODO Raycast for Collision
        Collider[] bopCollision = Physics.OverlapSphere(hammerBopAim.position, attackRadius, attackLayer);
        if (bopCollision.Length > 0) {
            Debug.Log($"Hitted Something!");
            for(int i = 0; i < bopCollision.Length; i++) {
                AIController aiHitted = bopCollision[i].transform.gameObject.GetComponent<AIController>();
                if(aiHitted != null) {
                    aiHitted.AIWasTagged(taggedColor, new Vector3(Random.value, 0f, Random.value));
                    break;
                }
            }
        }

        // Showing where the hammer will hit
        StartCoroutine(AttackAnimationRoutine());
    }

    private IEnumerator AttackAnimationRoutine() {

        Vector3 originalTransformLocalPosition = hammerTransform.localPosition;
        Vector3 originalLocalEulerAngles = hammerTransform.localEulerAngles;
        hammerTransform.localPosition = new Vector3(hammerBopAim.localPosition.x, hammerBopAim.localPosition.y + 0.25f, hammerBopAim.localPosition.z - 1f);
        hammerTransform.localEulerAngles = new Vector3(90, 0, 0);

        while(m_waitingTime > 0) {
            m_movementVector.x = 0;
            m_movementVector.z = 0;
            m_waitingTime -= Time.deltaTime;
            yield return null;
        }

        m_characterRenderer.material.color = regularBlobColor;
        m_currentState = ECharacterState.Moving;
        hammerTransform.localPosition = originalTransformLocalPosition;
        hammerTransform.localEulerAngles = originalLocalEulerAngles;
    }

    protected virtual void HandleAttack() {
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(hammerBopAim.transform.position, attackRadius);
    }
}
