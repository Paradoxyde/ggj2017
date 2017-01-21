using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    public double m_PeriodInSeconds = 5.0f;
    public List<AudioClip> m_AudioClips;
    public List<Phase> m_Phases;

    public Phase CurrentPhase { get { return m_Phases[m_PhaseIndex]; } }

    private int m_AudioClipIndex = 0;
    private int m_PhaseIndex = 0;
    private List<WaveComponent> m_WaveComponents;
    private double m_ElapsedTime;

    private AudioSource m_AudioSourceCache;

    public void Register(WaveComponent waveComponent) { m_WaveComponents.Add(waveComponent); }

    void Awake()
    {
        Debug.Assert(!Instance);
        Instance = this;
        m_WaveComponents = new List<WaveComponent>();
    }

    void Start()
    {
        m_PhaseIndex = 0;
        m_AudioClipIndex = -1;

        m_AudioSourceCache = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Play audio clip.
        if (!m_AudioSourceCache.isPlaying)
        {
            m_AudioClipIndex++;
            if (m_AudioClipIndex >= m_AudioClips.Count)
                m_AudioClipIndex = 0;

            m_AudioSourceCache.clip = m_AudioClips[m_AudioClipIndex];
            m_AudioSourceCache.Play();
        }

        // Update period.
        m_ElapsedTime += Time.deltaTime;

        if (m_ElapsedTime >= m_PeriodInSeconds)
        {
            m_ElapsedTime = m_ElapsedTime % m_PeriodInSeconds;

            m_PhaseIndex++;
            if (m_PhaseIndex >= m_Phases.Count)
                m_PhaseIndex = 0;

            foreach (WaveComponent waveComponent in m_WaveComponents)
			{
                waveComponent.OnPhaseChanged(m_Phases[m_PhaseIndex]);
            }

            Debug.Log("Phase changed!");
        }
    }
}
