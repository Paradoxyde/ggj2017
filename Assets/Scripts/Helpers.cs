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

    public static void ShockWave(Vector3 point, float radius, float force)
    {
        var surroundings = Physics2D.OverlapCircleAll(point, radius);
        foreach (var obj in surroundings)
        {
            var bones = obj.GetComponentsInChildren<BoneLeaf>();
            foreach (var bone in bones)
            {
                Vector3 delta = bone.transform.position - point;
                float attenuation = Mathf.Max(0f, (radius - delta.magnitude) / radius);
                bone.PropagateForce(Vector3.right * delta.normalized.x * attenuation * force);
            }
        }
    }
}
