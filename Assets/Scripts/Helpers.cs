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

    public static void MakeBlue(GameObject go)
    {
        SetColor(go, Color.blue);
    }
    public static void MakeRed(GameObject go)
    {
        SetColor(go, Color.red);
    }
    public static void MakeGrey(GameObject go)
    {
        SetColor(go, Color.grey);
    }
    public static void SetColor(GameObject go, Color color)
    {
        Renderer renderer = go.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.SetColor("_Color", color);
        }
    }
}
