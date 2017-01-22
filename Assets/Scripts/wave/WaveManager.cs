using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    public List<AudioClip> m_AudioClips;
    public List<Phase> Phases;

    public Phase CurrentPhase { get { return Phases[m_PhaseIndex]; } }
    public Phase NextPhase { get { int i = m_PhaseIndex + 1; if (i >= Phases.Count) i = 0; return Phases[i]; } }
    public float CycleSinValue { get; private set; }
    public float CycleAngle { get; private set; }
    public float CycleProgress { get; private set; }
    public float PhaseProgress { get { return Mathf.Min(m_PhaseElapsedTime / CurrentPhase.PeriodInSeconds, 1.0f); } }
    public float PhaseRemaingTime { get { return Mathf.Max(CurrentPhase.PeriodInSeconds - m_PhaseElapsedTime, 0.0f); } }
    public float PhaseElapsedTime { get { return m_PhaseElapsedTime; } }

    public float ColorTransitionRatio = 0.05f;

    private int m_AudioClipIndex = 0;
    private int m_PhaseIndex = 0;
    private float m_PhaseElapsedTime = 0.0f;
    private float m_CycleElapsedTime = 0.0f;
    private float m_CycleTimeInSeconds = 0.0f;
    private List<WaveComponent> m_WaveComponents;
    public float music_volume = 0.5f;

    private AudioSource m_AudioSourceCache;

    public void Register(WaveComponent waveComponent) { m_WaveComponents.Add(waveComponent); }

    public Color GetColorAt(float cycleRatio)
    {
        float tolerance = ColorTransitionRatio;
        Color color;

        if (IsWithinTolerance(cycleRatio, 0.0625f, tolerance))
            color = FadeTo(GetPhaseColor(3), GetPhaseColor(0), cycleRatio, 0.0625f, tolerance);
        else if (IsWithinTolerance(cycleRatio, 0.4375f, tolerance))
            color = FadeTo(GetPhaseColor(0), GetPhaseColor(1), cycleRatio, 0.4375f, tolerance);
        else if (IsWithinTolerance(cycleRatio, 0.5625f, tolerance))
            color = FadeTo(GetPhaseColor(1), GetPhaseColor(2), cycleRatio, 0.5625f, tolerance);
        else if (IsWithinTolerance(cycleRatio, 0.9375f, tolerance))
            color = FadeTo(GetPhaseColor(2), GetPhaseColor(3), cycleRatio, 0.9375f, tolerance);
        else if (cycleRatio < 0.0625f || cycleRatio > 0.9375f)
            color = GetPhaseColor(3);
        else if (cycleRatio > 0.4375f && cycleRatio < 0.5625f)
            color = GetPhaseColor(1);
        else if (cycleRatio < 0.5f)
            color = GetPhaseColor(0);
        else
            color = GetPhaseColor(2);

        return color;
    }
    
    bool IsWithinTolerance(float x, float point, float tolerance)
    {
        return x < (point + tolerance) && x > (point - tolerance);
    }

    Color FadeTo(Color from, Color to, float v, float point, float tolerance)
    {
        float t = (v - point + tolerance) / (tolerance * 2.0f);
        return Color.Lerp(from, to, t);
    }

    Color GetPhaseColor(int index)
    {
        return Phases[index].LightColor;
    }

    public Color CurrentColor { get { return GetColorAt(CycleProgress); } }

    void Awake()
    {
        Debug.Assert(!Instance);
        Instance = this;
        m_WaveComponents = new List<WaveComponent>();

        m_CycleTimeInSeconds = 0.0f;
        foreach (Phase phase in Phases)
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
        m_AudioSourceCache.volume = PlayerPrefs.GetFloat("MusicVolume", music_volume);

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

        CycleProgress = m_CycleElapsedTime / m_CycleTimeInSeconds;
        CycleAngle = Mathf.PI * 2.0f * CycleProgress;
        CycleSinValue = Mathf.Sin(CycleAngle);

        if (m_PhaseElapsedTime >= currentPhase.PeriodInSeconds)
        {
            m_PhaseElapsedTime = m_PhaseElapsedTime % currentPhase.PeriodInSeconds;

            m_PhaseIndex++;
            if (m_PhaseIndex >= Phases.Count)
            {
                m_PhaseIndex = 0;
            }

            foreach (WaveComponent waveComponent in m_WaveComponents)
			{
                waveComponent.OnPhaseChanged(Phases[m_PhaseIndex]);
            }
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("LDstuff");
    }
}
