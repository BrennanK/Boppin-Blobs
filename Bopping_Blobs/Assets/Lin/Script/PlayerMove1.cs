using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove1 : MonoBehaviour
{
    public float addForce = 10f;//accelerate
    public float dragForce = 5f;//decelerate
    public float maxSpeed = 10f;
    public float jumpForce = 5f;
    public DigitalJoystick DJ;
    private JoyButton JB;
    public Rigidbody rb;
    private bool jump;
    private Vector3 movement;
    void Start()
    {
        DJ = FindObjectOfType<DigitalJoystick>();
        rb = this.gameObject.GetComponent<Rigidbody>();
        JB = FindObjectOfType<JoyButton>();
    }

    void Update()
    {
        movement.x = DJ.Horizontal;
        movement.z = DJ.Vertical;
    }
    public void FixedUpdate()
    {
        Vector3 direction = Vector3.forward * DJ.Vertical + Vector3.right * DJ.Horizontal;
        if (direction != Vector3.zero)
        {
            rb.drag = 0f;
            rb.AddForce(direction * addForce * Time.fixedDeltaTime, ForceMode.VelocityChange);
            //rb.velocity = new Vector3(DJ.Horizontal*addForce,rb.velocity.y,DJ.Vertical*addForce);
            //rb.MovePosition(rb.position + movement *addForce*Time.fixedDeltaTime);
        }
        else
        {
            rb.drag = dragForce;
            //rb.velocity = new Vector3(1, 0, 1);

        }

        if (!jump && JB.pressed)
        {
            jump = true;
            rb.velocity += Vector3.up * jumpForce;
        }

        if (jump && !JB.pressed)
        {
            jump = false;
        }
    }
}
