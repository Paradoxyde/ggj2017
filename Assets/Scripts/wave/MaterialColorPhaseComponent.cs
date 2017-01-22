using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialColorPhaseComponent : WaveComponent
{
    private MeshRenderer m_MeshRenderer;
    private float m_SmoothVelocityA = 0.0f;
    private float m_SmoothVelocityR = 0.0f;
    private float m_SmoothVelocityG = 0.0f;
    private float m_SmoothVelocityB = 0.0f;

    public float m_MaximumTransitionTime = 0.4f;
    
    public override void Start()
    {
        base.Start();
        m_MeshRenderer = GetComponent<MeshRenderer>();
        m_MeshRenderer.material.color = WaveManager.Instance.CurrentColor;
    }

    public void Update()
    {
        m_MeshRenderer.material.color = WaveManager.Instance.CurrentColor;
    }

    public override void OnPhaseChanged(Phase phase)
    {
        m_MeshRenderer.material.color = WaveManager.Instance.CurrentColor;
    }
}
