using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBarUI : MonoBehaviour
{
    private Slider _energySlider;

    void Start()
    {
        _energySlider = GetComponent<Slider>();
        if (_energySlider == null)
        {
            Debug.LogError("Slider component is null!");
        }
    }

    public void SetMaxEnergy(int maxEnergy)
    {
        _energySlider.maxValue = maxEnergy;
        _energySlider.value = maxEnergy;
    }
    
    public void SetEnergy(int energy)
    {
        _energySlider.value = energy;
    }
    
}
