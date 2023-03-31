using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private bool _isGameOver = false;
    
    public static GameManager gameManager { get; private set; }
    // Set up ammo energy bar
    public EnergyBar _ammoEnergy;
    public EnergyBar _thrusterEnergy;
    private EnergyBar _energyBar;

    public EnergyBarUI _thrusterEnergyBarUI;

    void Start()
    {
        _ammoEnergy = new EnergyBar(15, 15);
        _thrusterEnergy = new EnergyBar(100, 100);
        // Set up thruster energy bar
        _thrusterEnergyBarUI = GameObject.FindGameObjectWithTag("ThrusterEnergyBar").GetComponent<EnergyBarUI>();
        if (_thrusterEnergyBarUI == null)
        {
            Debug.LogError("Could not find EnergyBarUI component on ThrusterEnergyBar object!");
            return;
        }
        
        _thrusterEnergyBarUI.SetEnergy(_thrusterEnergy.Energy);
        // Initialize _energyBar
        _energyBar = _thrusterEnergy;

    }


    void Awake()
    {
        if (gameManager != null && gameManager != this)
        {
            Destroy(this);
        }
        else
        {
            gameManager = this;
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && _isGameOver == true)
        {
            SceneManager.LoadScene(1); // 0 - "Game" Scene
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        
    }

    public void GameOver()
    {
        _isGameOver = true;
    }
}

