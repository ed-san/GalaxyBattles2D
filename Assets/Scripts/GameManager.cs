using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

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
    private int _currentLevel;
    private SpawnManager _spawnManager;


    void Start()
    {
        _ammoEnergy = new EnergyBar(15, 15);
        _thrusterEnergy = new EnergyBar(100, 100);
        
        // Set up thruster energy bar
        _thrusterEnergyBarUI = GameObject.FindGameObjectWithTag("ThrusterEnergyBar").GetComponent<EnergyBarUI>();
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

        if (_thrusterEnergyBarUI == null)
        {
            Debug.LogError("Could not find EnergyBarUI component on ThrusterEnergyBar object!");
            return;
        }
        
        _thrusterEnergyBarUI.SetEnergy(_thrusterEnergy.Energy);
        // Initialize _energyBar
        _energyBar = _thrusterEnergy;
        
        if (_spawnManager == null)
        {
            Debug.LogError("Spawn_Manager is NULL!");
        }
        else
        {
            _spawnManager.OnWaveUpdate += CheckBossWave;
        } 
        

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
            _currentLevel = SceneManager.GetActiveScene().buildIndex;
        }
        
        
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && _isGameOver == true)
        {
            SceneManager.LoadScene(_currentLevel); // 0 - "Game" Scene
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
    
    private void CheckBossWave(int wave)
    {
        Debug.Log("CheckBossWave called. Wave: " + wave);
        if(wave == 2) // change to whatever wave the boss is supposed to appear on
        {
            // Find the SpawnManager to stop spawning enemies
            if (_spawnManager != null)
            {
                _spawnManager.OnPlayerDeath(); // assuming this stops the spawning
                Debug.Log("Current level: " + _currentLevel);
                _spawnManager.SpawnBoss(_currentLevel); // spawn the boss for the current level
            }
            
        }
    }
}

