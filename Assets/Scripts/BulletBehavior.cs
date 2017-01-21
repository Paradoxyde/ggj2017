using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    public float speed = 8.0f;
    PhaseType m_phaseType = PhaseType.Neutral;

    public void SetPhaseType(PhaseType phaseType)
    {
        m_phaseType = phaseType;

        if (m_phaseType == PhaseType.Red)
        {
            Helpers.MakeRed(gameObject);
        }
        else if (m_phaseType == PhaseType.Blue)
        {
            Helpers.MakeBlue(gameObject);
        }
    }

    void Start ()
    {
    }

	void Update ()
    {
        transform.position += transform.rotation * Vector2.up * speed * Time.deltaTime;
	}

    private void OnTriggerEnter2D(Collider2D collider)
    {
        bool doDestroy = false;

        if (collider.gameObject.layer == 8 || collider.gameObject.layer == 10)
        {
            doDestroy = true;
        }
        else if (collider.CompareTag("Player") && Helpers.ArePhasesOpposite(m_phaseType, WaveManager.Instance.CurrentPhase.phase_type))
        {
            GameObject go = GameObject.FindGameObjectWithTag("Player");

            if (go)
            {
                PlatformingEntitiesManager pem = go.GetComponent<PlatformingEntitiesManager>();
                pem.OnPlayerDied();
            }
            doDestroy = true;
        }
        
        if (doDestroy)
        {
            GameObject.Destroy(gameObject);
        }
    }
}
