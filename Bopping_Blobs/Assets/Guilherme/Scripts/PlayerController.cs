using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public enum EPlayerState {
        Moving,
        Attacking,
    }

    [Header("Ground Movement")]
    public float playerSpeed;
    [Range(0, 1)]
    public float groundDamping;
    public float fallingGravityMultiplier = 3f;

    [Header("Attack Related")]
    public float attackDistance = 1f;
    public float attackTime = 1f;

    [Header("Feedback Colors (temp")]
    public Color regularBlobColor;
    public Color attackingBlobColor;

    // Cached References
    private CharacterController m_characterControllerReference;
    private DigitalJoystick m_digitalJoystickReference;
    private JoyButton m_joyButtonReference;
    private Vector3 m_movementVector;
    private Renderer m_playerRenderer;

    // Tracking Current State
    private EPlayerState m_currentState;
    private float m_waitingTime;

    private void Start() {
        m_characterControllerReference = GetComponent<CharacterController>();
        m_digitalJoystickReference = FindObjectOfType<DigitalJoystick>();
        m_joyButtonReference = FindObjectOfType<JoyButton>();
        m_currentState = EPlayerState.Moving;

        m_playerRenderer = GetComponent<Renderer>();
    }

    private void Update() {
        switch(m_currentState) {
            case EPlayerState.Moving:
                HandleMovement();
                break;
            case EPlayerState.Attacking:
                HandleAttack();
                break;
        }

        // TODO maybe this could be a class variable
        float dampingMultiplier = 1f;
        if(m_currentState == EPlayerState.Attacking) {
            dampingMultiplier *= 2;
        }

        m_movementVector.x = Mathf.Lerp(m_characterControllerReference.velocity.x, m_movementVector.x, groundDamping * dampingMultiplier);
        m_movementVector.z = Mathf.Lerp(m_characterControllerReference.velocity.z, m_movementVector.z, groundDamping * dampingMultiplier);

        // Drawing Debug Rays
        Debug.DrawRay(transform.position, transform.forward * attackDistance, Color.red);
        Debug.DrawRay(transform.position, new Vector3(m_movementVector.x, 0f, m_movementVector.z).normalized * attackDistance, Color.green);

        // Grounding the player
        if (m_characterControllerReference.isGrounded) {
            m_movementVector.y = 0;
        } else {
            m_movementVector.y += (Physics.gravity.y * fallingGravityMultiplier * Time.deltaTime);
        }

        // Updating the character position
        m_characterControllerReference.Move(m_movementVector * Time.deltaTime);
    }

    private void HandleMovement() {
        // Getting Plane Movement
        m_movementVector.x = m_digitalJoystickReference.Horizontal * playerSpeed;
        m_movementVector.z = m_digitalJoystickReference.Vertical * playerSpeed;
        
        // Looking at the position we are moving to
        transform.LookAt(transform.position + new Vector3(m_movementVector.x, 0f, m_movementVector.z));

        if (m_joyButtonReference.pressed) {
            m_currentState = EPlayerState.Attacking;
            m_playerRenderer.material.color = attackingBlobColor;
            m_waitingTime = attackTime;

            // TODO Raycast forward and see if there is a collision
        }
    }

    private void HandleAttack() {
        m_movementVector.x = 0;
        m_movementVector.z = 0;

        m_waitingTime -= Time.deltaTime;

        // TODO maybe raycast here so the attack has a time to land (has to be timed with animation)

        if(m_waitingTime <= 0) {
            m_playerRenderer.material.color = regularBlobColor;
            m_currentState = EPlayerState.Moving;
        }
    }
}
