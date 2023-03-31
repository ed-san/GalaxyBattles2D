using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBar 
{
  //Fields
  private int _currentEnergy;
  private int _currentMaxEnergy;
  
  //Properties
  public int Energy
  {
    get
    {
      return _currentEnergy;
    }
    set
    {
      _currentEnergy = value;
    }
  }
  
  public int MaxEnergy
  {
    get
    {
      return _currentMaxEnergy;
    }
    set
    {
      _currentMaxEnergy = value;
    }
  }
  
  //Constructor
  public EnergyBar(int energy, int maxEnergy)
  {
    _currentEnergy = energy;
    _currentMaxEnergy = maxEnergy;
  }
  
  //Methods
  /// <summary>
  /// This method decreases the energy amount by the amount passed as a parameter.
  /// </summary>
  public void EnergyUseAmount(int energyUsed)
  {
    //if (_currentEnergy > 0)
    //{
      //_currentEnergy -= energyUsed;
    //}
    _currentEnergy = Mathf.Max(_currentEnergy - energyUsed, 0);
  }
  
  /// <summary>
  /// This method increases the energy amount by the amount passed as a parameter.
  /// </summary>
  public void EnergyRegenAmount(int regenAmt)
  {
    if (_currentEnergy < _currentMaxEnergy)
    {
      _currentEnergy += regenAmt;
    }

    if (_currentEnergy > _currentMaxEnergy)
    {
      _currentEnergy = _currentMaxEnergy;
    }
  }
  
  /*Methods for Boost Thruster Energy System*/
  public bool UseEnergy(int energyUsed)
  {
    if (_currentEnergy >= energyUsed)
    {
      _currentEnergy -= energyUsed;
      return true;
    }
    return false;
  }
  
  public void RegenerateEnergy(int regenAmt)
  {
    _currentEnergy += regenAmt;

    if (_currentEnergy > _currentMaxEnergy)
    {
      _currentEnergy = _currentMaxEnergy;
    }
  }
  
  public IEnumerator RegenerateEnergyOverTime(int regenAmt, UnityEngine.UI.Slider energySlider)
  {
    while (_currentEnergy < _currentMaxEnergy)
    {
      _currentEnergy += regenAmt;

      if (_currentEnergy >= _currentMaxEnergy)
      {
        _currentEnergy = _currentMaxEnergy;
        UpdateEnergySlider(energySlider, _currentEnergy);
        yield break; // exit the loop
      }

      UpdateEnergySlider(energySlider, _currentEnergy);
      yield return new WaitForSeconds(.25f);
    }
  }

  public void UpdateEnergySlider(UnityEngine.UI.Slider slider, int energy)
  {
    slider.maxValue = MaxEnergy;
    slider.value = energy;
  }

  public bool IsFull()
  {
    return _currentEnergy == _currentMaxEnergy;
  }

  public int GetCurrentEnergy()
  {
    return _currentEnergy;
  }

}
