using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [Header("Ground Movement")]
    public float playerSpeed;
    [Range(0, 1)]
    public float groundDamping;

    [Header("Jumping Related")]
    public float jumpHeight;

    private CharacterController m_characterControllerReference;
    private DigitalJoystick m_digitalJoystickReference;
    private JoyButton m_joyButtonReference;
    private Vector3 m_movementVector;

    private void Start() {
        m_characterControllerReference = GetComponent<CharacterController>();
        m_digitalJoystickReference = FindObjectOfType<DigitalJoystick>();
        m_joyButtonReference = FindObjectOfType<JoyButton>();
    }

    private void Update() {
        m_movementVector.x = m_digitalJoystickReference.Horizontal * playerSpeed;
        m_movementVector.x = Mathf.Lerp(m_characterControllerReference.velocity.x, m_movementVector.x, groundDamping);
        m_movementVector.z = m_digitalJoystickReference.Vertical * playerSpeed;
        m_movementVector.z = Mathf.Lerp(m_characterControllerReference.velocity.z, m_movementVector.z, groundDamping);

        m_characterControllerReference.Move(m_movementVector * Time.deltaTime);
    }
}
