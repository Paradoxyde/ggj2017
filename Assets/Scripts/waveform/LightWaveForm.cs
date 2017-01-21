using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightWaveForm : WaveFormComponent
{
    public float m_MinimumSize = 2.0f;
    public float m_MaximumSize = 15.0f;

    public float m_SmoothTime = 0.2f;

    private Light m_Light;

    private float m_SmoothVelocity = 0.0f;
    private float m_MeanAmplitude = 0.0f;

    protected override void Start()
    {
        base.Start();
        m_Light = GetComponent<Light>();
    }

    public override void UpdateWaveMean(float mean)
    {
        m_MeanAmplitude = (m_MeanAmplitude + mean) * 0.5f;

        float log = Mathf.Log(mean / m_MeanAmplitude);
        float size = Mathf.Lerp(m_MinimumSize, m_MaximumSize, log);
        if (size < m_Light.range)
            size = Mathf.SmoothDamp(m_Light.range, size, ref m_SmoothVelocity, m_SmoothTime);

        m_Light.range = size;
    }
}
