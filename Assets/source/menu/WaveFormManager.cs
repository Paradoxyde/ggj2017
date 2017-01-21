using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveFormManager : MonoBehaviour
{
    public static WaveFormManager Instance { get; private set; }

    public int m_SampleSize = 256;
    
    private List<WaveFormComponent> m_Components = new List<WaveFormComponent>();

    private float m_MeanAmplitude = 0.0f;
    private float m_MaximumFrequency;
    private float m_SamplesPerFrequency;
    
    public void Register(WaveFormComponent waveFormComponent) { m_Components.Add(waveFormComponent); }

    void Awake()
    {
        Debug.Assert(!Instance);
        Instance = this;
    }

    void Start()
    {
        m_MaximumFrequency = AudioSettings.outputSampleRate / 2.0f;
        m_SamplesPerFrequency = m_SampleSize / m_MaximumFrequency;

    }
    
    void Update()
    {
        float[] spectrum = new float[m_SampleSize * 2];
        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Hamming);
        
        //for (int i = 0; i < m_SampleSize; ++i)
        //    m_MeanAmplitude += spectrum[i];

        //m_MeanAmplitude /= m_SampleSize;
            
        foreach (WaveFormComponent component in m_Components)
        {
            int n1 = (int)Mathf.Floor(component.MinimumFrequnecy * m_SamplesPerFrequency);
            int n2 = (int)Mathf.Floor(component.MaximumFrequnecy * m_SamplesPerFrequency);
            if (n1 == n2)
                continue;

            float mean = 0.0f;
            for (int i = n1; i < n2; ++i)
                mean += spectrum[i];

            mean /= (n2 - n1);
            component.Resize(mean/* / m_MeanAmplitude*/);
        }
    }
}
