using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    public List<AudioClip> m_AudioClips;
    public List<Phase> m_Phases;

    public Phase CurrentPhase { get { return m_Phases[m_PhaseIndex]; } }
    public float SinPhase { get; private set; }

    private int m_AudioClipIndex = 0;
    private int m_PhaseIndex = 0;
    private float m_PhaseElapsedTime = 0.0f;
    private float m_CycleElapsedTime = 0.0f;
    private float m_CycleTimeInSeconds = 0.0f;
    private List<WaveComponent> m_WaveComponents;

    private AudioSource m_AudioSourceCache;

    public void Register(WaveComponent waveComponent) { m_WaveComponents.Add(waveComponent); }

    void Awake()
    {
        Debug.Assert(!Instance);
        Instance = this;
        m_WaveComponents = new List<WaveComponent>();

        m_CycleTimeInSeconds = 0.0f;
        foreach (Phase phase in m_Phases)
            m_CycleTimeInSeconds += phase.PeriodInSeconds;
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

        Phase currentPhase = CurrentPhase;

        // Update period.
        m_PhaseElapsedTime += Time.deltaTime;
        m_CycleElapsedTime += Time.deltaTime;

        if (m_CycleElapsedTime > m_CycleTimeInSeconds)
            m_CycleElapsedTime -= m_CycleTimeInSeconds;

        float radElapsed = Mathf.PI * 2.0f / m_CycleTimeInSeconds * m_CycleElapsedTime;
        SinPhase = Mathf.Sin(radElapsed);

        if (m_PhaseElapsedTime >= currentPhase.PeriodInSeconds)
        {
            m_PhaseElapsedTime = m_PhaseElapsedTime % currentPhase.PeriodInSeconds;

            m_PhaseIndex++;
            if (m_PhaseIndex >= m_Phases.Count)
            {
                m_PhaseIndex = 0;
            }

            foreach (WaveComponent waveComponent in m_WaveComponents)
			{
                waveComponent.OnPhaseChanged(m_Phases[m_PhaseIndex]);
            }

            Debug.Log("Phase changed!");
        }
    }
}
