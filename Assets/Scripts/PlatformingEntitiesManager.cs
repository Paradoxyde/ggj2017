using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformingEntitiesManager : MonoBehaviour
{
    List<AirJumpHook> m_airJumpHooks;
    List<PassThroughPlatform> m_passThroughPlatforms;
    Rigidbody2D m_rigidBody;
    Collider2D m_collider;

    bool m_ignoringPassThroughCollisions = false;

    void Start()
    {
        m_airJumpHooks = new List<AirJumpHook>();
        m_passThroughPlatforms = new List<PassThroughPlatform>();
        m_rigidBody = GetComponent<Rigidbody2D>();
        m_collider = GetComponent<Collider2D>();
    }
	
	void Update()
    {
        float playerVerVel = m_rigidBody.velocity.y;

        bool ignorePassThroughCollisions = playerVerVel > 0.05f || Input.GetAxis("Vertical") < -0.9f;

        if (m_ignoringPassThroughCollisions != ignorePassThroughCollisions)
        {
            m_ignoringPassThroughCollisions = ignorePassThroughCollisions;
            OnIgnorePassThroughCollisionsChanged(m_ignoringPassThroughCollisions);
        }
	}

    private void OnIgnorePassThroughCollisionsChanged(bool ignore)
    {
        if (m_collider)
        {
            foreach (PassThroughPlatform platform in m_passThroughPlatforms)
            {
                Collider2D collider = platform.GetComponent<Collider2D>();
                Physics2D.IgnoreCollision(m_collider, collider, ignore);
            }
        }

        Physics2D.IgnoreLayerCollision(9, 10, ignore);
    }
    
    public void RegisterAirJumpHook(AirJumpHook hook)
    {
        m_airJumpHooks.Add(hook);
    }

    public void RegisterPassThroughPlatform(PassThroughPlatform platform)
    {
        m_passThroughPlatforms.Add(platform);
    }

    public AirJumpHook GetClosestActiveAirJumpHook(Vector3 position, float range)
    {
        float rangeSq = range * range;
        AirJumpHook closestHook = null;
        float closestRangeSq = float.MaxValue;
        
        foreach(AirJumpHook hook in m_airJumpHooks)
        {
            float distanceSq = (position - hook.transform.position).sqrMagnitude;
            if (hook.IsAvailable() && distanceSq < rangeSq && distanceSq < closestRangeSq)
            {
                closestRangeSq = distanceSq;
                closestHook = hook;
            }
        }

        return closestHook;
    }
}
