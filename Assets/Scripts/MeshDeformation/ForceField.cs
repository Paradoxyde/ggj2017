using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{

    public float radius;
    public float force;

    private void Update()
    {
        Helpers.ShockWave(transform.position, radius, force);
    }
}
