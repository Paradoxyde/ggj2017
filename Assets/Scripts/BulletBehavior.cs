using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    public float speed = 8.0f;
    PhaseType m_phaseType = PhaseType.Neutral;

    public Transform red_particles;
    public Transform blue_particles;
    public Transform neutral_particles;
    public SFXPreset sound_bullet_hit_player;
    public SFXPreset sound_bullet_hit_wall;

    Transform childParticles;

    public void SetPhaseType(PhaseType phaseType)
    {
        m_phaseType = phaseType;

        if (m_phaseType == PhaseType.Red)
        {
            Helpers.MakeRed(gameObject);
            SetParticles(red_particles);
        }
        else if (m_phaseType == PhaseType.Blue)
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

    void Start()
    {
    }

    void Update()
    {
        transform.position += transform.rotation * Vector2.up * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        bool doDestroy = false;
        bool hitPlayer = false;

        GameObject go = GameObject.FindGameObjectWithTag("Player");


        if (collider.gameObject.layer == 8 || collider.gameObject.layer == 10)
        {
            doDestroy = true;
        }
        else if (collider.CompareTag("Player") && m_phaseType != WaveManager.Instance.CurrentPhase.phase_type)
        {
            if (go)
            {
                PlatformingEntitiesManager pem = go.GetComponent<PlatformingEntitiesManager>();
                pem.OnPlayerDied();
            }
            doDestroy = true;
            hitPlayer = true;
        }

        if (doDestroy)
        {
            if (go != null && (transform.position - go.transform.position).sqrMagnitude < 400)
            {
                SFXExtension.PlayNow(hitPlayer ? sound_bullet_hit_player : sound_bullet_hit_wall);
            }
            GameObject.Destroy(gameObject);
        }
    }
}
