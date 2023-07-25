using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine.UI;

using UnityEngine;

public class BossController : MonoBehaviour
{
    public Vector3 startPosition = new Vector3(0.2f, 10.8f, 0f);
    public Vector3 endPosition = new Vector3(0.2f, 3.26f, 0f);
    public float moveDuration = 7.0f; // Duration in seconds.
    private Slider _bossHealthBarSlider;
    private bool _isDestroyed = false;
    private Player _player;
    private Animator _anim;
    private CameraShake _cameraShake;
    [SerializeField] 
    private AudioSource[] _audioSource;
    [SerializeField] 
    private float _cameraShakeStrength = 5.0f;
    
    public delegate void BossDestroyedAction(GameObject boss);
    public static event BossDestroyedAction OnBossDestroyed;
    private BossHealthBarUI _bossHealthBarUI;
    [SerializeField]
    private int initialHealth = 30;
    [SerializeField]
    private int maxHealth = 30;
    private BossHealthBar _bossHealthBar;
    
    /* Variables Regarding Boss Lasers */
    [SerializeField]
    private GameObject _laserPrefab;
    // The number of lasers in one wave
    [SerializeField]
    private int _waveSize = 5; 
    // The total angle of the wave in degrees
    [SerializeField]
    private float _waveAngle = 45; 
    [SerializeField]
    private float _fireRate = 3.5f; 
    private float _fireCounter = 0.0f;
    private int _waveOneMaxAngle = 105;
    private int _waveTwoMaxAngle = 130;
    private int _waveThreeMaxAngle = 160;
    [SerializeField]
    private int _laserWaveCycleCounter = 1;
    private bool _isRapidFireMode = false; // Flag to indicate whether the boss is in rapid fire mode.
    private float _rapidFireCounter = 0.0f; // Counter to keep track of how long the boss has been in rapid fire mode.
    private const float _rapidFireDuration = 5.0f; // The duration of rapid fire mode in seconds.
    private const float _rapidFireRate = 0.25f; // The fire rate during rapid fire mode.
    private const int _maxWaveSize = 100; // The maximum wave size before entering rapid fire mode.
    private int _waveSizeIncrease = 1; // The amount to increase the wave size by each time the boss fires.
    private int _waveAngleIncrease = 12; // The amount to increase the wave angle by each time the boss fires.
    private const int _maxLaserWaveCycleCounter = 3; // The maximum value for the laser wave cycle counter.
    

    
    
    
    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        _player = GameObject.Find("Player").GetComponent<Player>();
        _cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        _audioSource = GetComponents<AudioSource>();
        //_bossHealthBarUI = GetComponentInChildren<Slider>();
        //_bossHealthBarSlider = _bossHealthBarUI.GetComponentInChildren<Slider>();
        //_bossHealthBar = new BossHealthBar(initialHealth, maxHealth);


        if (_anim == null)
        {
            Debug.LogError("The Animator is NULL.");
        }
        
        if (_player == null)
        {
            Debug.LogError("The Player is NULL.");
        }
        
        if (_cameraShake == null)
        {
            Debug.LogError("The CameraShake script isn't attached to main cam.");
        }
        
        if (_audioSource == null)
        {
            Debug.LogError("Enemy prefab doesn't have audio source component!");
        }
        
        
        
        // Set the boss's initial position.
        transform.position = startPosition;

        // Start moving the boss.
        StartCoroutine(MoveBoss(startPosition, endPosition, moveDuration));
    }
    
    void Update()
    {
       
    }
    
    void Awake()
    {
        _bossHealthBar = new BossHealthBar(initialHealth, maxHealth);
        _bossHealthBarUI = GetComponentInChildren<BossHealthBarUI>();

        if (_bossHealthBarUI == null)
        {
            Debug.LogError("BossHealthBarUI component is null!");
        }
        else
        {
            _bossHealthBarUI.InitializeBossHealthBarUI();
        }
    }


    IEnumerator MoveBoss(Vector3 start, Vector3 end, float duration)
    {
        // This is a simple timer.
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Calculate how far along the duration we are (between 0 and 1).
            float t = elapsed / duration;

            // Interpolate the boss's position between start and end based on t.
            transform.position = Vector3.Lerp(start, end, t);

            // Wait for the next frame.
            yield return null;

            // Update our timer.
            elapsed += Time.deltaTime;
        }

        // Ensure the boss ends up at the exact end position.
        transform.position = end;

        // Reset the fire counter
        _fireCounter = 0.0f;

        // Start Firing Boss Laser
        while (!_isDestroyed)
        {
           // Increase the counter by the time since the last frame
        _fireCounter += Time.deltaTime;

        // Check if it's time to fire a wave
        if (_fireCounter >= (_isRapidFireMode ? _rapidFireRate : _fireRate))
        {
            FireWave();
            _fireCounter = 0.0f; // Reset the counter

            // If we're not in rapid fire mode, increase the wave size and angle.
            if (!_isRapidFireMode)
            {
                _waveSize = Mathf.Min(_waveSize + _waveSizeIncrease, _maxWaveSize);
                switch (_laserWaveCycleCounter)
                {
                    case 1:
                        _waveAngleIncrease = 12;
                        _waveAngle = Mathf.Min(_waveAngle + _waveAngleIncrease, _waveOneMaxAngle);
                        break;
                    case 2:
                        _waveAngleIncrease = 17;
                        _waveAngle = Mathf.Min(_waveAngle + _waveAngleIncrease, _waveTwoMaxAngle);
                        break;
                    case 3:
                        _waveAngleIncrease = 23;
                        _waveAngle = Mathf.Min(_waveAngle + _waveAngleIncrease, _waveThreeMaxAngle);
                        break;
                }

                // If we've reached the maximum wave size, enter rapid fire mode.
                if (_waveSize >= 11 && !_isRapidFireMode)
                {
                    _isRapidFireMode = true;
                    _fireRate = _rapidFireRate;
                    _waveSize = 100;
                }
            }
        }

        // If we're in rapid fire mode, increase the rapid fire counter.
        if (_isRapidFireMode)
        {
            _rapidFireCounter += Time.deltaTime;

            // If we've been in rapid fire mode for long enough, reset to normal mode.
            if (_rapidFireCounter >= _rapidFireDuration)
            {
                _isRapidFireMode = false;
                _rapidFireCounter = 0.0f;
                _laserWaveCycleCounter = (_laserWaveCycleCounter % _maxLaserWaveCycleCounter) + 1;
                
                switch (_laserWaveCycleCounter)
                {
                    case 1:
                        _fireRate = 3.5f;  // Reset fire rate to 3.5 when wave counter is 1
                        break;
                    case 2:
                        _fireRate = 3.0f;  // Decrease fire rate to 3 when wave counter is 2
                        break;
                    case 3:
                        _fireRate = 2.5f;  // Decrease fire rate to 2.5 when wave counter is 3
                        break;
                }

                // reset wave size and angle to defaults
                _waveSize = 5;
                _waveAngle = 45;
            }
        }

        // Wait for the next frame.
        yield return null;

        }
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isDestroyed || other.CompareTag("BossLaser"))
        {
            return;
        }



        if (other.CompareTag("Player"))
        {
            if (_player != null)
            {
                _player.Damage(2);
                _bossHealthBar.TakeDamage(1);  // boss takes damage
                _bossHealthBarUI.UpdateHealthBar(_bossHealthBar.Health);
                _cameraShake.Shake(_cameraShakeStrength);
            }
        }

        if (other.CompareTag("Laser"))
        {
            _bossHealthBar.TakeDamage(1);  // boss takes damage
            _bossHealthBarUI.UpdateHealthBar(_bossHealthBar.Health);
                
            if (_bossHealthBar.Health <= 0)  // check if boss's health is 0 or less
            {
                _isDestroyed = true;

                _anim.SetTrigger("OnEnemyDeath");
                _audioSource[0].Play();
                Destroy(GetComponent<Collider2D>());
                Destroy(this.gameObject, 2.2f);
            }
        }


        /* This next block will handle the boss logic when he gains the shield in the final phase */
        if (other.CompareTag("Laser") || other.CompareTag("HomingShot"))
        {
            // Invoke event right after we've decided to destroy the enemy.
            OnBossDestroyed?.Invoke(this.gameObject);

            Destroy(other.gameObject);

            if (gameObject.CompareTag("Boss"))
            {
                _bossHealthBar.TakeDamage(1);  // boss takes damage
                
                if (_bossHealthBar.Health <= 0)  // check if boss's health is 0 or less
                {
                    _isDestroyed = true;

                    _anim.SetTrigger("OnEnemyDeath");
                    _audioSource[0].Play();
                    Destroy(GetComponent<Collider2D>());
                    Destroy(this.gameObject, 2.2f);
                }
            }

            /*The logic below will handle the points the player earns for defeating the boss on stage 1.
             This will vary from level to level and will have to create a new enum to hold the points 
             for each levels boss*/

            /*if (_player != null)
                {
                    int scoreToAdd = 0;
                    switch (_movementType)
                    {
                        case MovementType.StraightDown:
                            scoreToAdd = 2;
                            break;
                        case MovementType.SineWave:
                            scoreToAdd = 4;
                            break;
                        case MovementType.Circle:
                            scoreToAdd = 8;
                            break;
                        case MovementType.Angle:
                            scoreToAdd = 16;
                            if (gameObject.CompareTag("AoeEnemy"))
                            {
                                scoreToAdd += 8;
                            }

                            break;
                        default:
                            throw new InvalidOperationException($"Unhandled MovementType: {_movementType}");
                    }

                    _player.IncreaseScore(scoreToAdd);
                }

            }
            */


        }
        
        
        
    }
    
    void FireWave()
    {
        // Calculate the angle between each laser in the wave
        float step = _waveAngle / (_waveSize - 1);

        // Loop to create each laser in the wave
        for (int i = 0; i < _waveSize; i++)
        {
            // Instantiate a new laser
            GameObject laserObj = Instantiate(_laserPrefab, transform.position, Quaternion.identity);

            // Get the Laser component from the new laser object
            Laser laser = laserObj.GetComponent<Laser>();

            // Calculate the angle of this laser
            float angle = -_waveAngle / 2 + step * i;

            // Convert the angle to radians for the Mathf.Sin function
            float rad = angle * Mathf.Deg2Rad;

            // Calculate the direction of this laser, notice the y-component is now positive
            Vector3 direction = new Vector3(Mathf.Sin(rad), Mathf.Cos(rad), 0);

            // Assign the direction to the laser
            laser.AssignDirection(direction);

            // This is a boss's laser
            laser.AssignEnemyLaser();
        }
    }
    

    public BossHealthBar GetBossHealthBar()
    {
        return _bossHealthBar;
    }


}
