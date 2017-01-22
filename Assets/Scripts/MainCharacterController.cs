﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacterController : MonoBehaviour
{
    public float max_speed = 12.0f;
    public float ground_check_radius = 0.05f;
    public float wall_check_radius = 0.15f;
    public Transform feet_sensor_pos;
    public Transform left_sensor_pos;
    public Transform right_sensor_pos;
    public LayerMask ground_check_layers;
    public LayerMask wall_check_layers;
    public float jump_force = 1000.0f;
    public float high_jump_force = 2400.0f;
    public float high_jump_duration = 0.25f;
    public float double_jump_force = 850.0f;
    public float double_high_jump_force = 2400.0f;
    public float double_high_jump_duration = 0.25f;
    public float air_control_force = 6000.0f;
    public int air_jumps = 2;
    public float double_jump_vel_force_factor = 45f;
    public float wall_hug_gravity_modifier = 0.25f;
    public float wall_jump_horiz_force = 500.0f;
    public float wall_jump_control_lock_duration = 0.25f;
    public bool enable_wall_jump = true;
    public bool wall_jump_refreshes_air_jump = true;
    public bool wall_jump_refreshes_dash = true;
    public float air_jump_hook_range = 1.5f;

    public float dashing_speed = 40.0f;
    public float min_dashing_duration = 0.15f;
    public float max_dashing_duration = 0.30f;
    public float max_dashing_angle = 25.0f;

    public float jump_shockwave_force = 50f;
    public float jump_shockwave_radius = 6f;


    Vector2 m_moveInput = new Vector2();
    bool m_lastInputWasRight = true;
    bool m_jumpHeld = false;
    bool m_isFreshJumpPress = true;
    float m_timeSinceJumpPress = 0.0f;
    float m_timeSinceJumpStart = 0.0f;
    bool m_isJumping = false;

    bool m_dashHeld = false;
    bool m_isFreshDashPress = true;
    float m_timeSinceDashPress = 0.0f;
    float m_timeSinceLastDashStart = 10.0f;
    bool m_isDashing = false;
    bool m_isDashAvailabe = true;
    Vector2 m_dashingDirection;

    float m_timeHuggingWall = 0.0f;
    int m_airJumpCount = 0;
    bool m_isHuggingWall = false;
    bool m_wallHugIsOnRight = false;
    float m_baseGravityScale = 10.0f;
    AirJumpHook m_airJumpHook = null;

    bool m_isGrounded = false;
    bool m_hasLeftContact = false;
    bool m_hasRightContact = false;
    bool m_isGhosting = false;

    Rigidbody2D m_rigidBody;
    PlatformingEntitiesManager m_platformingEntities;
    JumpType m_jumpType = JumpType.first;

    private Animator m_animator;
    private Transform m_visualsTransform;
    
    enum JumpType
    {
        first,
        air,
        wall
    }

    public bool GetShouldGoThroughTwoWayPlatforms()
    {
        return m_isJumping && m_timeSinceJumpPress < 0.2f;
    }

	void Start()
    {
        m_rigidBody = GetComponent<Rigidbody2D>();
        m_platformingEntities = GetComponent<PlatformingEntitiesManager>();
        m_animator = GetComponentInChildren<Animator>();

        if (m_animator != null)
            m_visualsTransform = m_animator.transform.parent;

        m_baseGravityScale = m_rigidBody.gravityScale;

        if (m_visualsTransform != null)
            m_visualsTransform.LookAt(m_visualsTransform.position - Vector3.right);
    }

    void Update()
    {
        UpdateContacts();
        UpdateInputs();
        
        if (m_isGhosting)
        {
            UpdateGhosting();
        }
        else
        {
            UpdateHorizontalVelocity();
            UpdateWallHugging();
            UpdateJumping();
            UpdateDashing();
        }
    }

    void UpdateContacts()
    {
        bool wasOnGround = m_isGrounded;

        m_isGrounded = Physics2D.OverlapCircle(feet_sensor_pos.position, ground_check_radius, ground_check_layers);
        m_hasLeftContact = Physics2D.OverlapCircle(left_sensor_pos.position, wall_check_radius, wall_check_layers);
        m_hasRightContact = Physics2D.OverlapCircle(right_sensor_pos.position, wall_check_radius, wall_check_layers);
        
        if (m_isGrounded) m_airJumpCount = 0;

        m_animator.SetBool("OnLand", m_isGrounded && !wasOnGround);
        m_animator.SetBool("IsGrounded", m_isGrounded);
        m_animator.SetBool("IsWallSliding", !m_isGrounded && m_isHuggingWall);
        m_animator.SetBool("CanDoubleJump", m_airJumpCount != 0);

        m_airJumpHook = m_platformingEntities.GetClosestActiveAirJumpHook(transform.position, air_jump_hook_range);
    }

    void UpdateInputs()
    {
        m_moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));


        if (Mathf.Abs(m_moveInput.x) > 0.05f)
        {
            bool currentInputIsRight = m_moveInput.x >= 0;
            
            m_animator.SetBool("HasChangedDirection", currentInputIsRight != m_lastInputWasRight);

            m_lastInputWasRight = currentInputIsRight;
        }

        if (m_moveInput.x > 0f)
        {
            if (m_visualsTransform != null)
                m_visualsTransform.LookAt(m_visualsTransform.position - Vector3.right);
        }
        else if (m_moveInput.x < 0f)
        {
            if (m_visualsTransform != null)
                m_visualsTransform.LookAt(m_visualsTransform.position + Vector3.right);
        }

        if (m_moveInput.x != 0)
        {
            m_animator.SetBool("IsRunning", true);
        }
        else
        {
            m_animator.SetBool("IsRunning", false);
        }

        if (Input.GetButtonDown("Ghost"))
        {
            m_isGhosting = !m_isGhosting; // [REMOVE]
            m_animator.SetBool("IsGhosting", m_isGhosting);
            if (m_isGhosting)
            {
                m_rigidBody.gravityScale = 0.0f;
                m_rigidBody.velocity = Vector3.zero;

                m_timeSinceJumpStart = 0.0f;
                m_isJumping = false;

                m_timeSinceLastDashStart = 10.0f;
                m_isDashing = false;
                m_isDashAvailabe = true;

                m_timeHuggingWall = 0.0f;
                m_airJumpCount = 0;
                m_isHuggingWall = false;
                m_airJumpHook = null;

                Collider2D col = GetComponent<Collider2D>();
                col.enabled = false;
            }
            else
            {
                m_rigidBody.gravityScale = m_baseGravityScale;
                Collider2D col = GetComponent<Collider2D>();
                col.enabled = true;
            }
        }

        bool jumpHeld = Input.GetButton("Jump");
        if (jumpHeld)
        {
            if (!m_jumpHeld)
            {
                m_isFreshJumpPress = true;
                m_timeSinceJumpPress = 0.0f;
                Helpers.ShockWave(transform.position, jump_shockwave_radius, jump_shockwave_force);
            }
        }
        else
        {
            m_isFreshJumpPress = false;
        }
        m_jumpHeld = jumpHeld;
        m_timeSinceJumpPress += Time.deltaTime;

        bool dashHeld = Input.GetButton("Dash");
        if (dashHeld)
        {
            if (!m_dashHeld)
            {
                m_isFreshDashPress = true;
                m_timeSinceDashPress = 0.0f;
            }
            else
            {
                m_timeSinceDashPress += Time.deltaTime;
            }
        }
        else
        {
            m_isFreshDashPress = false;
        }
        m_dashHeld = dashHeld;

        m_dashHeld = Input.GetButton("Dash");
    }

    void UpdateGhosting()
    {
        float ghostSpeed = 40.0f;
        Vector3 position = transform.position;
        position.x += m_moveInput.x * ghostSpeed * Time.deltaTime;
        position.y += m_moveInput.y * ghostSpeed * Time.deltaTime;
        transform.position = position;
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
            if (!(m_isJumping && m_jumpType == JumpType.wall && m_timeSinceJumpStart < wall_jump_control_lock_duration))
            {
                if ((currentVel.x >= -max_speed && m_moveInput.x < 0 && !m_hasLeftContact)
                 || (currentVel.x <= max_speed && m_moveInput.x > 0 && !m_hasRightContact))
                {
                    m_rigidBody.AddForce(new Vector2(m_moveInput.x * air_control_force * Time.deltaTime, 0.0f));
                }
            }
        }
    }

    void UpdateWallHugging()
    {
        Vector2 currentVel = m_rigidBody.velocity;
        bool shouldHugWall = m_isHuggingWall;
        if (!m_isHuggingWall)
        {
            shouldHugWall = (m_hasLeftContact && m_moveInput.x < -0.5f)
             || (m_hasRightContact && m_moveInput.x > 0.5f);
            shouldHugWall &= !m_isGrounded;
            if (shouldHugWall) m_wallHugIsOnRight = m_hasRightContact;
        }
        else
        {
            shouldHugWall = ((m_moveInput.x <= 0f && m_hasLeftContact) || (m_moveInput.x >= 0f && m_hasRightContact)) && !m_isGrounded;
            m_timeHuggingWall += Time.deltaTime;
        }

        if (m_isHuggingWall != shouldHugWall)
        {
            m_isHuggingWall = shouldHugWall;
            if (m_isHuggingWall) m_timeHuggingWall = 0.0f;
        }
        
        m_rigidBody.gravityScale = m_baseGravityScale * (m_isHuggingWall && currentVel.y <= 0 ? wall_hug_gravity_modifier : 1.0f);
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
                    m_airJumpCount = 1;
                    m_jumpType = JumpType.first;

                    m_animator.SetBool("OnJump", true);
                }
                else if (enable_wall_jump && m_isHuggingWall && currentVertSpeed <= 0.0f && m_timeHuggingWall > 0.1f)
                {
                    // Air jump
                    float horizForce = wall_jump_horiz_force * (m_wallHugIsOnRight ? -1.0f : 1.0f);
                    if (horizForce * m_rigidBody.velocity.x > 0) // We're pushing in the direction we're already going, reduce the push
                    {
                        horizForce -= m_rigidBody.velocity.x * double_jump_vel_force_factor;
                    }

                    m_rigidBody.AddForce(new Vector2(horizForce, double_jump_force - currentVertSpeed * double_jump_vel_force_factor));
                    m_isFreshJumpPress = false;
                    m_timeSinceJumpStart = 0.0f;
                    m_jumpType = JumpType.wall;
                    
                    if (wall_jump_refreshes_air_jump)
                    {
                        m_airJumpCount = 1;
                    }

                    if (wall_jump_refreshes_dash && m_timeSinceLastDashStart > 0.2f)
                    {
                        m_isDashAvailabe = true;
                    }
                    m_animator.SetBool("OnJump", true);
                }
                else if (!m_isHuggingWall && ((m_airJumpCount < air_jumps && currentVertSpeed <= 0.0f) || m_airJumpHook != null))
                {
                    // Double jump
                    m_rigidBody.AddForce(new Vector2(0.0f, double_jump_force - currentVertSpeed * double_jump_vel_force_factor));
                    m_isFreshJumpPress = false;
                    m_timeSinceJumpStart = 0.0f;
                    if (m_airJumpHook != null)
                    {
                        m_airJumpHook.OnUsed();
                    }
                    else
                    {
                        m_airJumpCount++;
                    }
                    m_jumpType = JumpType.air;
                    m_animator.SetBool("OnJump", true);
                }
            }
            else
            {
                m_animator.SetBool("OnJump", false);

                // Handle different jump heights
                if (m_isJumping /*&& !m_hasLeftContact && !m_hasRightContact*/)
                {
                    if (m_jumpType == JumpType.first)
                    {
                        if (m_timeSinceJumpStart < high_jump_duration)
                        {
                            m_rigidBody.AddForce(new Vector2(0.0f, high_jump_force * Time.deltaTime));
                        }
                    }
                    else if (m_jumpType == JumpType.air || m_jumpType == JumpType.wall)
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

    private void UpdateDashing()
    {
        m_timeSinceLastDashStart += Time.deltaTime;

        if (m_isGrounded)
        {
            m_isDashAvailabe = true;
        }

        if (m_isDashing)
        {
            if ((m_dashHeld && m_timeSinceLastDashStart < max_dashing_duration) || m_timeSinceLastDashStart < min_dashing_duration)
            {
                m_rigidBody.velocity = m_dashingDirection * dashing_speed;
            }
            else
            {
                m_rigidBody.velocity = m_rigidBody.velocity * 0.5f;
                m_isDashing = false;
            }
        }
        else
        {
            if (m_dashHeld && m_isFreshDashPress && m_isDashAvailabe && m_timeSinceLastDashStart > 0.8)
            {
                m_isDashing = true;
                m_isDashAvailabe = false;
                m_timeSinceLastDashStart = 0.0f;
                m_isFreshDashPress = false;

                if (m_moveInput.sqrMagnitude > 0.01f)
                {
                    m_dashingDirection = m_moveInput.normalized;
                }
                else
                {
                    if (m_lastInputWasRight)
                    {
                        m_dashingDirection = new Vector2(1.0f, 0.0f);
                    }
                    else
                    {
                        m_dashingDirection = new Vector2(-1.0f, 0.0f);
                    }
                }

                float dashingAngle = Mathf.Atan2(m_dashingDirection.y, m_dashingDirection.x) * Mathf.Rad2Deg;
                if (dashingAngle >= 90 && dashingAngle < 180 - max_dashing_angle)
                {
                    dashingAngle = 180 - max_dashing_angle;
                }
                else if (dashingAngle <= 90 && dashingAngle > max_dashing_angle)
                {
                    dashingAngle = max_dashing_angle;
                }
                else if (dashingAngle <= -90 && dashingAngle > -180 + max_dashing_angle)
                {
                    dashingAngle = -180 + max_dashing_angle;
                }
                else if (dashingAngle >= -90 && dashingAngle < -max_dashing_angle)
                {
                    dashingAngle = -max_dashing_angle;
                }
                m_dashingDirection.x = Mathf.Cos(dashingAngle * Mathf.Deg2Rad);
                m_dashingDirection.y = Mathf.Sin(dashingAngle * Mathf.Deg2Rad);
            }
        }
        
        m_animator.SetBool("OnDash", m_isDashing);
    }
    
    private void OnGUI()
    {
        GUI.color = Color.red;
        string contacts = "";
        if (m_isGrounded) contacts += "ground ";
        if (m_hasLeftContact) contacts += "left ";
        if (m_hasRightContact) contacts += "right ";
        /*GUI.Label(new Rect(20, 20, 200, 100), String.Format("Horiz input: {0:0.00}", m_moveInput.x));
        GUI.Label(new Rect(20, 40, 200, 100),               "Jump held: " + (m_jumpHeld ? "true" : "false"));
        GUI.Label(new Rect(20, 60, 200, 100),               "Dash held: " + (m_dashHeld ? "true" : "false"));
        GUI.Label(new Rect(20, 80, 200, 100),               "Hugging wall: " + (m_isHuggingWall ? "true" : "false"));
        GUI.Label(new Rect(20, 100, 200, 100),               "Contacts : " + contacts);
        GUI.Label(new Rect(20, 120, 200, 100),               "Air hook? : " + (m_airJumpHook != null ? "true" : "false"));*/
    }

    public void OnPlayerDied()
    {
        m_timeSinceJumpStart = 0.0f;
        m_isJumping = false;
        
        m_timeSinceLastDashStart = 10.0f;
        m_isDashing = false;
        m_isDashAvailabe = true;

        m_timeHuggingWall = 0.0f;
        m_airJumpCount = 0;
        m_isHuggingWall = false;
        m_airJumpHook = null;
    }
}
