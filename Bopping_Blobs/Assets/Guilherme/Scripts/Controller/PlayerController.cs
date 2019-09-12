using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IBoppable {
    // TODO separate the hammer/tag/it logic from the base controller
    public enum ECharacterState {
        Moving,
        Attacking,
        KnockedBack
    }

    [Header("Ground Movement")]
    public float characterSpeed;
    [Range(0, 1)]
    public float groundDamping;
    public float fallingGravityMultiplier = 3f;

    // Cached References
    private CharacterController m_characterControllerReference;
    private Vector3 m_movementVector;
    private DigitalJoystick m_digitalJoystickReference;
    private JoyButton m_joyButtonReference;

    // Tracking Current State
    private ECharacterState m_currentState;

    private void Start() {
        m_characterControllerReference = GetComponent<CharacterController>();
        m_currentState = ECharacterState.Moving;
        m_digitalJoystickReference = FindObjectOfType<DigitalJoystick>();
        m_joyButtonReference = FindObjectOfType<JoyButton>();
    }

    private void Update() {
        switch (m_currentState) {
            case ECharacterState.Moving:
                HandleMovement();
                break;
            case ECharacterState.Attacking:
                m_movementVector.x = 0;
                m_movementVector.z = 0;
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

        // Updating the character position
        m_characterControllerReference.SimpleMove(m_movementVector);
    }

    private void HandleMovement() {
        // Getting Plane Movement
        m_movementVector.x = m_digitalJoystickReference.Horizontal * characterSpeed;
        m_movementVector.z = m_digitalJoystickReference.Vertical * characterSpeed;
        
        // Looking at the position we are moving to
        transform.LookAt(transform.position + new Vector3(m_movementVector.x, 0f, m_movementVector.z));
        //
    }

    #region IBoppable Functions
    public bool HasAttacked() {
        return m_joyButtonReference.pressed;
    }

    public void TriggerAttackTransition() {
        m_currentState = ECharacterState.Attacking;
    }

    public void TriggerEndAttackTransition() {
        m_currentState = ECharacterState.Moving;
    }

    // Not Used on Player Controller
    public void UpdateWhoIsTag(Transform _whoIsTag) {
        return;
    }

    public void TriggerIsTagTransition() {
        return;
    }

    public void TriggerIsNotTagTransition() {
        return;
    }

    public void DeactivateController() {
        m_currentState = ECharacterState.KnockedBack;
    }

    public void ReactivateController() {
        m_currentState = ECharacterState.Moving;
    }
    #endregion
}
