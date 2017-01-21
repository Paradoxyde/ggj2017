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
		
	}
	
	void Update ()
    {
        if (!m_registered) TryRegister();

        m_timeSinceLastUsed += Time.deltaTime;

        bool isAvailable = m_timeSinceLastUsed > cooldown_time && !Helpers.ArePhasesOpposite(phase_type, WaveManager.Instance.CurrentPhase.phase_type);

        if (m_isAvailable != isAvailable)
        {
            m_isAvailable = isAvailable;
            OnAvailableChanged(m_isAvailable);
        }
    }

    public void OnAvailableChanged(bool available)
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.enabled = available;
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
}
