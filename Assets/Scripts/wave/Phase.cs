using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PhaseType
{
    Red,
    Blue,
    Neutral
}

[System.Serializable]
public class Phase
{
    public Color LightColor = Color.white;

    public float PeriodInSeconds = 5.0f;

    public PhaseType phase_type;
}
