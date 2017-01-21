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
        Phase phase = WaveManager.Instance.CurrentPhase;
        m_Light.color = phase.LightColor;
    }

    public void Update()
    {
        float remainingPhaseTime = WaveManager.Instance.PhaseRemaingTime;
        if (remainingPhaseTime <= m_MaximumTransitionTime)
        {
            Phase nextPhase = WaveManager.Instance.NextPhase;

            Color c0 = m_Light.color, c1 = nextPhase.LightColor;
            c0.a = Mathf.SmoothDamp(c0.a, c1.a, ref m_SmoothVelocityA, remainingPhaseTime);
            c0.r = Mathf.SmoothDamp(c0.r, c1.r, ref m_SmoothVelocityR, remainingPhaseTime);
            c0.g = Mathf.SmoothDamp(c0.g, c1.g, ref m_SmoothVelocityG, remainingPhaseTime);
            c0.b = Mathf.SmoothDamp(c0.b, c1.b, ref m_SmoothVelocityB, remainingPhaseTime);

            m_Light.color = c0;
        }
    }

    public override void OnPhaseChanged(Phase phase)
    {
        m_Light.color = phase.LightColor;
    }
}
