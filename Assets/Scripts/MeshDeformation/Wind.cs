using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{

    private BoneLeaf[] m_AllBones;

    public float minForce = 0f;
    public float maxForce = 10f;
        
	void Start ()
    {
        m_AllBones = GameObject.FindObjectsOfType<BoneLeaf>();
	}
	
	void Update ()
    {


		foreach (var bone in m_AllBones)
        {
            float force = Random.Range(minForce, maxForce);
            bone.PropagateForce(Vector3.right * force);
        }
	}
}
