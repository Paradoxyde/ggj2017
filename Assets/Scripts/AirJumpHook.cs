using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirJumpHook : MonoBehaviour
{
    public float cooldown_time = 0.5f;
    public PhaseType phase_type = PhaseType.Red;

    bool m_registered = false;
    float m_timeSinceLastUsed = 10.0f;
    bool m_isAvailable = true;

	void Start ()
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
	
	void Update ()
    {
        if (!m_registered) TryRegister();

        m_timeSinceLastUsed += Time.deltaTime;

        UpdateColor();

        m_isAvailable = m_timeSinceLastUsed > cooldown_time && !Helpers.ArePhasesOpposite(phase_type, WaveManager.Instance.CurrentPhase.phase_type);
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
