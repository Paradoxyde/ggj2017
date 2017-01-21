using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    public float speed = 8.0f;

	void Start ()
    {
		
	}

	void Update ()
    {
        transform.position += transform.rotation * Vector2.up * speed * Time.deltaTime;
	}

    private void OnTriggerEnter2D(Collider2D collider)
    {
        bool doDestroy = false;

        if (collider.CompareTag("Player"))
        {
            GameObject go = GameObject.FindGameObjectWithTag("Player");

            if (go)
            {
                PlatformingEntitiesManager pem = go.GetComponent<PlatformingEntitiesManager>();
                pem.OnPlayerDied();
            }
            doDestroy = true;
        }
        else if (collider.gameObject.layer == 8 || collider.gameObject.layer == 10)
        {
            doDestroy = true;
        }

        if (doDestroy)
        {
            GameObject.Destroy(gameObject);
        }
    }
}
