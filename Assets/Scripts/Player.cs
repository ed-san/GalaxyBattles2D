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
    [SerializeField]
    private float _speedMultiplier = 3.0f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleLaserPrefab;
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
    private bool _isShieldActive = false;
    private bool _isDamagedShieldActive = false;
    [SerializeField]
    private bool _isSpeedBoostActive = false;
    private bool _isAmmoReloadActive = false;
    private Coroutine m_MyRunningCoroutine = null;
    private bool _coroutineActive = false;
    [SerializeField]
    private int _score = 0;
    private UIManager _uiManager;

    //variable to store audio clip
    [SerializeField]
    private AudioSource[] _audioSource;
    [SerializeField]
    private EnergyBarUI _energyBar;
    [SerializeField] 
    private bool _LaserCooldown = false;

    void Start()
    { 
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponents<AudioSource>();

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
        
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
              FireLaser();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_isTripleShotActive == true)
            {
                LaserEnergyCost(3);
                //Debug.Log(GameManager.gameManager._playerEnergy.Energy); 
            }
            else
            {
                LaserEnergyCost(1);
                //Debug.Log(GameManager.gameManager._playerEnergy.Energy); 
            }
            
        }
        
        if (_isAmmoReloadActive == true)
        {
            LaserCostRegen(15);
        }
        
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
        
    }

    private void LaserEnergyCost(int energyUsed)
    {
        GameManager.gameManager._playerEnergy.EnergyUseAmount(energyUsed);
        _energyBar.SetEnergy(GameManager.gameManager._playerEnergy.Energy);
    }
    
    private void LaserCostRegen(int regenAmt)
    {
        GameManager.gameManager._playerEnergy.EnergyRegenAmount(regenAmt);
        _energyBar.SetEnergy(GameManager.gameManager._playerEnergy.Energy);
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

    /// <summary>
    /// Adds the fire rate delay to the current time(in seconds) of the frame and spawns
    /// the laser prefab object with a Y-Axis off-set from the player object location.
    /// </summary>
    
    void FireLaser()
    {
        _canFire = Time.time + _fireRate;
        
        if (_isTripleShotActive == true && _LaserCooldown == false)
        {
            if (GameManager.gameManager._playerEnergy.Energy > 0)
            {
                Instantiate(_tripleLaserPrefab, transform.position, Quaternion.identity);
                _audioSource[0].Play();
            }
            else
            {
                LaserCoolDownActive();
            }
        } else if (_LaserCooldown == false)
        {
            if (GameManager.gameManager._playerEnergy.Energy > 0)
            {
                Instantiate(_laserPrefab, transform.position + _offSetLaserSpawn, Quaternion.identity); 
                
                _audioSource[0].Play();
            }
            else
            {
                LaserCoolDownActive();
            }
        }

    }

    public void Damage()
    {
        if (_isShieldActive == true)
        {
            _isShieldActive = false;
            _shieldVisualizer.SetActive(false);
            DamagedShieldActive();
            return;
        } else if (_isShieldActive == false && _isDamagedShieldActive == true)
        {
            _isDamagedShieldActive = false;
            _shieldDamagedVisualizer.SetActive(false);
            return;
        }
        else
        {
            _lives -= 1;
            
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
        _audioSource[1].Play();
        yield return new WaitForSeconds(waitTime);
        LaserCostRegen(15);
        _LaserCooldown = false;
        _audioSource[1].Stop();
    }

    IEnumerator TripleShotPowerDownRoutine(float waitTime)
    {
            yield return new WaitForSeconds(waitTime);
            _isTripleShotActive= false;
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

    void DestroyedPlayerSequence()
    {
        _spawnManager.OnPlayerDeath();
        _speed = 0.0f;
        Instantiate(_playerDeath, transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }

    void ThrusterEngaged()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            _thrusterMultiplier = .75f;
            _thrusterSpeed = _speedMultiplier + _thrusterMultiplier;
            _thrusterBoostPrefab.SetActive(true);
        }
        else
        {
            _thrusterSpeed = 1.0f;
            _thrusterBoostPrefab.SetActive(false);
        }
    }

}
