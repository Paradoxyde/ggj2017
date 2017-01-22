using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public float wiggle_amplitude = 0.5f;
    public float wiggle_duration = 3.0f;

    Vector3 m_basePos;
    float m_wiggleCycleProgress = 0.0f;
    bool m_collected = false;

    void Start()
    {
        m_wiggleCycleProgress = Random.Range(0.0f, wiggle_duration);
        m_basePos = transform.position;
    }

    private void Update()
    {
        m_wiggleCycleProgress += Time.deltaTime;
        if (m_wiggleCycleProgress > wiggle_duration) m_wiggleCycleProgress -= wiggle_duration;
        
        transform.position = new Vector3(m_basePos.x, m_basePos.y + wiggle_amplitude * Mathf.Sin(m_wiggleCycleProgress / wiggle_duration * Mathf.PI * 2));
    }

    private void OnCollected()
    {
        Debug.Log("picked up collectible");
        m_collected = true;

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!m_collected && collider.CompareTag("Player"))
        {
            GameObject go = GameObject.FindGameObjectWithTag("Player");

            if (go)
            {
                OnCollected();
            }
        }
    }
}
