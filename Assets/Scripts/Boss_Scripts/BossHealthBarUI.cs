using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBarUI : MonoBehaviour
{
    private Slider _healthSlider;
    private BossHealthBar _bossHealthBar;
    

    public void SetMaxHealth(int maxHealth)
    {
        _healthSlider.maxValue = maxHealth;
        _healthSlider.value = maxHealth;
    }
    
    public void SetHealth(int health)
    {
        _healthSlider.value = health;
    }  
    
    public void InitializeBossHealthBarUI()
    {
        _healthSlider = GetComponentInChildren<Slider>();
        if (_healthSlider == null)
        {
            Debug.LogError("Slider component is null!");
            return;
        }

        GameObject bossObject = GameObject.FindGameObjectWithTag("Boss");
        if (bossObject == null)
        {
            Debug.LogError("Could not find Boss object!");
            return;
        }

        BossController bossController = bossObject.GetComponent<BossController>();
        if (bossController == null)
        {
            Debug.LogError("BossController component is null!");
            return;
        }

        _bossHealthBar = bossController.GetBossHealthBar();
        if (_bossHealthBar == null)
        {
            Debug.LogError("BossHealthBar is null!");
            return;
        }

        SetMaxHealth(_bossHealthBar.MaxHealth);
    }
    
    public void UpdateHealthBar(int currentHealth)
    {
        _healthSlider.value = currentHealth;
    }
    
}
