using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempoScaler : MonoBehaviour
{

    public float m_MinimumSize = 0.1f;
    public float m_MaximumSize = 2.0f;

    public float m_SmoothTime = 0.2f;

    private Transform m_Transform;

    private float m_SmoothVelocity = 0.0f;
    private float m_Size = 0f;

    private int m_CurrentBeat = -1;

    protected void Start()
    {
        m_Transform = GetComponent<Transform>();
        m_Size = m_MinimumSize;
    }

    public void Update()
    {
        m_Size = Mathf.SmoothDamp(m_Size, m_MinimumSize, ref m_SmoothVelocity, m_SmoothTime);

        int currentBeat = (int)(BGM.Instance.Time * BGM.Instance.tempo / 60f);
        if (m_CurrentBeat != currentBeat)
        {
            m_CurrentBeat = currentBeat;
            m_Size = m_MaximumSize;
        }

        m_Transform.localScale = new Vector3(m_Size, m_Size, m_Size);
    }
}
