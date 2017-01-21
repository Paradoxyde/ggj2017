using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum SFXTransitionMode
{
    None,
    EaseIn,
    EaseOut,
    EaseInAndOut
}

[System.Serializable]
public class SFXPreset
{
    public List<ManagedClip> Clips = new List<ManagedClip>();
    public SoundDampener SoundDampener = new SoundDampener();
    public ManagedClip GetRandomClip() { int clipIndex = Clips.Count > 1 ? Random.Range(0, Clips.Count) : 0; return Clips[clipIndex]; }
    
    public float FadeInTime = 0.0f;
    public float FadeOutTime = 0.0f;
    public bool IsExclusive = false;

    public void Update()
    {
        SoundDampener.regenVolume();
    }
}
