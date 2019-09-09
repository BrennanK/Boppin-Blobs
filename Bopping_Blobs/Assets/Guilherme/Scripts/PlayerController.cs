using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [Header("Ground Movement")]
    public float playerSpeed;
    [Range(0, 1)]
    public float groundDamping;
    public float fallingGravityMultiplier = 3f;

    [Header("Attack Related")]
    public float attackDistance = 1f;

    // Cached References
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
        // Getting Plane Movement
        m_movementVector.x = m_digitalJoystickReference.Horizontal * playerSpeed;
        m_movementVector.x = Mathf.Lerp(m_characterControllerReference.velocity.x, m_movementVector.x, groundDamping);
        m_movementVector.z = m_digitalJoystickReference.Vertical * playerSpeed;
        m_movementVector.z = Mathf.Lerp(m_characterControllerReference.velocity.z, m_movementVector.z, groundDamping);

        // Drawing Debug Rays
        Debug.DrawRay(transform.position, transform.forward * attackDistance, Color.red);
        Debug.DrawRay(transform.position, new Vector3(m_movementVector.x, 0f, m_movementVector.z).normalized * attackDistance, Color.green);
        transform.LookAt(transform.position + new Vector3(m_movementVector.x, 0f, m_movementVector.z));

        if (m_characterControllerReference.isGrounded) {
            m_movementVector.y = 0;
        } else {
            m_movementVector.y += (Physics.gravity.y * fallingGravityMultiplier * Time.deltaTime);
        }

        // Updating the character position
        m_characterControllerReference.Move(m_movementVector * Time.deltaTime);
    }
}
