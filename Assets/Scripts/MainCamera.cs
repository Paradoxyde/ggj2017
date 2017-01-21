using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{

    public float lookAheadHorizontal = 4.0f;
    public float lookAheadUp = 3.0f;
    public float lookAheadDown = 3.0f;

    public float horizontalOffsetUpwardsDamping = 0.1f;
    public float horizontalOffsetDownwardsDamping = 0.8f;
    public float verticalJumpDamping = 0.1f;
    public float verticalFallDamping = 0.5f;
    public float followDamping = 0.5f;
    public Transform target;

    private Vector3 m_TargetOffset = Vector3.zero;
    private float m_HorizontalOffsetVelocity = 0f;
    private float m_VerticalOffsetVelocity = 0f;
    private Vector3 m_FollowVelocity = Vector3.zero;

	
	void Update ()
    {
        FindTarget();
        UpdateOffset();
        UpdatePosition();
    }

    private void FindTarget()
    {
        if (!target)
        {
            var character = FindObjectOfType<MainCharacterController>();
            if (character)
                target = character.transform;
        }
    }

    private void UpdateOffset()
    {
        Rigidbody2D rigidbody2D = target.GetComponent<Rigidbody2D>();
        if (!rigidbody2D)
            return;

        Vector2 vel = rigidbody2D.velocity;

        
        // Horizontal Offset
        float targetOffsetX = System.Math.Sign(vel.x) * lookAheadHorizontal;
        float dampingX = horizontalOffsetDownwardsDamping;
        if (Mathf.Abs(targetOffsetX) > Mathf.Abs(m_TargetOffset.x))
        {
            dampingX = horizontalOffsetUpwardsDamping;
        }

        m_TargetOffset.x = Mathf.SmoothDamp(m_TargetOffset.x, targetOffsetX, ref m_HorizontalOffsetVelocity, dampingX);


        // Vertical Offset
        float targetOffsetY = lookAheadUp;
        float dampingY = verticalJumpDamping;
        if (vel.y < 0f)
        {
            targetOffsetY = -lookAheadDown;
            dampingY = verticalFallDamping;
        }

        m_TargetOffset.y = Mathf.SmoothDamp(m_TargetOffset.y, targetOffsetY, ref m_VerticalOffsetVelocity, dampingY);
    }

    private void UpdatePosition()
    {
        Vector3 targetPos = target ? target.position : transform.position;
        targetPos += m_TargetOffset;

        Vector3 targetCamPos = new Vector3(targetPos.x, targetPos.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, targetCamPos, ref m_FollowVelocity, followDamping);
    }
}
