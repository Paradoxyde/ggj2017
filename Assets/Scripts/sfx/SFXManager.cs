using UnityEngine;
using System.Collections.Generic;
using System;

public class SFXManager : MonoBehaviour
{
    public AudioSource m_AudioSource;
    
    private bool IsMuted = false;
    public float SFXVolume = 1f;
    
    public static SFXManager Instance { get; private set; }

    private List<SFXPreset> m_SFXPresets = new List<SFXPreset>();
    public void Register(SFXPreset preset) { m_SFXPresets.Add(preset); }

    
    void Awake()
    {
        Debug.Assert(!Instance);

        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    void Update()
    {
        SFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f);
        foreach (var preset in m_SFXPresets)
            preset.Update();
    }

    public void PlaySound(SFXPreset preset)
    {
        if (IsMuted)
            return;

        ManagedClip clip = preset.GetRandomClip();
        m_AudioSource.volume = SFXVolume * clip.Volume;
        if (preset.UseDampener)
            m_AudioSource.volume *= preset.SoundDampener.getNextVolume();
        m_AudioSource.clip = clip.AudioClip;
        m_AudioSource.Play();
    }
}

static class SFXExtension
{
    public static void PlayNow(this SFXPreset preset)
    {
        if (preset != null)
            SFXManager.Instance.PlaySound(preset);
    }
}