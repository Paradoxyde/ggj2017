using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColoredPlatform : MonoBehaviour
{
    public PhaseType phase_type = PhaseType.Red;

    bool m_isActive = true;
    Collider2D m_collider;

    void Start()
    {
        m_collider = GetComponent<Collider2D>();

        if (phase_type == PhaseType.Red)
        {
            Helpers.MakeRed(gameObject);
        }
        else
        {
            Helpers.MakeBlue(gameObject);
        }
    }
	
	void Update()
    {
        bool isActive = !Helpers.ArePhasesOpposite(phase_type, WaveManager.Instance.CurrentPhase.phase_type);

        if (m_isActive != isActive)
        {
            m_isActive = isActive;
            OnActiveChanged(m_isActive);
        }
    }

    void OnActiveChanged(bool active)
    {
        m_collider.enabled = active;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("playerkillintersect"))
        {
            GameObject go = GameObject.FindGameObjectWithTag("Player");

            if (go)
            {
                PlatformingEntitiesManager pem = go.GetComponent<PlatformingEntitiesManager>();
                pem.OnPlayerDied();
            }
        }
    }
}
