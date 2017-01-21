using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WaveFormManager : MonoBehaviour
{
    private static float[] m_Coeficients = new float[] 
{
0.000055f,
0.000063f,
0.000073f,
0.000083f,
0.000093f,
0.000104f,
0.000115f,
0.000127f,
0.000140f,
0.000152f,
0.000166f,
0.000180f,
0.000194f,
0.000209f,
0.000224f,
0.000240f,
0.000256f,
0.000272f,
0.000289f,
0.000305f,
0.000323f,
0.000340f,
0.000358f,
0.000376f,
0.000393f,
0.000412f,
0.000430f,
0.000448f,
0.000466f,
0.000484f,
0.000502f,
0.000520f,
0.000538f,
0.000556f,
0.000573f,
0.000590f,
0.000607f,
0.000623f,
0.000639f,
0.000655f,
0.000670f,
0.000685f,
0.000699f,
0.000713f,
0.000726f,
0.000738f,
0.000750f,
0.000761f,
0.000771f,
0.000781f,
0.000790f,
0.000798f,
0.000805f,
0.000812f,
0.000817f,
0.000822f,
0.000826f,
0.000829f,
0.000832f,
0.000833f,
0.000833f,
0.000833f,
0.000832f,
0.000829f,
0.000826f,
0.000822f,
0.000817f,
0.000812f,
0.000805f,
0.000798f,
0.000790f,
0.000781f,
0.000771f,
0.000761f,
0.000750f,
0.000738f,
0.000726f,
0.000713f,
0.000699f,
0.000685f,
0.000670f,
0.000655f,
0.000639f,
0.000623f,
0.000607f,
0.000590f,
0.000573f,
0.000556f,
0.000538f,
0.000520f,
0.000502f,
0.000484f,
0.000466f,
0.000448f,
0.000430f,
0.000412f,
0.000393f,
0.000376f,
0.000358f,
0.000340f,
0.000323f,
0.000305f,
0.000289f,
0.000272f,
0.000256f,
0.000240f,
0.000224f,
0.000209f,
0.000194f,
0.000180f,
0.000166f,
0.000152f,
0.000140f,
0.000127f,
0.000115f,
0.000104f,
0.000093f,
0.000083f,
0.000073f,
0.000063f,
0.000055f};


    public static WaveFormManager Instance { get; private set; }

    public int m_SampleSize = 2048;
    
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
        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
        
        foreach (WaveFormComponent component in m_Components)
        {
            int n1 = (int)Mathf.Floor(component.MinimumFrequency * m_SamplesPerFrequency);
            int n2 = (int)Mathf.Floor(component.MaximumFrequency * m_SamplesPerFrequency);
            if (n1 == n2)
                continue;

            float mean = 0.0f;
            for (int i = n1; i < n2; ++i)
                mean += spectrum[i];

            mean /= (n2 - n1);
            component.UpdateWaveMean(mean);
        }
        
        float[] audioOutput = new float[800];
        AudioListener.GetOutputData(audioOutput, 0);

        for (int i = 0; i < audioOutput.Length; ++i)
            audioOutput[i] = Mathf.Abs(audioOutput[i]);

        audioOutput = FIR(m_Coeficients, audioOutput);
        float audioOutputValue = audioOutput.Average();
        
        foreach (WaveFormComponent component in m_Components)
        {
            component.UpdateAudioOutput(audioOutputValue);
        }
    }

    private static float[] FIR(float[] coeficients, float[] audioOutput)
    {
        int M = coeficients.Length;
        int n = audioOutput.Length;
        //y[n]=b0x[n]+b1x[n-1]+....bmx[n-M]
        var y = new float[n];
        for (int yi = 0; yi < n; yi++)
        {
            float t = 0.0f;
            for (int bi = M - 1; bi >= 0; bi--)
            {
                if (yi - bi < 0) continue;

                t += coeficients[bi] * audioOutput[yi - bi];
            }
            y[yi] = t;
        }
        return y;
    }
}
