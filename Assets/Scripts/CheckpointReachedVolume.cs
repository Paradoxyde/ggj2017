using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointReachedVolume : MonoBehaviour
{
    public int target_checkpoint;

	void Start ()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = false;
        }
	}

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            GameObject go = GameObject.FindGameObjectWithTag("Player");

            if (go)
            {
                PlatformingEntitiesManager pem = go.GetComponent<PlatformingEntitiesManager>();
                pem.OnReachedCheckpoint(target_checkpoint);
            }
        }
    }
}
