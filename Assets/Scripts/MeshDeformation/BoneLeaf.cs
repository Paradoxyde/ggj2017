using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneLeaf : MonoBehaviour
{
    public Vector3 OriginalPosition { get; private set; }
    public Vector3 ReferencePosition { get; private set; }
    public BoneLeaf Parent { get; private set; }
    public List<BoneLeaf> Children { get; private set; }

    public float resistanceToParent = 0.5f;
    public float resistanceToChild = 0.3f;
    public float springForce = 20f;
    public float damping = 5f;

    public float oscillationForce = 10f;
    public float oscillationPropagationRate = 0.5f;

    private Vector3 m_Velocity = Vector3.zero;
    private float m_OscillationOffset;
    private Vector3 m_Accel = Vector3.zero;


    private void Start()
    {
        OriginalPosition = transform.localPosition;

        Children = new List<BoneLeaf>();
        foreach (Transform child in transform)
        {
            BoneLeaf leaf = child.GetComponent<BoneLeaf>(); 
            if(leaf)
            {
                leaf.Parent = this;
                Children.Add(leaf);
            }
        }

        m_OscillationOffset = Random.Range(-Mathf.PI, Mathf.PI);
    }

    private void Update()
    {
        if (!Parent)
        {
            float phase = Time.time / 5f + m_OscillationOffset;
            PropagateOscillation(phase, oscillationForce, oscillationPropagationRate);
        }
        
        Vector3 displacement = transform.localPosition - OriginalPosition;
        m_Velocity -= displacement * springForce * Time.deltaTime;
        m_Velocity *= 1f - damping * Time.deltaTime;

        transform.localPosition += m_Velocity * Time.deltaTime;
    }

    public void PropagateOscillation(float phase, float force, float propagation)
    {
        phase += propagation;

        foreach (var child in Children)
        {
            m_Velocity = Vector3.SmoothDamp(m_Velocity, Vector3.right * Mathf.Sin(phase) * force, ref m_Accel, 1f);
            child.PropagateOscillation(phase, force, propagation);
        }
    }

    public void PropagateForce(Vector3 force)
    {
        PropagateForceToParents(force);
        PropagateForceToChildren(force);
    }

    public void ApplyForce(Vector3 force)
    {
        if (Mathf.Approximately(force.magnitude, 0f))
            return;

        float velocity = force.magnitude * Time.deltaTime;
        Vector3 velocityDelta = force.normalized * velocity;
        m_Velocity += new Vector3(velocityDelta.x, velocityDelta.y);
    }

    public void PropagateForceToParents(Vector3 force)
    {
        force *= resistanceToParent;

        if (Parent)
        {
            ApplyForce(force);
            Parent.PropagateForceToParents(force);
        }
    }

    public void PropagateForceToChildren(Vector3 force)
    {
        force *= resistanceToChild;

        foreach (var child in Children)
        {
            child.PropagateForceToChildren(force);
            child.ApplyForce(force);
        }
    }


}
