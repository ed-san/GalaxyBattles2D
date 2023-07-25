using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealthBar
{
    private int _currentHealth;
    private int _maxHealth;
  
    public int Health
    {
        get { return _currentHealth; }
        set { _currentHealth = value; }
    }
  
    public int MaxHealth
    {
        get { return _maxHealth; }
        set { _maxHealth = value; }
    }
  
    public BossHealthBar(int health, int maxHealth)
    {
        _currentHealth = health;
        _maxHealth = maxHealth;
    }
  
    public void TakeDamage(int damage)
    {
        _currentHealth = Mathf.Max(_currentHealth - damage, 0);
    }

    public void UpdateHealthSlider(UnityEngine.UI.Slider slider, int health)
    {
        slider.maxValue = MaxHealth;
        slider.value = health;
    }
  
    public bool IsDead()
    {
        return _currentHealth <= 0;
    }
}