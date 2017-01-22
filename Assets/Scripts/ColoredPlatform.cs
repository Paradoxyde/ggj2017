using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColoredPlatform : MonoBehaviour
{
    public PhaseType phase_type = PhaseType.Red;

    public float lowAlpha = 0.15f;
    public float fadeDamping = 2f;
    public Material opaqueMat;
    public Material fadeMat;
    public Renderer renderer;
    private Texture m_texture;

    bool m_isActive = true;
    Collider2D m_collider;
    public SFXPreset player_killed_by_color_platform;
    Renderer m_renderer;

    void Start()
    {
        m_collider = GetComponent<Collider2D>();

        if (renderer)
        {
            m_renderer = renderer;
            m_texture = m_renderer.material.mainTexture;
        }
        else
            m_renderer = GetComponent<Renderer>();

        ApplyColor();
    }
	
	void Update()
    {
        bool isActive = !Helpers.ArePhasesOpposite(phase_type, WaveManager.Instance.CurrentPhase.phase_type);
        
        if (m_isActive != isActive)
        {
            m_isActive = isActive;
            OnActiveChanged(m_isActive);

            if (isActive)
            {
                m_renderer.material = opaqueMat;
            }
            else
            {
                m_renderer.material = fadeMat;
            }
            m_renderer.material.mainTexture = m_texture;

            ApplyColor();
        }

        float targetAlpha = isActive ? 1f : lowAlpha;
        Color color = m_renderer.material.color;
        color.a = Mathf.Lerp(color.a, targetAlpha, fadeDamping * Time.deltaTime);
        m_renderer.material.color = color;
    }

    void ApplyColor()
    {
        if (phase_type == PhaseType.Red)
        {
            Helpers.MakeRed(gameObject);
        }
        else if (phase_type == PhaseType.Blue)
        {
            Helpers.MakeBlue(gameObject);
        }
    }

    void OnActiveChanged(bool active)
    {
        m_collider.enabled = active;

        if (active)
        {
            GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
            Transform childTran = playerGO.transform.FindChild("killintersectvolume");

            Collider2D thisCol = GetComponent<Collider2D>();

            Vector3 playerPos = playerGO.transform.position;
            Collider2D[] colliders = Physics2D.OverlapCapsuleAll(playerPos, new Vector2(0.85f, 1.95f), CapsuleDirection2D.Vertical, 0.0f);

            foreach (Collider2D col in colliders)
            {
                if (col == thisCol)
                {
                    PlatformingEntitiesManager pem = playerGO.GetComponent<PlatformingEntitiesManager>();
                    pem.OnPlayerDied();
                    SFXExtension.PlayNow(player_killed_by_color_platform);
                    return;
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
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
