using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderToPlayerPref : MonoBehaviour
{
    public string PlayerPrefName;
    private Slider m_Slider;

    public void Start()
    {
        m_Slider = GetComponent<Slider>();
        m_Slider.onValueChanged.AddListener(delegate { onValueChanged(); });
    }
    
    public void onValueChanged()
    {
        PlayerPrefs.SetFloat(PlayerPrefName, m_Slider.value);
        PlayerPrefs.Save();
    }
}
