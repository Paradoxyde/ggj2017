using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFiringBehavior : MonoBehaviour
{
    public Target target = Target.forward;
    public GameObject projectile;
    public float delay_between_shots = 3.0f;
    public float player_target_range = 15.0f;

    float m_shotCooldown = 0.0f;
    bool m_inRange = true;

    public enum Target
    {
        forward,
        player
    }

	void Start ()
    {
		
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
                
                if ((playerPos - localPos).sqrMagnitude < player_target_range * player_target_range)
                {
                    m_inRange = true;
                    float angleToPlayer = Mathf.Atan2(playerPos.y - localPos.y, playerPos.x - localPos.x);
                    Vector3 eulerAngles = transform.eulerAngles;
                    eulerAngles.z = angleToPlayer * Mathf.Rad2Deg - 90.0f;
                    transform.eulerAngles = eulerAngles;
                }
                else
                {
                    m_inRange = false;
                }
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

                GameObject.Instantiate(projectile, transform.position, transform.rotation);
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
    }
}
