using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacterController : MonoBehaviour
{
    public float max_speed = 10.0f;
    public float ground_check_radius = 0.2f;
    public Transform feet_pos;
    public LayerMask ground_check_layers;
    Vector2 m_moveInput = new Vector2();
    bool m_jumpHeld = false;
    bool m_isGrounded = false;

    Rigidbody2D m_rigidBody;
    
	void Start()
    {
        m_rigidBody = GetComponent<Rigidbody2D>();
	}
	
	void Update()
    {
        UpdateInputs();
        UpdateHorizontalVelocity();
    }

    void UpdateInputs()
    {
        m_isGrounded = Physics2D.OverlapCircle(feet_pos.position, ground_check_radius, ground_check_layers);
        m_moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        m_jumpHeld = Input.GetButton("Jump");
    }

    void UpdateHorizontalVelocity()
    {
        Vector2 currentVel = m_rigidBody.velocity;
        currentVel.x = m_moveInput.x * max_speed;
        m_rigidBody.velocity = currentVel;
    }

    private void OnGUI()
    {
        GUI.color = Color.red;
        GUI.Label(new Rect(20, 20, 200, 100), String.Format("Move input  : {0:0.00}:{1:0.00}", m_moveInput.x, m_moveInput.y));
        GUI.Label(new Rect(20, 40, 200, 100),               "Jump held   : " + (m_jumpHeld ? "true" : "false"));
        GUI.Label(new Rect(20, 60, 200, 100),               "Is grounded : " + (m_isGrounded ? "true" : "false"));
    }
}
