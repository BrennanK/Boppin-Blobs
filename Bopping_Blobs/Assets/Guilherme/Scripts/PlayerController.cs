using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [Header("Ground Movement")]
    public float playerSpeed;
    [Range(0, 1)]
    public float groundDamping;

    [Header("Jumping Related")]
    public float jumpHeight = 1f;
    public float jumpDistance = 1f;
    public float goingDownGravityMultiplier = 2f;
    // private variables related to jumping
    private float m_initialJumpVelocity;
    private float m_goingUpGravity;
    private float m_goingDownGravity;
    private const float km_jumpPressedRememberTime = 0.15f;
    private const float km_groundedRememberTime = 0.15f;
    private float m_jumpRememberTime;
    private float m_groundedRememberTime;

    // Cached References
    private CharacterController m_characterControllerReference;
    private DigitalJoystick m_digitalJoystickReference;
    private JoyButton m_joyButtonReference;
    private Vector3 m_movementVector;

    private void Awake() {
        m_initialJumpVelocity = (2 * jumpHeight * playerSpeed) / jumpDistance;
        m_goingUpGravity = (-2 * jumpHeight * (playerSpeed * playerSpeed)) / (jumpDistance * jumpDistance);
        m_goingUpGravity /= -Physics.gravity.y;
        m_goingDownGravity = m_goingUpGravity * goingDownGravityMultiplier;
    }

    private void Start() {
        m_characterControllerReference = GetComponent<CharacterController>();
        m_digitalJoystickReference = FindObjectOfType<DigitalJoystick>();
        m_joyButtonReference = FindObjectOfType<JoyButton>();
    }

    private void Update() {
        m_jumpRememberTime -= Time.deltaTime;
        m_groundedRememberTime -= Time.deltaTime;

        // Getting Plane Movement
        m_movementVector.x = m_digitalJoystickReference.Horizontal * playerSpeed;
        m_movementVector.x = Mathf.Lerp(m_characterControllerReference.velocity.x, m_movementVector.x, groundDamping);
        m_movementVector.z = m_digitalJoystickReference.Vertical * playerSpeed;
        m_movementVector.z = Mathf.Lerp(m_characterControllerReference.velocity.z, m_movementVector.z, groundDamping);

        if(m_characterControllerReference.isGrounded) {
            m_groundedRememberTime = km_groundedRememberTime;
            m_movementVector.y = 0;
        } else {
            m_movementVector.y += (m_goingDownGravity * Time.deltaTime);
        }

        if(m_joyButtonReference.pressed) {
            m_jumpRememberTime = km_jumpPressedRememberTime;
        }

        // Jumping Here
        if((m_jumpRememberTime > 0) && (m_groundedRememberTime > 0)) {
            m_jumpRememberTime = 0;
            m_groundedRememberTime = 0;

            m_movementVector.y = m_initialJumpVelocity;
        }

        // Updating the character position
        m_characterControllerReference.Move(m_movementVector * Time.deltaTime);
    }
}
