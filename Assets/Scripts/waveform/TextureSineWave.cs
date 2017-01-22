using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureSineWave : MonoBehaviour
{
    public Transform Ball;
    public int m_SimulationSteps = 360;
    public float m_SineRadius = 1.0f;
    public int m_LineTickness = 10; // Really it is just a fake .. but w/e it works.

    void Start()
    {
    }

    void Update()
    {
        if (Ball != null)
        {
            float phase = WaveManager.Instance.CycleAngle;
            Vector3 position = new Vector3(Mathf.PI, Mathf.Sin(phase), 0) * m_SineRadius;
            Ball.localPosition = position;
        }
    }

    Material lineMaterial;
    void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            //Shader shader = Shader.Find("Custom/Gradient_3Color");
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);

            //lineMaterial.SetColor("Top Color", TopColor);
            //lineMaterial.SetColor("Mid Color", MiddleColor);
            //lineMaterial.SetColor("Bot Color", BottomColor);
            //lineMaterial.SetFloat("Middle", 0.5f);
        }
    }
    
    public void OnRenderObject()
    {
        CreateLineMaterial();
        // Apply the line material
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        GL.MultMatrix(transform.localToWorldMatrix);

        // Draw lines
        GL.Begin(GL.LINES);

        float phase = WaveManager.Instance.CycleAngle - Mathf.PI;
        float stepAngle = Mathf.PI * 2.0f / m_SimulationSteps;
        float angle = phase;

        float x = 0.0f;
        Vector3 previousPos = new Vector3(x, Mathf.Sin(angle), 0) * m_SineRadius;
        angle += stepAngle;
        if (angle > 2.0f * Mathf.PI)
            angle -= 2.0f * Mathf.PI;
        else if (angle < 0)
            angle += 2.0f * Mathf.PI;
        x += stepAngle;  
        
        for (int i = 1; i < m_SimulationSteps; ++i)
        {
            float y = Mathf.Sin(angle);
            float angleRatio = angle / (2.0f * Mathf.PI);

            Color color = WaveManager.Instance.GetColorAt(angleRatio);
            
            Vector3 pos = new Vector3(x, y, 0) * m_SineRadius;

            GL.Color(color);

            //GL.Vertex3(previousPos.x, previousPos.y, previousPos.z);
            //GL.Vertex3(pos.x, pos.y, pos.z);

            int half = m_LineTickness / 2;
            for (int j = -half; j <= half; ++j)
            {
                GL.Vertex3(previousPos.x, previousPos.y + 0.001f * j, previousPos.z);
                GL.Vertex3(pos.x, pos.y + 0.001f * j, pos.z);
            }

            previousPos = pos;

            angle += stepAngle;
            if (angle > 2.0f * Mathf.PI)
                angle -= 2.0f * Mathf.PI;
            x += stepAngle;
        }

        GL.End();
        GL.PopMatrix();
    }
}
