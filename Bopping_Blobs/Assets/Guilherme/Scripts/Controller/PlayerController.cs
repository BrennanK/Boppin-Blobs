using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseBlobController {
    
    // Player Cached References
    private DigitalJoystick m_digitalJoystickReference;
    private JoyButton m_joyButtonReference;

    protected override void InitializeController() {
        base.InitializeController();
        m_digitalJoystickReference = FindObjectOfType<DigitalJoystick>();
        m_joyButtonReference = FindObjectOfType<JoyButton>();

    }

    protected override void HandleMovement() {
        // Getting Plane Movement
        m_movementVector.x = m_digitalJoystickReference.Horizontal * characterSpeed;
        m_movementVector.z = m_digitalJoystickReference.Vertical * characterSpeed;
        
        // Looking at the position we are moving to
        transform.LookAt(transform.position + new Vector3(m_movementVector.x, 0f, m_movementVector.z));

        if (m_joyButtonReference.pressed) {
            TriggerAttackTransition();
        }
    }
}
