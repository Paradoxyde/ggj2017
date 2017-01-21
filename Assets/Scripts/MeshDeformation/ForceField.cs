using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{

    public float radius;
    public float force;

    private void Update()
    {
        var surroundings = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (var obj in surroundings)
        {
            //var deformer = obj.GetComponent<MeshDeformer2D>();
            //if (deformer)
            //{
            //    deformer.AddDeformingForce(transform.position, force);
            //}

            var bones = obj.GetComponentsInChildren<BoneLeaf>();
            foreach (var bone in bones)
            {
                Vector3 delta = bone.transform.position - transform.position;
                float attenuation = Mathf.Max(0f, (radius - delta.magnitude) / radius);
                bone.PropagateForce(Vector3.right * delta.normalized.x * attenuation * force);
            }
        }
    }
}
