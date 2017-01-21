﻿using System.Collections;
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

        Renderer renderer = GetComponent<Renderer>();
        renderer.enabled = active;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("playerkillintersect"))
        {
            Debug.Log("Kill player!");
        }
    }
}