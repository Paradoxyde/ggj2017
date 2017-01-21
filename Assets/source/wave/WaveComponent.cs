using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveComponent : MonoBehaviour
{

    void Start()
    {
        WaveManager.Instance.Register(this);
    }
    
    void Update()
    {

    }

    public void OnPhaseChanged(Phase phase)
    {

    }
}
