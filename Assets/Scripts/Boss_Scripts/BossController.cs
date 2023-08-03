using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine.UI;

using UnityEngine;

public class BossController : MonoBehaviour
{
    public Vector3 startPosition = new Vector3(0.2f, 10.8f, 0f);
    public Vector3 endPosition = new Vector3(0.2f, 4.25f, 0f);
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
    private int initialHealth = 80;
    [SerializeField]
    private int maxHealth = 80;
    private BossHealthBar _bossHealthBar;
    
    /* Variables Regarding Boss Lasers */
    [SerializeField]
    private GameObject _laserPrefab;
    // The number of lasers in one wave
    [SerializeField]
    private int _waveSize = 5; 
    // The total angle of the wave in degrees
    [SerializeField]
    private float _waveAngle = 140; 
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
    private const int _maxLaserWaveCycleCounter = 6; // The maximum value for the laser wave cycle counter.
    [SerializeField]
    private int _buildUpWaveCycleCounter = 1; // Counter for the buildup during non-rapid fire phase
    [SerializeField]
    private bool _isBuildUpPhase = true; // Flag to indicate whether the boss is in the build-up phase.
    [SerializeField]
    private bool _hasEnteredRapidFire = false;

    
    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponent<Animator>();
        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
        _cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        _audioSource = GetComponents<AudioSource>();


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
        StartMoving();
    }
    
    public void StartMoving()
    {
        StartCoroutine(MoveBoss(startPosition, endPosition, moveDuration));
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
        
        // Determine the fire rate based on whether we're in rapid fire mode.
        float currentFireRate;
        if (_isRapidFireMode)
        {
            currentFireRate = _rapidFireRate;
        }
        else
        {
            currentFireRate = _fireRate;
        }

        // Check if it's time to fire a wave
        if (_fireCounter >= currentFireRate)
        {
            // If we've just entered rapid fire mode, don't fire a wave yet.
            if (_hasEnteredRapidFire)
            {
                _hasEnteredRapidFire = false;
            }
            else
            {
                FireWave();
                _fireCounter = 0.0f; // Reset the counter

                // If we're not in rapid fire mode, increase the wave size and angle.
                if (!_isRapidFireMode)
                {
                    _waveSize = Mathf.Min(_waveSize + _waveSizeIncrease, _maxWaveSize);

                    // If we've reached the maximum wave size, enter rapid fire mode.
                    if (_waveSize >= 11 && !_isRapidFireMode)
                    {
                        _isRapidFireMode = true;
                        _isBuildUpPhase = false;
                        _fireRate = _rapidFireRate;
                        _waveSize = 100;

                        // Set _hasEnteredRapidFire to true.
                        _hasEnteredRapidFire = true;
                    }
                }
            }
        }

        // If we're in rapid fire mode, increase the rapid fire counter.
        if (_isRapidFireMode)
        {
            _rapidFireCounter += Time.deltaTime;

            if (_rapidFireCounter >= _rapidFireDuration)
            {
                _isRapidFireMode = false;
                _isBuildUpPhase = true;
                _rapidFireCounter = 0.0f;
                // Update the _laserWaveCycleCounter here, so it changes each time a wave is fired
                _laserWaveCycleCounter = (_laserWaveCycleCounter % _maxLaserWaveCycleCounter) + 1;

                switch (_laserWaveCycleCounter)
                {
                    case 1:
                    case 4:
                        _fireRate = 3.5f;  // Reset fire rate to 3.5 when wave counter is 1
                        _waveAngle = 120;
                        break;
                    case 2:
                    case 5:
                        _fireRate = 3.0f;  // Decrease fire rate to 3 when wave counter is 2
                        _waveAngle = 130;
                        break;
                    case 3:
                    case 6:
                        _fireRate = 2.5f;  // Decrease fire rate to 2.5 when wave counter is 3
                        _waveAngle = 150;
                        break;
                }

                // reset wave size and angle to defaults
                _waveSize = 5;
                //_waveAngle = 140;
                
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

        if (other.CompareTag("Laser") || other.CompareTag("HomingShot"))
        {
            Destroy(other.gameObject);
            
            if (other.CompareTag("HomingShot"))
            {
                _bossHealthBar.TakeDamage(2);  // boss takes damage
            }
            else
            {
                _bossHealthBar.TakeDamage(1);  // boss takes damage
            }
            
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
        
        if (other.CompareTag("SpecialShot"))
        {
            _bossHealthBar.TakeDamage(20);  // boss takes damage
                _bossHealthBarUI.UpdateHealthBar(_bossHealthBar.Health);

                if (_bossHealthBar.Health <= 0) // check if boss's health is 0 or less
                {
                    // Invoke event right after we've decided to destroy the enemy.
                    OnBossDestroyed?.Invoke(this.gameObject);
                    _isDestroyed = true;

                    _anim.SetTrigger("OnEnemyDeath");
                    _audioSource[0].Play();
                    Destroy(GetComponent<Collider2D>());
                    Destroy(this.gameObject, 2.2f);
                }

             SpriteRenderer specialShotSprite = other.gameObject.GetComponent<SpriteRenderer>();
            specialShotSprite.enabled = false;
            Laser specialShot = other.gameObject.GetComponent<Laser>();
            specialShot.SetProjectileSpeed(0);
            ParticleSystem specialShotParticles = other.gameObject.GetComponent<ParticleSystem>();
            specialShotParticles.Play();
            CircleCollider2D specialShotColliderRadius = other.gameObject.GetComponent<CircleCollider2D>();
            specialShotColliderRadius.radius = 18.3f;
            Destroy(other.gameObject, 4.0f);

        }
        
    }
    
    
    void FireWave()
    {
        // If in rapid fire mode, set wave angle based on laser wave cycle counter
        if (_isRapidFireMode)
        {
            switch (_laserWaveCycleCounter)
            {
                case 1:
                case 4:
                    _waveAngle = 120;
                    break;
                case 2:
                case 5:
                    _waveAngle = 130;
                    break;
                case 3:
                case 6:
                    _waveAngle = 150;
                    break;
            }
        }
        
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
        
        // Only update _buildUpWaveCycleCounter and _waveAngle if not in rapid fire mode
        if (!_isRapidFireMode && _isBuildUpPhase)
        {
            // Update the _buildUpWaveCycleCounter here, so it changes each time a wave is fired
            _buildUpWaveCycleCounter = (_buildUpWaveCycleCounter % 6) + 1;

            switch (_buildUpWaveCycleCounter)
            {
                case 1:
                    _waveAngle = 140;
                    break;
                case 2:
                    _waveAngle = 70;
                    break;
                case 3:
                    _waveAngle = 35;
                    break;
                case 4:
                    _waveAngle = 35;
                    break;
                case 5:
                    _waveAngle = 70;
                    break;
                case 6:
                    _waveAngle = 140;
                    break;
            }
        }
        
    }
    

    public BossHealthBar GetBossHealthBar()
    {
        return _bossHealthBar;
    }
    
    public bool IsDestroyed
    {
        get { return _isDestroyed; }
    }
    
    public void RemoveBoss()
    {
        // This will remove the boss object from the scene immediately.
        Destroy(this.gameObject);
    }




}
