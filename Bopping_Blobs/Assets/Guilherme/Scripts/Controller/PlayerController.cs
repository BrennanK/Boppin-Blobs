using UnityEngine;

public class PlayerController : MonoBehaviour, IBoppable {
    public enum ECharacterState {
        Moving,
        Attacking,
        KnockedBack
    }

    private float m_characterSpeed = 10f;
    [Header("Ground Movement")]
    [Range(0, 1)]
    public float groundDamping;

    // Cached References
    private CharacterController m_characterControllerReference;
    private Vector3 m_movementVector;
    private DigitalJoystick m_digitalJoystickReference;
    private JoyButton m_joyButtonReference;
    private Transform m_whoIsTag;

    // Tracking Current State
    private ECharacterState m_currentState;

    private void Awake() {
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

        float dampingMultiplier = 1f;
        if (m_currentState == ECharacterState.Attacking) {
            dampingMultiplier *= 2;
        }

        m_movementVector.x = Mathf.Lerp(m_characterControllerReference.velocity.x, m_movementVector.x, groundDamping * dampingMultiplier);
        m_movementVector.y = 0;
        m_movementVector.z = Mathf.Lerp(m_characterControllerReference.velocity.z, m_movementVector.z, groundDamping * dampingMultiplier);

        if(m_characterControllerReference.enabled) {
            m_characterControllerReference.SimpleMove(m_movementVector);
        }
    }

    private void HandleMovement() {
        // TEMP
        if(m_digitalJoystickReference.Horizontal == 0 && m_digitalJoystickReference.Vertical == 0) {
            m_movementVector.x = Input.GetAxis("Horizontal") * m_characterSpeed;
            m_movementVector.z = Input.GetAxis("Vertical") * m_characterSpeed;
        } else {
            m_movementVector.x = m_digitalJoystickReference.Horizontal * m_characterSpeed;
            m_movementVector.z = m_digitalJoystickReference.Vertical * m_characterSpeed;
        }

        transform.LookAt(transform.position + new Vector3(m_movementVector.x, 0f, m_movementVector.z));
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

    public void UpdateWhoIsTag(Transform _whoIsTag) {
        m_whoIsTag = _whoIsTag;
    }

    public void ChangeSpeed(float _baseSpeed, float _tempSpeedBost, float _externalSpeedBoost) {
        m_characterSpeed = _baseSpeed + _tempSpeedBost + _externalSpeedBoost;
    }

    public float GetSpeed() {
        return m_characterSpeed;
    }

    public void DeactivateController() {
        m_characterControllerReference.enabled = false;
        m_currentState = ECharacterState.KnockedBack;
    }

    public void ReactivateController() {
        m_characterControllerReference.enabled = true;
        m_currentState = ECharacterState.Moving;
    }
#endregion
}
