using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAutoDestroy : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        if (ps.isStopped)
        {
            GameObject.Destroy(this);
        }
	}
}
