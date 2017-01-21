using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassThroughPlatform : MonoBehaviour
{
    bool m_registered = false;

    void Start()
    {
		
	}
	
	void Update()
    {
        if (!m_registered) TryRegister();
    }

    void TryRegister()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Player");

        if (go)
        {
            PlatformingEntitiesManager pem = go.GetComponent<PlatformingEntitiesManager>();
            pem.RegisterPassThroughPlatform(this);
            m_registered = true;
        }
    }
}
