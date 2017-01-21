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

    public PhaseType phase_type;
}
