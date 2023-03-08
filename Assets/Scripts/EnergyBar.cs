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
  public void EnergyUseAmount(int energyUsed)
  {
    if (_currentEnergy > 0)
    {
      _currentEnergy -= energyUsed;
    }
  }
  
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
  
}
