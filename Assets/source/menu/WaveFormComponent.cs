﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WaveFormComponent : MonoBehaviour
{
    public float MinimumFrequnecy;
    public float MaximumFrequnecy;

    public float m_Multiplier = 1.0f;

    public float m_MinimumSize = 100.0f;
    public float m_MaximumSize = 250.0f;

    public float m_SmoothTime = 0.08f;

    private RectTransform m_Transform;

    private float m_SmoothVelocity = 0.0f;
    private float m_MeanAmplitude = 0.0f;

    void Start()
    {
        m_Transform = GetComponent<RectTransform>();

        WaveFormManager.Instance.Register(this);
    }

    public void Resize(float mean)
    {
        m_MeanAmplitude = (m_MeanAmplitude + mean) * 0.5f;

        float log = Mathf.Log(mean / m_MeanAmplitude);
        float size = Mathf.Lerp(m_MinimumSize, m_MaximumSize, log);
        if (size < m_Transform.sizeDelta.x)
            size = Mathf.SmoothDamp(m_Transform.sizeDelta.x, size, ref m_SmoothVelocity, m_SmoothTime);

        m_Transform.sizeDelta = new Vector2(size, m_Transform.sizeDelta.y);
    }
}