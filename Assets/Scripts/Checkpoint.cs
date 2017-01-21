using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpoint_index = 0;

    bool m_registered = false;
    
	void Update ()
    {
        if (!m_registered) TryRegister();
    }

    void TryRegister()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Player");

        if (go)
        {
            PlatformingEntitiesManager pem = go.GetComponent<PlatformingEntitiesManager>();
            pem.RegisterCheckpoint(this);
            m_registered = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
