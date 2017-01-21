using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPhaseComponent : WaveComponent
{
    void Awake()
    {
        Helpers.MakeGrey(gameObject);
    }

    void Update()
    {

    }

    public override void OnPhaseChanged(Phase phase)
    {
        switch(phase.phase_type)
        {
            case PhaseType.Blue:
                Helpers.MakeBlue(gameObject);
                break;
            case PhaseType.Red:
                Helpers.MakeRed(gameObject);
                break;
            case PhaseType.Neutral:
                Helpers.MakeGrey(gameObject);
                break;
        }
    }
}
