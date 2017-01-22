using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillVolume : MonoBehaviour
{
    public bool invisible_at_runtime = true;
    public PhaseType phase_type = PhaseType.Neutral;
    public SFXPreset sound_kill_by_spikes;

    bool m_playerContact = false;

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
    }

    private void Update()
    {
        if (m_playerContact)
        {
            GameObject go = GameObject.FindGameObjectWithTag("Player");

            if (go)
            {
                if (phase_type == PhaseType.Neutral || phase_type == WaveManager.Instance.CurrentPhase.phase_type)
                {
                    PlatformingEntitiesManager pem = go.GetComponent<PlatformingEntitiesManager>();
                    pem.OnPlayerDied();
                    SFXExtension.PlayNow(sound_kill_by_spikes);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            GameObject go = GameObject.FindGameObjectWithTag("Player");

            if (go)
            {
                m_playerContact = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            GameObject go = GameObject.FindGameObjectWithTag("Player");

            if (go)
            {
                m_playerContact = false;
            }
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
        Gizmos.DrawCube(transform.position + new Vector3(0.0f, 0.0f, 0.0f), new Vector3(1.0f, 1.0f, 5.0f));
    }
}
