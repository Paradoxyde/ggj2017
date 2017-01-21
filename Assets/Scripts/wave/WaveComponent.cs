using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveComponent : MonoBehaviour
{
    public virtual void Start()
    {
        WaveManager.Instance.Register(this);
    }
    
    void Update()
    {

    }

    public virtual void OnPhaseChanged(Phase phase)
    {

    }
}
