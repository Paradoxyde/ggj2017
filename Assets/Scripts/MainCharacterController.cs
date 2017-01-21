using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacterController : MonoBehaviour
{
    public float max_speed = 12.0f;
    public float ground_check_radius = 0.05f;
    public Transform feet_pos;
    public LayerMask ground_check_layers;
    public float jump_force = 1000.0f;
    public float high_jump_force = 2400.0f;
    public float high_jump_duration = 0.25f;
    public float double_jump_force = 1000.0f;
    public float double_high_jump_force = 2400.0f;
    public float double_high_jump_duration = 0.25f;
    public float air_control_force = 5000.0f;
    public int continuous_jumps = 2;
    public float test = 100f;

    Vector2 m_moveInput = new Vector2();
    bool m_jumpHeld = false;
    bool m_isFreshJumpPress = true;
    float m_timeSinceJumpPress = 0.0f;
    float m_timeSinceJumpStart = 0.0f;
    bool m_isGrounded = false;
    bool m_isJumping = false;
    int m_continuousJumpCount = 0;

    Rigidbody2D m_rigidBody;
    
	void Start()
    {
        m_rigidBody = GetComponent<Rigidbody2D>();
	}
	
	void Update()
    {
        UpdateInputs();
        UpdateHorizontalVelocity();
        UpdateJumping();
    }

    void UpdateInputs()
    {
        m_isGrounded = Physics2D.OverlapCircle(feet_pos.position, ground_check_radius, ground_check_layers);
        m_moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        bool jumpHeld = Input.GetButton("Jump");
        if (jumpHeld)
        {
            if (!m_jumpHeld)
            {
                m_isFreshJumpPress = true;
                m_timeSinceJumpPress = 0.0f;
            }
            else
            {
                m_timeSinceJumpPress += Time.deltaTime;
            }
        }
        else
        {
            m_isFreshJumpPress = false;
        }
        m_jumpHeld = jumpHeld;
    }

    void UpdateHorizontalVelocity()
    {
        Vector2 currentVel = m_rigidBody.velocity;
        if (m_isGrounded)
        {
            currentVel.x = m_moveInput.x * max_speed;
            m_rigidBody.velocity = currentVel;
        }
        else
        {
            if (currentVel.x >= -max_speed && m_moveInput.x < 0
             || currentVel.x <= max_speed  && m_moveInput.x > 0)
            {
                m_rigidBody.AddForce(new Vector2(m_moveInput.x * air_control_force * Time.deltaTime, 0.0f));
            }
        }
    }

    void UpdateJumping()
    {
        if (m_isJumping)
        {
            m_timeSinceJumpStart += Time.deltaTime;
        }

        if (m_isGrounded && m_timeSinceJumpStart > 0.1f)
        {
            m_isJumping = false;
        }

        if (m_jumpHeld)
        {
            if (m_isFreshJumpPress)
            {
                float currentVertSpeed = m_rigidBody.velocity.y;
                if (m_isGrounded)
                {
                    // Jump from ground
                    m_rigidBody.AddForce(new Vector2(0.0f, jump_force));
                    m_isFreshJumpPress = false;
                    m_timeSinceJumpStart = 0.0f;
                    m_isJumping = true;
                    m_continuousJumpCount = 1;
                }
                else if (m_continuousJumpCount < continuous_jumps && currentVertSpeed <= 0.0f)
                {
                    // Double jump
                    m_rigidBody.AddForce(new Vector2(0.0f, double_jump_force - currentVertSpeed * test));
                    m_isFreshJumpPress = false;
                    m_timeSinceJumpStart = 0.0f;
                    m_continuousJumpCount++;
                }
            }
            else
            {
                // Handle different jump heights
                if (m_isJumping)
                {
                    if (m_continuousJumpCount == 1)
                    {
                        if (m_timeSinceJumpStart < high_jump_duration)
                        {
                            m_rigidBody.AddForce(new Vector2(0.0f, high_jump_force * Time.deltaTime));
                        }
                    }
                    else
                    {
                        if (m_timeSinceJumpStart < double_high_jump_duration)
                        {
                            m_rigidBody.AddForce(new Vector2(0.0f, double_high_jump_force * Time.deltaTime));
                        }
                    }
                }
            }
        }
    }
    
    private void OnGUI()
    {
        GUI.color = Color.red;
        GUI.Label(new Rect(20, 20, 200, 100), String.Format("Move input  : {0:0.00}:{1:0.00}", m_moveInput.x, m_moveInput.y));
        GUI.Label(new Rect(20, 40, 200, 100),               "Jump held   : " + (m_jumpHeld ? "true" : "false"));
        GUI.Label(new Rect(20, 60, 200, 100),               "Is grounded : " + (m_isGrounded ? "true" : "false"));
        GUI.Label(new Rect(20, 80, 200, 100),               "Jump count : " + m_continuousJumpCount);
    }
}
