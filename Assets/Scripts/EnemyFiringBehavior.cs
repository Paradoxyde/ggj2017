using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFiringBehavior : MonoBehaviour
{
    public bool invisible_at_runtime = true;
    public Target target = Target.forward;
    public GameObject projectile;
    public float delay_between_shots = 3.0f;
    public float player_target_range = 15.0f;
    public PhaseType phase_type = PhaseType.Neutral;
    public SFXPreset sound_bullet_fire;

    public Transform red_particles;
    public Transform blue_particles;
    public Transform neutral_particles;
    Transform childParticles;

    float m_shotCooldown = 0.0f;
    bool m_inRange = true;

    public enum Target
    {
        forward,
        player
    }

	void Start ()
    {
        m_shotCooldown = Random.Range(0, delay_between_shots);
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

        childParticles = GameObject.Instantiate(ps, transform.position, ps.rotation);
        
        Vector3 pos = childParticles.position;
        pos += transform.rotation * new Vector3(0, -1.5f, 0);
        childParticles.position = pos;
        childParticles.rotation = Quaternion.LookRotation(transform.position - childParticles.position, new Vector3(0, 0, -1));
        childParticles.SetParent(transform);
    }

    void Update ()
    {
        if (target == Target.player)
        {
            GameObject go = GameObject.FindGameObjectWithTag("Player");

            if (go)
            {
                Vector3 playerPos = go.transform.position;
                Vector3 localPos = transform.position;

                m_inRange = (playerPos - localPos).sqrMagnitude < player_target_range * player_target_range;

                float angleToPlayer = Mathf.Atan2(playerPos.y - localPos.y, playerPos.x - localPos.x);
                Vector3 eulerAngles = transform.eulerAngles;
                eulerAngles.z = angleToPlayer * Mathf.Rad2Deg - 90.0f;
                transform.eulerAngles = eulerAngles;
            }
        }
        else
        {
            m_inRange = true;
        }

        if (m_inRange)
        {
            m_shotCooldown += Time.deltaTime;

            if (m_shotCooldown > delay_between_shots && projectile != null)
            {
                m_shotCooldown -= delay_between_shots;

                GameObject bullet = GameObject.Instantiate(projectile, transform.position, transform.rotation);
                if (bullet)
                {
                    BulletBehavior bb = bullet.GetComponent<BulletBehavior>();
                    bb.SetPhaseType(phase_type);

                    GameObject go = GameObject.FindGameObjectWithTag("Player");

                    if (go != null && (bullet.transform.position - go.transform.position).sqrMagnitude < 400)
                    {
                        SFXExtension.PlayNow(sound_bullet_fire);
                    }
                }
            }
        }
        else
        {
            m_shotCooldown = 0.0f;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, player_target_range);

        if (target == Target.forward)
        {
            Vector3 offsetV = new Vector3(0.0f, 0.2f, 0.0f);
            Vector3 offsetH = new Vector3(0.2f, 0.0f, 0.0f);
            Gizmos.DrawLine(transform.position + offsetV + offsetH, transform.position + transform.rotation * Vector2.up);
            Gizmos.DrawLine(transform.position + offsetV - offsetH, transform.position + transform.rotation * Vector2.up);
            Gizmos.DrawLine(transform.position - offsetV + offsetH, transform.position + transform.rotation * Vector2.up);
            Gizmos.DrawLine(transform.position - offsetV - offsetH, transform.position + transform.rotation * Vector2.up);
        }

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
