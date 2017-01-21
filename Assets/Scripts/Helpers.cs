using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    public static bool ArePhasesOpposite(PhaseType phaseA, PhaseType phaseB)
    {
        return (phaseA == PhaseType.Red && phaseB == PhaseType.Blue)
            || (phaseA == PhaseType.Blue && phaseB == PhaseType.Red);
    }
}
