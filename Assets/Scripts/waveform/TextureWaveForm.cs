using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureWaveForm : WaveFormComponent
{
    public Color m_StartColor = Color.yellow;
    public Color m_EndColor = Color.red;
    public float[] m_SampleHistory = new float[200];
    public float m_StartLineWidth = 0.1f;
    public float m_EndLineWidth = 0.1f;
    public float m_MaximumWidth = 10.0f;
    public float m_AmplitudeMultiplier = 2.0f;
    public float m_SmoothTime = 0.2f;
    public float m_DbFilter = 1.0f;

    //public int m_LeadingSlaveSamplesCount = 20;

    public bool m_UseAmplitudeAbsolute = true;
    public bool m_UseAmplitudeDropDampening = false;

    private float m_MeanAmplitude = 0.0f;
    private int m_WriteHead = 0;
    private float m_SmoothVelocity = 0.0f;

    protected override void Start()
    {
        base.Start();
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        if (!lineRenderer.material)
            lineRenderer.material = new Material(Shader.Find("Particles/Additive"));

        lineRenderer.startColor = m_StartColor;
        lineRenderer.endColor = m_EndColor;
        lineRenderer.startWidth = m_StartLineWidth;
        lineRenderer.endWidth = m_EndLineWidth;
        lineRenderer.numPositions = m_SampleHistory.Length;
    }
    
    void Update()
    {
        float widthPerEntry = m_MaximumWidth / m_SampleHistory.Length;

        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        int rendererIndex = m_SampleHistory.Length - 1;
        int sampleIndex = m_WriteHead + 1;
        if (sampleIndex >= m_SampleHistory.Length)
            sampleIndex = 0;

        while (rendererIndex > 0)
        {
            Vector3 pos = new Vector3(rendererIndex * widthPerEntry, m_SampleHistory[sampleIndex], 0);
            lineRenderer.SetPosition(rendererIndex, pos);

            ++sampleIndex;
            if (sampleIndex >= m_SampleHistory.Length)
                sampleIndex = 0;

            rendererIndex--;
        }
    }

    public override void UpdateWaveMean(float mean)
    {
        m_MeanAmplitude = (m_MeanAmplitude + mean) * 0.5f;
        m_SampleHistory[m_WriteHead] = Mathf.Log(mean / m_MeanAmplitude) * m_AmplitudeMultiplier;
        float abs = Mathf.Abs(m_SampleHistory[m_WriteHead]);

        if (abs < m_DbFilter)
        {
            m_SampleHistory[m_WriteHead] = 0.0f;
        }
        else
        {
            if (m_UseAmplitudeAbsolute)
                m_SampleHistory[m_WriteHead] = abs;

            int previousEntry = m_WriteHead - 1;
            if (previousEntry < 0)
                previousEntry = m_SampleHistory.Length - 1;

            if (m_UseAmplitudeDropDampening && m_SampleHistory[m_WriteHead] < m_SampleHistory[previousEntry])
                m_SampleHistory[m_WriteHead] = Mathf.SmoothDamp(m_SampleHistory[previousEntry], m_SampleHistory[m_WriteHead], ref m_SmoothVelocity, m_SmoothTime);
        }
        
        ++m_WriteHead;
        if (m_WriteHead >= m_SampleHistory.Length)
            m_WriteHead = 0;
    }

}
