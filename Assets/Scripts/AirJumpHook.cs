using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirJumpHook : MonoBehaviour
{
    public bool invisible_at_runtime = true;
    public float cooldown_time = 0.5f;
    public PhaseType phase_type = PhaseType.Red;

    public Transform red_particles;
    public Transform blue_particles;
    public Transform neutral_particles;

    Transform childParticles;

    bool m_registered = false;
    float m_timeSinceLastUsed = 10.0f;
    bool m_isAvailable = true;
    bool m_wasOnCooldown = false;

    void Start()
    {
        if (invisible_at_runtime)
        {
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.enabled = false;
            }
        }

        if (phase_type == PhaseType.Red)
        {
            Helpers.MakeRed(gameObject);
            SetParticles(red_particles);
        }
        else if (phase_type == PhaseType.Blue)
        {
            Helpers.MakeBlue(gameObject);
            SetParticles(blue_particles);
        }
        else
        {
            SetParticles(neutral_particles);
        }
    }

    private void SetParticles(Transform ps)
    {
        if (childParticles != null)
        {
            GameObject.Destroy(childParticles);
        }

        childParticles = GameObject.Instantiate(ps, transform.position, transform.rotation, transform);
    }

    void Update()
    {
        if (!m_registered) TryRegister();

        m_timeSinceLastUsed += Time.deltaTime;

        UpdateColor();

        bool isOnCooldown = m_timeSinceLastUsed < cooldown_time;

        if (isOnCooldown != m_wasOnCooldown)
        {
            if (childParticles != null)
            {
                ParticleSystem ps = childParticles.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    if (isOnCooldown)
                    {
                        ps.Clear();
                    }

                    ParticleSystem.MainModule mainModule = ps.main;
                    mainModule.startSize = isOnCooldown ? 0.1f : 2.5f;
                }
            }
        }

        m_wasOnCooldown = m_timeSinceLastUsed < cooldown_time;
        m_isAvailable = m_timeSinceLastUsed > cooldown_time && !Helpers.ArePhasesOpposite(phase_type, WaveManager.Instance.CurrentPhase.phase_type); ;
    }

    void UpdateColor()
    {
        if (m_timeSinceLastUsed < cooldown_time)
        {
            Helpers.MakeGrey(gameObject);
        }
        else
        {
            if (phase_type == PhaseType.Red)
            {
                Helpers.MakeRed(gameObject);
            }
            else
            {
                Helpers.MakeBlue(gameObject);
            }
        }
    }

    public void OnClosestChanged(bool isClosest)
    {
        if (childParticles != null)
        {
            ParticleSystem ps = childParticles.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ParticleSystem.EmissionModule emissionModule = ps.emission;
                emissionModule.rateOverTimeMultiplier = isClosest ? 20.0f : 1.0f;
                ParticleSystem.MainModule mainModule = ps.main;
                mainModule.startLifetime = isClosest ? 0.2f : 2.0f;
                mainModule.startSize = isClosest ? 5.0f : 2.5f;
            }
        }
    }

    public void OnUsed()
    {
        m_timeSinceLastUsed = 0.0f;
    }

    public bool IsAvailable()
    {
        return m_isAvailable;
    }

    void TryRegister()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Player");

        if (go)
        {
            PlatformingEntitiesManager pem = go.GetComponent<PlatformingEntitiesManager>();
            pem.RegisterAirJumpHook(this);
            m_registered = true;
        }
    }

    private void OnDrawGizmos()
    {
        if (phase_type == PhaseType.Red)
        {
            Gizmos.color = Color.red;
        }
        else if (phase_type == PhaseType.Blue)
        {
            Gizmos.color = Color.blue;
        }
        else
        {
            return;
        }
        Gizmos.DrawCube(transform.position + new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.5f, 0.5f, 5.0f));
    }
}
