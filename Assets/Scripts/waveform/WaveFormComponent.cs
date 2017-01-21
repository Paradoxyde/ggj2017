using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WaveFormComponent : MonoBehaviour
{
    public float MinimumFrequency;
    public float MaximumFrequency;

    protected virtual void Start() { WaveFormManager.Instance.Register(this); }
    public virtual void UpdateWaveMean(float mean) { }
}
