using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformingEntitiesManager : MonoBehaviour
{
    List<AirJumpHook> m_airJumpHooks;

	void Start ()
    {
        m_airJumpHooks = new List<AirJumpHook>();
	}
	
	void Update ()
    {
		
	}

    public void RegisterAirJumpHook(AirJumpHook hook)
    {
        m_airJumpHooks.Add(hook);
    }

    public AirJumpHook GetClosestActiveAirJumpHook(Vector3 position, float range)
    {
        float rangeSq = range * range;
        AirJumpHook closestHook = null;
        float closestRangeSq = float.MaxValue;
        
        foreach(AirJumpHook hook in m_airJumpHooks)
        {
            float distanceSq = (position - hook.transform.position).sqrMagnitude;
            if (hook.IsAvailable() && distanceSq < rangeSq && distanceSq < closestRangeSq)
            {
                closestRangeSq = distanceSq;
                closestHook = hook;
            }
        }

        return closestHook;
    }
}
