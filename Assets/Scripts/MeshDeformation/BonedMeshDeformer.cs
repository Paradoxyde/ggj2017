using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonedMeshDeformer : MonoBehaviour
{

    public BoneLeaf root;

    public float springForce = 20f;
    public float damping = 5f;

    Mesh deformingMesh;
    Vector3[] originalVertices, displacedVertices;
    Vector3[] vertexVelocities;

    Vector2 uniformScale = new Vector2(1f, 1f);

    void Start()
    {
        deformingMesh = GetComponent<MeshFilter>().mesh;
        originalVertices = deformingMesh.vertices;
        displacedVertices = new Vector3[originalVertices.Length];
        for (int i = 0; i < originalVertices.Length; i++)
        {
            displacedVertices[i] = originalVertices[i];
        }
        vertexVelocities = new Vector3[originalVertices.Length];
    }

    void Update()
    {
        for (int i = 0; i < originalVertices.Length; i++)
        {
            displacedVertices[i] = originalVertices[i];
        }
        uniformScale = new Vector2(transform.localScale.x, transform.localScale.y);

        UpdateBone(root);

        deformingMesh.vertices = displacedVertices;
        deformingMesh.RecalculateNormals();
    }

    void UpdateBone(BoneLeaf bone)
    {
        foreach (BoneLeaf child in bone.Children)
        {
            UpdateBone(child);

            for (int i = 0; i < displacedVertices.Length; i++)
            {
                UpdateVertex(i, child, bone);
            }
        }
    }

    void UpdateVertex(int i, BoneLeaf current, BoneLeaf parent)
    {
        Vector3 currentPos = current.transform.localPosition;
        Vector3 currentOriginalPos = current.OriginalPosition;
        Vector3 parentPos = transform.InverseTransformPoint(parent.transform.position);

        Vector3 displacement = (currentPos - currentOriginalPos);
        float height = currentPos.y;
        Vector3 pointToVertex = displacedVertices[i] - parentPos;

        float weight = 0f;
        if (pointToVertex.y > currentPos.y)
        {
            weight = 1f;
        }
        else if (pointToVertex.y > 0f)
        {
            weight = pointToVertex.y / height;
        }
        else
        {
            weight = 0f;
        }

        displacement *= weight;
        displacedVertices[i].x += displacement.x;
        displacedVertices[i].y += displacement.y;
    }


    private void OnDrawGizmos()
    {
        if (root)
        {
            DrawChildren(root.transform);
        }
    }

    private void DrawChildren(Transform transform)
    {
        foreach (Transform child in transform)
        {
            DrawChildren(child);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
}
