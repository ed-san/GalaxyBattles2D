using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private bool _isGameOver = false;
    [SerializeField]
    private GameObject _playerPrefab;
    public static GameManager gameManager { get; private set; }
    // Set up ammo energy bar
    public EnergyBar _ammoEnergy;
    public EnergyBar _thrusterEnergy;
    private EnergyBar _energyBar;

    public EnergyBarUI _thrusterEnergyBarUI;
    private Slider _thrusterSlider;
    public EnergyBarUI _ammoEnergyBarUI;
    private Slider _ammoSlider;
    private int _currentLevel;
    private SpawnManager _spawnManager;
    private BossController _bossController;
    private Player _player;
    [SerializeField]
    private bool _isLevelWon = false;
    private UIManager _uiManager;
    [SerializeField]
    private AudioSource _victoryAudioSource;
    


    void Start()
    {
        _ammoEnergy = new EnergyBar(15, 15);
        _thrusterEnergy = new EnergyBar(100, 100);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        GameObject playerObject = GameObject.FindWithTag("Player");
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        
        
        if (playerObject != null) {
            _player = playerObject.GetComponent<Player>();
            if (_player == null) {
                Debug.LogError("Player script not found on Player GameObject!");
            }
        } else {
            Debug.LogError("Player GameObject not found!");
        }
        
        if (_thrusterEnergyBarUI == null)
        {
            Debug.LogError("Could not find EnergyBarUI component on ThrusterEnergyBar object under Canvas!");
            return;
        }
        
        if (_ammoEnergyBarUI == null)
        {
            Debug.LogError("Could not find EnergyBarUI component on AmmoEnergyBar object under Canvas!");
            return;
        }
        
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
        
        // Set the energy bars to their full values
        _thrusterEnergyBarUI.SetEnergy(_thrusterEnergy.Energy);
        _ammoEnergyBarUI.SetEnergy(_ammoEnergy.Energy);
        

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
        
        CheckLevelWon();
        
        if (Input.GetKeyDown(KeyCode.R) && _isGameOver == true && !_spawnManager.IsBossSpawned)
        {
            SceneManager.LoadScene(_currentLevel); // 0 - "Game" Scene
        }

        if (Input.GetKeyDown(KeyCode.Space) && _isGameOver == true && _spawnManager.IsBossSpawned)
        {
            /* Execute all of the code to reset the state of the game to start at boss fight */
    
                //-Remove boss from scene
                if (_bossController != null)
                {
                    _bossController.RemoveBoss(); //This works!
                    _bossController = null;
                }

                //-Set _bossSpawned flag to false
                _spawnManager.IsBossSpawned = false;
                Debug.Log("SpawnManager instance ID before player creation: " + _spawnManager.GetInstanceID());

                //-Spawn player at position 0,0,0
                Vector3 position = new Vector3(0, 0, 0); //This works!
                Quaternion rotation = Quaternion.identity;
                
                GameObject newPlayer = Instantiate(_playerPrefab, position, rotation);//This works!

                // Find and assign the UI components immediately after instantiating the player
                Player playerComponent = newPlayer.GetComponent<Player>();
                if (playerComponent != null)
                {
                    playerComponent.SetThrusterEnergyBar(GameObject.FindGameObjectWithTag("ThrusterEnergyBar").GetComponent<EnergyBarUI>());
                    playerComponent.SetAmmoEnergyBarUI(GameObject.FindGameObjectWithTag("AmmoEnergyBar").GetComponent<EnergyBarUI>());
                    
                    // Assign _uiManager to the player component
                    UIManager uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
                    if(uiManager != null)
                    {
                        playerComponent.SetUIManager(uiManager);
                        uiManager.ResetGameOverUI();
                    }
                    else
                    {
                        Debug.LogError("UIManager is null!");
                    }

                    playerComponent.ResetHealth();
                }
                else
                {
                    Debug.LogError("Player component not found on the new Player GameObject!");
                }

                _player = playerComponent; // update the reference to the player
                        
                _thrusterEnergy = new EnergyBar(100, 100); //This works!
                _ammoEnergy = new EnergyBar(15, 15);       //This works!

                // Set the energy bars to their full values
                _thrusterEnergyBarUI.SetEnergy(_thrusterEnergy.Energy); //This works!
                _ammoEnergyBarUI.SetEnergy(_ammoEnergy.Energy);         //This works!
                        
                // Find the UI elements in the scene by their tags and assign them
                _thrusterEnergyBarUI = GameObject.FindGameObjectWithTag("ThrusterEnergyBar").GetComponent<EnergyBarUI>();
                _thrusterSlider = _thrusterEnergyBarUI.GetComponent<Slider>();
                _ammoEnergyBarUI = GameObject.FindGameObjectWithTag("AmmoEnergyBar").GetComponent<EnergyBarUI>();
                _ammoSlider = _ammoEnergyBarUI.GetComponent<Slider>();
                            
                //-Run code that spawns boss into the scene and move to center of screen
                _bossController = _spawnManager.SpawnBoss(_currentLevel); //This works!
                _bossController.StartMoving();                            //This works!

                _isGameOver = false; // Reset game over status
                
        }

        

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        
        if (Input.GetKeyDown(KeyCode.Q) && _isLevelWon)
        {
            SceneManager.LoadScene(0); // 0 - "Main Menu" Scene
        }
        
        if (Input.GetKeyDown(KeyCode.Q) && _isGameOver == true && _spawnManager.IsBossSpawned)
        {
            SceneManager.LoadScene(0); // 0 - "Main Menu" Scene
        }
        
        
        
    }

    public void GameOver()
    {
        _isGameOver = true;
    }
    
    private void CheckBossWave(int wave)
    {
        Debug.Log("CheckBossWave called. Wave: " + wave);
        if(wave == 8) // change to whatever wave the boss is supposed to appear on
        {
            // Find the SpawnManager to stop spawning enemies
            if (_spawnManager != null)
            {
                _spawnManager.OnPlayerDeath(); // stops the spawning of enemies and powerups
                Debug.Log("Current level: " + _currentLevel);
                _spawnManager.SpawnBoss(_currentLevel); // spawn the boss for the current level
                
                GameObject bossObject = GameObject.FindWithTag("Boss");
                if (bossObject != null) 
                {
                    _bossController = bossObject.GetComponent<BossController>();
                }
                
            }
            
        }
    }
    
    public void CheckLevelWon()
    {
        if (!_isLevelWon && _player != null && _player.Lives > 0 && _bossController != null && _bossController.IsDestroyed)
        {
            // Player has won the level!
            _isLevelWon = true;
            
            // Play the victory sound
            if (_victoryAudioSource != null)
            {
                // Stop the boss music
                _spawnManager.StopMusic();

                // Play the victory sound
                _victoryAudioSource.Play();

                // Start the coroutine to play the background music after the victory sound
                StartCoroutine(PlayBackgroundMusicAfterVictory());

            }
            else
            {
                Debug.LogError("Victory audio source is null!");
            }
        
            // Notify the UI
            _uiManager.DisplayLevelWon();
        }
    }
    
    private IEnumerator PlayBackgroundMusicAfterVictory()
    {
        // Wait for the length of the victory audio clip
        yield return new WaitForSeconds(_victoryAudioSource.clip.length);

        // Play the background music
        _spawnManager.PlayOriginalMusic();
    }

    
    

}

