using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempoCharacterPulse : MonoBehaviour
{
    public Renderer renderer1;
    public Renderer renderer2;

    public float minimumGlow = 1f;
    public float maximumGlow = 3.0f;

    public float m_SmoothTime = 0.2f;
    

    private float m_SmoothVelocity = 0.0f;
    private float glow = 0f;

    private int m_CurrentBeat = -1;

    protected void Start()
    {
        glow = minimumGlow;
    }

    public void Update()
    {
        glow = Mathf.SmoothDamp(glow, minimumGlow, ref m_SmoothVelocity, m_SmoothTime);

        if (BGM.Instance != null)
        {
            int currentBeat = (int)(BGM.Instance.Time * BGM.Instance.tempo / 60f);
            if (m_CurrentBeat != currentBeat)
            {
                m_CurrentBeat = currentBeat;
                glow = maximumGlow;
            }

            renderer1.material.SetFloat("_Brightness", glow);
            renderer2.material.SetFloat("_Brightness", glow);
        }
    }
}
