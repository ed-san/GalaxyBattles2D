using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{

    [SerializeField]
    private float _speed = 6.0f;
    private float _originalSpeed = 6.0f;
    [SerializeField]
    private float _speedMultiplier = 2.0f; 
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleLaserPrefab;
    [SerializeField]
    private GameObject _shieldVisualizer;
    [SerializeField]
    private GameObject _leftEngineVisualizer, _rightEngineVisualizer;
    [SerializeField]
    private Vector3 _offSetLaserSpawn = new Vector3(0, 0.866f, 0);
    [SerializeField]
    private float _fireRate = 0.15f;
    private float _canFire = -1.0f;
    [SerializeField]
    private int _lives = 3;
    private SpawnManager _spawnManager;
    private bool _isTripleShotActive = false;
    private bool _isShieldActive = false;
    [SerializeField]
    private bool _isSpeedBoostActive = false;
    private Coroutine m_MyRunningCoroutine = null;
    private bool _coroutineActive = false;
    [SerializeField]
    private int _score = 0;
    private UIManager _uiManager;

    //variable to store audio clip
    [SerializeField]
    private AudioClip _laserShotAudioClip;
    private AudioSource _audioSource;
    
    void Start()
    { 
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();
        
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
        else
        {
            _audioSource.clip = _laserShotAudioClip;
        }
    }

    void Update()
    {
        CalculateMovement();
        
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
              FireLaser();
        }
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
        transform.Translate(Time.deltaTime * _speed * direction);
        
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
        
        if (_isTripleShotActive == true)
        {
            Instantiate(_tripleLaserPrefab, transform.position, Quaternion.identity);
        } else
        {
            Instantiate(_laserPrefab, transform.position + _offSetLaserSpawn, Quaternion.identity);
        }
        
        _audioSource.Play();
    }

    public void Damage()
    {
        if (_isShieldActive == true)
        {
            _isShieldActive = false;
            _shieldVisualizer.SetActive(false);         
            return;
        }else
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
                _spawnManager.OnPlayerDeath();
                Destroy(this.gameObject);
            }

            _uiManager.UpdateLives(_lives);
        }
       
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine(5));
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

    IEnumerator TripleShotPowerDownRoutine(float waitTime)
    {
            yield return new WaitForSeconds(waitTime);
            _isTripleShotActive= false;
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

}
