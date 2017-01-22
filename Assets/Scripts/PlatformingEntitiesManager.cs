using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformingEntitiesManager : MonoBehaviour
{
    List<AirJumpHook> m_airJumpHooks;
    List<PassThroughPlatform> m_passThroughPlatforms;
    List<Checkpoint> m_checkpoints;
    Rigidbody2D m_rigidBody;
    Collider2D m_collider;
    public ParticleSystem death_particles;

    bool m_ignoringPassThroughCollisions = false;
    int m_currentCheckpoint = 0;
    Vector3 m_initialPlayerPosition;

    bool m_isDying = false;
    float m_deathDuration = 1.0f;
    float m_deathTimer = 0.0f;

    void Start()
    {
        m_airJumpHooks = new List<AirJumpHook>();
        m_passThroughPlatforms = new List<PassThroughPlatform>();
        m_checkpoints = new List<Checkpoint>();
        m_rigidBody = GetComponent<Rigidbody2D>();
        m_collider = GetComponent<Collider2D>();
        m_initialPlayerPosition = transform.position;
    }
	
	void Update()
    {
        float playerVerVel = m_rigidBody.velocity.y;

        bool ignorePassThroughCollisions = playerVerVel > 0.05f || Input.GetAxis("Vertical") < -0.9f;

        GameObject go = GameObject.FindGameObjectWithTag("Player");
        if (go)
        {
            MainCharacterController controller = go.GetComponent<MainCharacterController>();
            ignorePassThroughCollisions |= controller.GetShouldGoThroughTwoWayPlatforms();
        }

        if (m_ignoringPassThroughCollisions != ignorePassThroughCollisions)
        {
            m_ignoringPassThroughCollisions = ignorePassThroughCollisions;
            OnIgnorePassThroughCollisionsChanged(m_ignoringPassThroughCollisions);
        }

        if (m_isDying)
        {
            m_deathTimer += Time.deltaTime;
            if (m_deathTimer >= m_deathDuration)
            {
                transform.position = GetRespawnPosition();

                SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
                foreach (SkinnedMeshRenderer r in renderers)
                {
                    r.enabled = true;
                }
                m_isDying = false;
            }
        }
	}

    private void OnIgnorePassThroughCollisionsChanged(bool ignore)
    {
        Physics2D.IgnoreLayerCollision(9, 10, ignore);

        if (m_collider)
        {
            foreach (PassThroughPlatform platform in m_passThroughPlatforms)
            {
                Collider2D collider = platform.GetComponent<Collider2D>();
                Physics2D.IgnoreCollision(m_collider, collider, ignore);
            }
        }
    }
    
    public void RegisterAirJumpHook(AirJumpHook hook)
    {
        m_airJumpHooks.Add(hook);
    }

    public void RegisterPassThroughPlatform(PassThroughPlatform platform)
    {
        m_passThroughPlatforms.Add(platform);
    }

    public void RegisterCheckpoint(Checkpoint checkpoint)
    {
        foreach(Checkpoint c in m_checkpoints)
        {
            if (checkpoint.checkpoint_index == c.checkpoint_index)
            {
                Debug.LogWarning("2 checkpoints have the same index. Make sure each index is unique!");
            }
        }

        m_checkpoints.Add(checkpoint);
    }

    public Vector3 GetRespawnPosition()
    {
        Vector3 spawnPos = Vector3.zero;
        if (!GetCheckpointPosition(m_currentCheckpoint, ref spawnPos))
        {
            spawnPos = m_initialPlayerPosition;
        }
        return spawnPos;
    }

    public bool GetCheckpointPosition(int index, ref Vector3 pos)
    {
        foreach (Checkpoint c in m_checkpoints)
        {
            if (index == c.checkpoint_index)
            {
                pos = c.transform.position + new Vector3(0.0f, 1.0f);

                return true;
            }
        }
        return false;
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

    public void OnPlayerDied()
    {
        if (!m_isDying)
        {
            SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer r in renderers)
            {
                r.enabled = false;
            }

            MainCharacterController controller = GetComponent<MainCharacterController>();
            if (controller != null)
            {
                controller.OnPlayerDied();
            }

            GameObject.Instantiate(death_particles, transform.position, transform.rotation);

            m_isDying = true;
            m_deathTimer = 0.0f;
        }
    }

    public void OnReachedCheckpoint(int index)
    {
        if (m_currentCheckpoint < index)
        {
            m_currentCheckpoint = index;
        }
    }
}
