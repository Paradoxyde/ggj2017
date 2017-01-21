using UnityEngine;
using System.Collections.Generic;
using System;

enum SFXPresetType
{
    Ambiance,
    SFXPresetCount
}

public class SFXManager : MonoBehaviour
{
    public AudioSource m_AudioSource0;
    public AudioSource m_AusioSource1;

    public SFXPreset m_PlayingPreset0;
    public SFXPreset m_PlayingPreset1;

    public ManagedClip m_PlayingClip0;
    public ManagedClip m_PlayingClip1;

    //public SFXPreset[] m_SFXPresets = new SFXPresetCount[(int)(SFXPresetType.SFXPresetCount)];

    private bool IsMuted = false;
    public float SFXMasterVolume = 1f;

    public static SFXManager Instance { get; private set; }

    void Awake()
    {
        Debug.Assert(!Instance);

        if (Instance != null && Instance != this) {
            Destroy(this.gameObject);
            return;
        } else {
            Instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    void Update()
    {
        if (m_AudioSource0.isPlaying)
        {
            //if (m_PlayingPreset0.FadeInTime > m_AudioSource0)
            //{

            //}
        }

        //foreach (var preset in m_SFXPresets)
        //    preset.Update();
    }
    
    private float GetDampenerModifier(SoundDampener dampener)
    {
        return dampener != null ? dampener.getNextVolume() : 1.0f;
    }
}