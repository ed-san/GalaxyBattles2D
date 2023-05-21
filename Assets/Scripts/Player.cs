using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    [SerializeField]
    private float _speed = 6.0f;
    private float _originalSpeed = 6.0f;
    [SerializeField] 
    private float _thrusterSpeed;
    [SerializeField] 
    private float _thrusterMultiplier = 0.0f;
    private bool _boostedThrusterActive = true;
    [SerializeField]
    private float _speedMultiplier = 3.0f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleLaserPrefab;
    [SerializeField] 
    private GameObject _specialShotPrefab;
    [SerializeField] 
    private GameObject _tripleSpecialShotPrefab;
    [SerializeField]
    private GameObject _shieldVisualizer;
    [SerializeField] 
    private GameObject _shieldDamagedVisualizer;
    [SerializeField]
    private GameObject _leftEngineVisualizer, _rightEngineVisualizer;
    [SerializeField] 
    private GameObject _thrusterBoostPrefab;
    [SerializeField]
    private GameObject _playerDeath;
    [SerializeField]
    private Vector3 _offSetLaserSpawn = new Vector3(0, 0.866f, 0);
    [SerializeField]
    private float _fireRate = 0.15f;
    private float _canFire = -1.0f;
    [SerializeField]
    private int _lives = 3;
    private SpawnManager _spawnManager;
    // Powerup variables
    private bool _isTripleShotActive = false;
    private bool _isSpecialShotActive = false;
    private bool _isShieldActive = false;
    private bool _isDamagedShieldActive = false;
    [SerializeField]
    private bool _isSpeedBoostActive = false;
    private bool _isAmmoReloadActive = false;
    private Coroutine m_MyRunningCoroutine = null;
    private bool _coroutineActive = false;
    [SerializeField]
    private int _score = 0;
    private int _ammoCount = 15;
    private UIManager _uiManager;

    //variable to store audio clip
    [SerializeField]
    private AudioSource[] _audioSource;
    [SerializeField]
    private EnergyBarUI _ammoEnergyBarUI;
    [SerializeField] 
    private bool _LaserCooldown = false;
    [SerializeField]
    private EnergyBarUI _thrusterEnergyBar;
    private Slider _thrusterSlider;
    private bool _isEnergyRegenRunning = false;
    private bool _takeDamageActivePow = false;
    
    void Start()
    { 
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponents<AudioSource>();
        _thrusterSlider = _thrusterEnergyBar.GetComponent<Slider>();


        if (_thrusterSlider == null)
        {
            Debug.LogError("BoostThruster_EB_Fill Slider component is null!");
        }

        if (_spawnManager == null)
        {
            Debug.LogError("Spawn_Manager object doesn't have Spawn Manager script!");
        }
        
        if(_uiManager == null)
        {
            Debug.LogError("UIManager is NULL!");
        }

        if (_audioSource == null)
        {
            Debug.LogError("Player Audio Source component is NULL!");
        }

    }

    void Update()
    {
        CalculateMovement();
        ThrusterEngaged();
        FireLaser();

        if (_lives.Equals(3))
        {
            _rightEngineVisualizer.SetActive(false);
            _leftEngineVisualizer.SetActive(false);
        } 
        else if (_lives.Equals(2))
        {
            _rightEngineVisualizer.SetActive(true);
            _leftEngineVisualizer.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            AttractPowerups();
        }
        
        if (Input.GetKeyUp(KeyCode.C))
        {
            StopAttractingPowerups();
        }
        
    }

    void FireLaser()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            int energyCost = CalculateLaserEnergyCost();

            if (!AmmoIsEmpty() && GameManager.gameManager._ammoEnergy.Energy >= energyCost)
            {
                InstantiateLaser();
                LaserEnergyCost(energyCost);
                _canFire = Time.time + _fireRate;
            }
            else if (AmmoIsEmpty() && !_isEnergyRegenRunning)
            {
                StartCoroutine(LaserEnergyRegensRoutine(3.0f)); 
            }
        }
    }
    
     /// <summary>
    ///Spawns the appropriate laser prefab based on active power-ups,
    ///offset from the player's position, and plays an audio effect.
    /// </summary>
    
    void InstantiateLaser()
    {
        if (_LaserCooldown == false)
        {
            GameObject shotPrefab = null;

            if (_isTripleShotActive && _isSpecialShotActive)
            {
                shotPrefab = _tripleSpecialShotPrefab;
            }
            else if (_isTripleShotActive)
            {
                shotPrefab = _tripleLaserPrefab;
            }
            else if (_isSpecialShotActive)
            {
                shotPrefab = _specialShotPrefab;
            }
            else
            {
                shotPrefab = _laserPrefab;
            }

            Instantiate(shotPrefab, transform.position + _offSetLaserSpawn, Quaternion.identity);
            _audioSource[0].Play();
        }
    }
    
    int CalculateLaserEnergyCost()
    {
        int baseCost = 1; // Set the base energy cost for a regular shot
        int tripleShotCost = 3; // Set the energy cost for a triple shot
        int specialShotCost = 5; // Set the energy cost for a special shot
        int tripleSpecialShotCost = 8; // Set the energy cost for a triple special shot

        if (_isTripleShotActive && _isSpecialShotActive)
        {
            return tripleSpecialShotCost;
        }
        else if (_isTripleShotActive)
        {
            return tripleShotCost;
        }
        else if (_isSpecialShotActive)
        {
            return specialShotCost;
        }
        else
        {
            return baseCost;
        }
    }

    private void ThrusterEnergyCost(int energyUsed)
    {
        GameManager.gameManager._thrusterEnergy.EnergyUseAmount(energyUsed);
        _thrusterEnergyBar.SetEnergy(GameManager.gameManager._thrusterEnergy.Energy);

    }
    
    private void LaserEnergyCost(int energyUsed)
    {
        GameManager.gameManager._ammoEnergy.EnergyUseAmount(energyUsed);
        _ammoEnergyBarUI.SetEnergy(GameManager.gameManager._ammoEnergy.Energy);
        DecreaseAmmoCount(energyUsed);
    }
    

    private void LaserCostRegen(int regenAmt)
    {
        GameManager.gameManager._ammoEnergy.EnergyRegenAmount(regenAmt);
        _ammoEnergyBarUI.SetEnergy(GameManager.gameManager._ammoEnergy.Energy);
        IncreaseAmmoCount(regenAmt);
    }
    
    /// <summary>
    /// This method registers the players input from the WASD keys to move along the X-Axis and Y-Axis.
    /// It also limits the players range along the Y-Axis so that they can't go off-screen.
    /// </summary>
    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        transform.Translate(Time.deltaTime * _speed * _thrusterSpeed * direction);
        
       // The code below restricts Y-Axis movement between the ranges of -3.8f and 3.0f creating a vertical boundary for player.
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.8f, 3.0f),0);
        
        /*
           This if block allows the player to wrap around the screen
           when going off-screen on the left or right side along the X-Axis.
        */
        
        if (transform.position.x >= 11.3f)
        {
            float xPosOffScreenRight = transform.position.x * -1;
            float yPosOffScreenRight = transform.position.y * 1;
            
            transform.position = new Vector3(xPosOffScreenRight, yPosOffScreenRight, 0);
        }
        else if (transform.position.x <= -11.3f)
        {
            float xPosOffScreenLeft = transform.position.x * -1;
            float yPosOffScreenLeft = transform.position.y * 1;
            
            transform.position = new Vector3(xPosOffScreenLeft,  yPosOffScreenLeft, 0);
        }
    }
    

    public void Damage(int damageAmount)
    {
        if (_isShieldActive || _isDamagedShieldActive && _takeDamageActivePow )
        {
            _isShieldActive = false;
            _shieldVisualizer.SetActive(false);
            _isDamagedShieldActive = false;
            _shieldDamagedVisualizer.SetActive(false);
            _takeDamageActivePow = false;
        }
        else if (_isShieldActive == true)
        {
            _isShieldActive = false;
            _shieldVisualizer.SetActive(false);
            DamagedShieldActive();
        } else if (_isShieldActive == false && _isDamagedShieldActive == true)
        {
            _isDamagedShieldActive = false;
            _shieldDamagedVisualizer.SetActive(false);
        }
        else
        {
            _lives -= damageAmount;
            
            if (_lives.Equals(2))
            {
                _rightEngineVisualizer.SetActive(true);
            } 
            else if (_lives.Equals(1))
            {
                _leftEngineVisualizer.SetActive(true);
            }
            
            
            if (_lives < 1)
            {
                DestroyedPlayerSequence();
            }

            _uiManager.UpdateLives(_lives);
        }
       
    }

    public void HealActive()
    {
        if (_lives.Equals(3))
        {
            return;
        }
        else
        {
            _lives += 1;
            _uiManager.UpdateLives(_lives);
        }
       
    }

    public void SpecialShotActive()
    {
        _isSpecialShotActive = true;
        StartCoroutine(SpecialShotPowerDownRoutine(5)); 
    }
    
    public void LaserCoolDownActive()
    {
        _LaserCooldown = true;
        StartCoroutine(LaserEnergyRegensRoutine(3.0f));
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine(5));
    }
    
    public void AmmoReloadActive()
    {
        _isAmmoReloadActive = true;
        LaserCostRegen(15 - GameManager.gameManager._ammoEnergy.Energy);
        StartCoroutine(AmmoReloadPowerDownRoutine(.5f));
    }

    public void SpeedBoostActive()
    {
        if (_isSpeedBoostActive == true)
        {
            
            if (_coroutineActive == true && _speed >= 15.0f)
            {
                return;        
            } else if (_coroutineActive == true)
            {
                _speedMultiplier = 1.25f;
                _speed *= _speedMultiplier;
                StopMyCoroutine();
            } else
            {
                StopMyCoroutine();
            }
                StartMyCoroutine();
        }
        else
        {
            _speed *= _speedMultiplier;
            
        }

        _isSpeedBoostActive = true;
        StartMyCoroutine();
    }


    public void DrainEnergyActive()
    {
        LaserEnergyCost(15);
        _ammoCount = 0;
        _uiManager.UpdateAmmoCount(_ammoCount);
    }
    
    public void TakeDamageActive()
    {
        _takeDamageActivePow = true;
        Damage(2);
    }
   
        void StartMyCoroutine()
    {
        
        _coroutineActive = true;
        m_MyRunningCoroutine = StartCoroutine(SpeedBoostPowerDownRoutine(5));
    }

    void StopMyCoroutine()
    {
        _coroutineActive = false;
        if (m_MyRunningCoroutine != null)
        {
            StopCoroutine(m_MyRunningCoroutine);
            _isSpeedBoostActive = false;
            m_MyRunningCoroutine = null;
        }
    }

    public void ShieldActive()
    {
        _isShieldActive = true;
        _shieldVisualizer.SetActive(true);
    }

    public void DamagedShieldActive()
    {
        _isDamagedShieldActive = true;
        _shieldDamagedVisualizer.SetActive(true);
    }

    IEnumerator LaserEnergyRegensRoutine(float waitTime)
    {
        _isEnergyRegenRunning = true;
        _audioSource[1].Play();
        yield return new WaitForSeconds(waitTime);
        LaserCostRegen(15);
        _LaserCooldown = false;
        _audioSource[1].Stop();
        _isEnergyRegenRunning = false;
    }

    IEnumerator TripleShotPowerDownRoutine(float waitTime)
    {
            yield return new WaitForSeconds(waitTime);
            _isTripleShotActive= false;
    }
    
    IEnumerator SpecialShotPowerDownRoutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        _isSpecialShotActive= false;
    }
    
    IEnumerator AmmoReloadPowerDownRoutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        _isAmmoReloadActive= false;
    }

    IEnumerator SpeedBoostPowerDownRoutine(float waitTime)
    {

        yield return new WaitForSeconds(waitTime);

        switch(_speed)
        {
            case 15:
                _speed /= 2.5f;
                break;
            case 12:
                _speed /= _speedMultiplier;
                break;
            default:
                _speed = _originalSpeed;
                break;
        }

        _isSpeedBoostActive = false;
        _speedMultiplier = 2.0f;
    }
    public void IncreaseScore(int playerScore)
    {
        _score += playerScore;
        _uiManager.UpdateScore(_score);
    }

    public void IncreaseAmmoCount(int shotRecharged)
    {
        _ammoCount += shotRecharged;
        _uiManager.UpdateAmmoCount(_ammoCount);
    }

    public void DecreaseAmmoCount(int shotFired)
    {
        _ammoCount -= shotFired;
        _uiManager.UpdateAmmoCount(_ammoCount);
    }
    
    public bool AmmoIsEmpty()
    {
        return _ammoCount == 0;
    }

    public int AmmoCount
    {
        get { return _ammoCount;}
    }

    void DestroyedPlayerSequence()
    {
        _spawnManager.OnPlayerDeath();
        _speed = 0.0f;
        Instantiate(_playerDeath, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
    
   void ThrusterEngaged()
   {
       if (_boostedThrusterActive && Input.GetKey(KeyCode.LeftShift))
       {
           if (GameManager.gameManager._thrusterEnergy.GetCurrentEnergy() > 0)
           {
               _thrusterMultiplier = .75f;
               _thrusterSpeed = _speedMultiplier + _thrusterMultiplier;
               _thrusterBoostPrefab.SetActive(true);
               ThrusterEnergyCost(1);
           }
           else
           {
               _thrusterSpeed = 1.0f;
               _thrusterBoostPrefab.SetActive(false);
               StartCoroutine(BoostedThrusterCoolDownRoutine(5));
               StartCoroutine(RegenerateEnergyAndLockBoostRoutine(5));
           }
       }
       else
       {
           _thrusterSpeed = 1.0f;
           _thrusterBoostPrefab.SetActive(false);
       }
   }

   IEnumerator RegenerateEnergyAndLockBoostRoutine(float regenTime)
   {
       int initialEnergy = GameManager.gameManager._thrusterEnergy.GetCurrentEnergy();
       _audioSource[2].Play();
       // Wait for energy to regenerate over time
       yield return StartCoroutine(GameManager.gameManager._thrusterEnergy.RegenerateEnergyOverTime(5,_thrusterSlider));
       _audioSource[2].Stop();
       // Lock boost until energy reaches full
       while (!GameManager.gameManager._thrusterEnergy.IsFull())
       {   
           yield return null;
       }

       // Restore ability to use boost
       
       _boostedThrusterActive = true;
   }

   IEnumerator BoostedThrusterCoolDownRoutine(float cooldownTime)
   {
       _boostedThrusterActive = false;
       
       // Wait for cooldown time
       yield return new WaitForSeconds(cooldownTime);
       
       _boostedThrusterActive = true;
   }
   
   public bool IsShieldActive()
   {
       return _isShieldActive;
   }

   void AttractPowerups()
   {
       // Define the target layer
       int targetLayer = LayerMask.NameToLayer("Positive Powerup");

        // Get all GameObjects in the scene
       GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

       // Create a list to store the Powerup components
       List<Powerup> powerups = new List<Powerup>();

        // Loop over all GameObjects
       foreach (GameObject obj in allObjects)
       {
           // If the object is on the target layer and has a Powerup component, add it to the list
           if (obj.layer == targetLayer && obj.GetComponent<Powerup>() != null)
           {
               Powerup powerup = obj.GetComponent<Powerup>();
               powerups.Add(powerup);
               powerup.SetAttract(true, this.gameObject); // This will attract the powerups to the player
           }
       }
       
   }
   
   void StopAttractingPowerups()
   {
       // Get all powerups and call SetAttract(false, null) on each
       Powerup[] powerups = GameObject.FindObjectsOfType<Powerup>();
       foreach (Powerup powerup in powerups)
       {
           powerup.SetAttract(false, null);
       }
   }
   
   
}
