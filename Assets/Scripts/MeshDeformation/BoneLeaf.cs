using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneLeaf : MonoBehaviour
{
    public Vector3 OriginalPosition { get; private set; }
    public BoneLeaf Parent { get; private set; }
    public List<BoneLeaf> Children { get; private set; }

    public float resistanceToParent = 0.5f;
    public float resistanceToChild = 0.3f;
    public float springForce = 20f;
    public float damping = 5f;

    private Vector3 m_Velocity = Vector3.zero;


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
    }

    private void Update()
    {
        Vector3 displacement = transform.localPosition - OriginalPosition;
        //displacement.x *= uniformScale.x;
        //displacement.y *= uniformScale.y;
        m_Velocity -= displacement * springForce * Time.deltaTime;
        m_Velocity *= 1f - damping * Time.deltaTime;

        transform.localPosition += m_Velocity * Time.deltaTime;
        //displacedVertices[i].y += velocity.y * Time.deltaTime;
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
