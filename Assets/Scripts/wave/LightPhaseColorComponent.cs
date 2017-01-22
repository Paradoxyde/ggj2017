using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightPhaseColorComponent : WaveComponent
{
    private Light m_Light;
    private float m_SmoothVelocityA = 0.0f;
    private float m_SmoothVelocityR = 0.0f;
    private float m_SmoothVelocityG = 0.0f;
    private float m_SmoothVelocityB = 0.0f;

    public float m_MaximumTransitionTime = 0.4f;

    public override void Start()
    {
        base.Start();
        m_Light = GetComponent<Light>();
        m_Light.color = WaveManager.Instance.CurrentColor;
    }

    public void Update()
    {
        m_Light.color = WaveManager.Instance.CurrentColor;
    }

    public override void OnPhaseChanged(Phase phase)
    {
        m_Light.color = WaveManager.Instance.CurrentColor;
    }
}
