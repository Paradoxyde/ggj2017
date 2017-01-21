using System;
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

    Vector2 m_moveInput = new Vector2();
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

    Rigidbody2D m_rigidBody;
    PlatformingEntitiesManager m_platformingEntities;
    JumpType m_jumpType = JumpType.first;
    
    enum JumpType
    {
        first,
        air,
        wall
    }

	void Start()
    {
        m_rigidBody = GetComponent<Rigidbody2D>();
        m_platformingEntities = GetComponent<PlatformingEntitiesManager>();
        m_baseGravityScale = m_rigidBody.gravityScale;
    }
	
	void Update()
    {
        UpdateContacts();
        UpdateInputs();
        UpdateHorizontalVelocity();
        UpdateWallHugging();
        UpdateJumping();
        UpdateDashing();
    }

    void UpdateContacts()
    {
        m_isGrounded = Physics2D.OverlapCircle(feet_sensor_pos.position, ground_check_radius, ground_check_layers);
        m_hasLeftContact = Physics2D.OverlapCircle(left_sensor_pos.position, wall_check_radius, wall_check_layers);
        m_hasRightContact = Physics2D.OverlapCircle(right_sensor_pos.position, wall_check_radius, wall_check_layers);

        m_airJumpHook = m_platformingEntities.GetClosestActiveAirJumpHook(transform.position, air_jump_hook_range);
    }

    void UpdateInputs()
    {
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
            shouldHugWall = (m_hasLeftContact || m_hasRightContact) && !m_isGrounded;
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
                }
            }
            else
            {
                // Handle different jump heights
                if (m_isJumping && !m_hasLeftContact && !m_hasRightContact)
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
                m_dashingDirection = m_moveInput.normalized;

                float dashingAngle = Mathf.Atan2(m_dashingDirection.y, m_dashingDirection.x) * Mathf.Rad2Deg;
                Debug.Log(dashingAngle);
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
    }
    
    private void OnGUI()
    {
        GUI.color = Color.red;
        string contacts = "";
        if (m_isGrounded) contacts += "ground ";
        if (m_hasLeftContact) contacts += "left ";
        if (m_hasRightContact) contacts += "right ";
        GUI.Label(new Rect(20, 20, 200, 100), String.Format("Horiz input: {0:0.00}", m_moveInput.x));
        GUI.Label(new Rect(20, 40, 200, 100),               "Jump held: " + (m_jumpHeld ? "true" : "false"));
        GUI.Label(new Rect(20, 60, 200, 100),               "Dash held: " + (m_dashHeld ? "true" : "false"));
        GUI.Label(new Rect(20, 80, 200, 100),               "Hugging wall: " + (m_isHuggingWall ? "true" : "false"));
        GUI.Label(new Rect(20, 100, 200, 100),               "Contacts : " + contacts);
        GUI.Label(new Rect(20, 120, 200, 100),               "Air hook? : " + (m_airJumpHook != null ? "true" : "false"));
    }
}
