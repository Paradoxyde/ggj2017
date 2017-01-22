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
    public bool UseDampener = false;
    public SoundDampener SoundDampener = new SoundDampener();
    public ManagedClip GetRandomClip() { int clipIndex = Clips.Count > 1 ? Random.Range(0, Clips.Count) : 0; return Clips[clipIndex]; }

    public SFXPreset()
    {
        //SFXManager.Instance.Register(this);
    }

    public void Update()
    {
        SoundDampener.regenVolume();
    }
}
