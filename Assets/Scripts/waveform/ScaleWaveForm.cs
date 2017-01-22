using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleWaveForm : WaveFormComponent
{
    public float m_MinimumSize = 0.1f;
    public float m_MaximumSize = 2.0f;

    public float m_SmoothTime = 0.2f;

    private Transform m_Transform;

    private float m_SmoothVelocity = 0.0f;
    private float m_MeanAmplitude = 0.0f;

    protected override void Start()
    {
        base.Start();
        m_Transform = GetComponent<Transform>();
    }

    public override void UpdateWaveMean(float mean)
    {
        m_MeanAmplitude = (m_MeanAmplitude + mean) * 0.5f;

        float log = Mathf.Log(mean / m_MeanAmplitude);
        float size = Mathf.Lerp(m_MinimumSize, m_MaximumSize, log);
        if (size < m_Transform.localScale.x)
            size = Mathf.SmoothDamp(m_Transform.localScale.x, size, ref m_SmoothVelocity, m_SmoothTime);

        m_Transform.localScale = new Vector3(size, size, size);
    }
}
