using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _enemySpeed = 4.0f;
    private Player _player;
    private Animator _anim;
    private CameraShake _cameraShake;
    [SerializeField] private AudioClip _enemyExplodingSoundClip;
    [SerializeField] private AudioSource[] _audioSource;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private float _fireRate;
    private float _canFire = -1.0f;
    [SerializeField] private float _cameraShakeStrength = 5.0f;
    private bool _isDestroyed = false;
    // All variables in relation to MovementType Logic
    [SerializeField] private MovementType _movementType;
    [SerializeField] private float _horizontalSpeed = 1.0f;
    [SerializeField] private float _horizontalWaveFrequency = 1.0f;
    private bool _hasEnteredView = false;
    private bool _hasReachedCircleStartPosition = false;
    private Vector3 _circleCenter;
    private float _circleRadius;
    private float _circleStartY;
    private float _angle; // Adjust this value to change the angle of descent

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _anim = GetComponent<Animator>();
        _audioSource = GetComponents<AudioSource>();
        _cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        _circleStartY = _circleCenter.y + 2.0f;
        _circleCenter = new Vector3(transform.position.x, _circleStartY - _circleRadius, transform.position.z);
        _angle = Random.Range(0, 2) == 0 ? -45.0f : -135.0f; // Angles pointing downwards

        if (_cameraShake == null)
        {
            Debug.LogError("The CameraShake script isn't attached to main cam.");
        }
        
        if (_player == null)
        {
            Debug.LogError("The Player is NULL.");
        }

        if (_anim == null)
        {
            Debug.LogError("The Animator is NULL.");
        }

        if (_audioSource == null)
        {
            Debug.LogError("Enemy prefab doesn't have audio source component!");
        }

    }

    public enum MovementType
    {
        StraightDown,
        Angle,
        SineWave,
        Circle
    }

    void Update()
    {
        CalculateMovement();
            // Check if the enemy has entered the visible area
            if (transform.position.y <= 5.5f)
            {
                _hasEnteredView = true;
            }

            // Check if the enemy has reached the start position for the "Circle" movement type
            if (transform.position.y <= _circleStartY && !_hasReachedCircleStartPosition)
            {
                _hasReachedCircleStartPosition = true;
                _circleCenter = new Vector3(transform.position.x, _circleStartY - _circleRadius, transform.position.z);
            }

            // Check if the enemy has a clear line of sight to the player before firing
            if (!_isDestroyed && Time.time > _canFire)
            {
                SetFireRateForMovementType();
                _canFire = Time.time + _fireRate;

                GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
                enemyLaser.layer =
                    LayerMask.NameToLayer("Enemy Laser"); // Set the instantiated laser's layer to "Enemy Laser"
                Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

                foreach (Laser laser in lasers)
                {
                    laser.AssignEnemyLaser();
                }
            }

    }

    void CalculateMovement()
    {
        float currentEnemySpeed = _enemySpeed;

        // If the enemy has not yet entered the visible area, move straight down
        if (!_hasEnteredView)
        {
            transform.Translate(Time.deltaTime * _enemySpeed * Vector3.down);
            return;
        }

        // Increase the movement speed by 3 when MovementType is "Angle"
        if (_movementType == MovementType.Angle)
        {
            currentEnemySpeed += 3;
        }

        Vector3 newPosition;

        switch (_movementType)
        {
            case MovementType.StraightDown:
                transform.Translate(Time.deltaTime * _enemySpeed * Vector3.down);
                break;
            case MovementType.Angle:
                // Move at a custom angle (in degrees)
                newPosition = new Vector3(
                    transform.position.x + Mathf.Cos(Mathf.Deg2Rad * _angle) * currentEnemySpeed * Time.deltaTime,
                    transform.position.y + Mathf.Sin(Mathf.Deg2Rad * _angle) * currentEnemySpeed * Time.deltaTime,
                    transform.position.z);
                transform.position = newPosition;
                break;
            case MovementType.Circle:
                if (_hasReachedCircleStartPosition)
                {
                    // Circle around a central point
                    _circleRadius = .75f; // Adjust this value to change the radius of the circle
                    float circleSpeed = 4.0f; // Adjust this value to change the speed of circular movement
                    float angleRadians = Time.time * circleSpeed;
                    newPosition = new Vector3(
                        _circleCenter.x + Mathf.Cos(angleRadians) * _circleRadius,
                        _circleCenter.y - _circleRadius + Mathf.Sin(angleRadians) * _circleRadius,
                        _circleCenter.z);
                    transform.position = newPosition;
                }
                else
                {
                    // If the enemy has not reached the start position, move straight down
                    transform.Translate(Time.deltaTime * _enemySpeed * Vector3.down);
                }

                break;
            case MovementType.SineWave:
                // Enemy moves side to side
                float horizontalOffset = Mathf.Sin(Time.time * _horizontalWaveFrequency) * _horizontalSpeed;
                newPosition = new Vector3(Mathf.Clamp(transform.position.x + horizontalOffset, -12.0f, 12.0f),
                    transform.position.y, transform.position.z);
                transform.position = Vector3.MoveTowards(transform.position, newPosition, Time.deltaTime * _enemySpeed);
                break;
            default:
                throw new InvalidOperationException($"Unhandled MovementType: {_movementType}");
        }

        // if enemy object is at the bottom of screen
        // respawn at top with a random X-Axis position
        if (transform.position.y <= -10.5f)
        {
            float randomX = Random.Range(-9.46f, 9.46f);
            transform.position = new Vector3(randomX, 10.5f, 0);
        }
    }


    public void SetMovementType(MovementType movementType)
    {
        _movementType = movementType;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        // Handles enemy collisions so that they don't destroy each other with their more dynamic movements.
        if (other.CompareTag("Enemy"))
        {
            StartCoroutine(PauseAndChangeMovement());
        }

        if (other.CompareTag("Player"))
        {
            if (_player != null)
            {
                _player.Damage();
                _cameraShake.Shake(_cameraShakeStrength);
                _isDestroyed = true;
            }

            _anim.SetTrigger("OnEnemyDeath");
            _enemySpeed = 0;
            _audioSource[0].Play();
            Destroy(this.gameObject, 2.2f);
        }

        if (other.CompareTag("Laser"))
        {
            Destroy(other.gameObject);
            _isDestroyed = true;
            if (_player != null)
            {
                _player.IncreaseScore(10);
            }

            _anim.SetTrigger("OnEnemyDeath");
            _enemySpeed = 0;
            _audioSource[0].Play();

            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.2f);
        }

        if (other.CompareTag("SpecialShot"))
        {
            SpriteRenderer specialShotSprite = other.gameObject.GetComponent<SpriteRenderer>();
            specialShotSprite.enabled = false;
            Laser specialShot = other.gameObject.GetComponent<Laser>();
            specialShot.SetProjectileSpeed(0);
            ParticleSystem specialShotParticles = other.gameObject.GetComponent<ParticleSystem>();
            specialShotParticles.Play();
            CircleCollider2D specialShotColliderRadius = other.gameObject.GetComponent<CircleCollider2D>();
            specialShotColliderRadius.radius = 18.3f;
            Destroy(other.gameObject, 4.0f);
            _isDestroyed = true;

            if (_player != null)
            {
                _player.IncreaseScore(10);
            }

            _anim.SetTrigger("OnEnemyDeath");
            _enemySpeed = 0;
            _audioSource[0].Play();

            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.2f);
        }
    }

    private IEnumerator PauseAndChangeMovement()
    {
        // Pause movement
        float originalSpeed = _enemySpeed;
        _enemySpeed = 0;

        // Wait for a short duration
        yield return new WaitForSeconds(1.0f);

        // Change movement type to StraightDown and resume movement
        _movementType = MovementType.StraightDown;
        _enemySpeed += 8.0f;

        while (!_hasEnteredView)
        {
            yield return null;
        }
        _enemySpeed = originalSpeed;
    }
    
    private void SetFireRateForMovementType()
    {
        switch (_movementType)
        {
            case MovementType.StraightDown:
                _fireRate = Random.Range(2.0f, 3.0f);
                break;
            case MovementType.Angle:
                _fireRate = Random.Range(0.5f, 1.5f);
                break;
            case MovementType.SineWave:
                _fireRate = Random.Range(0.1f, 0.20f);
                break;
            case MovementType.Circle:
                _fireRate = Random.Range(1.0f, 3.0f);
                break;
            default:
                throw new InvalidOperationException($"Unhandled MovementType: {_movementType}");
        }
    }
    
    
    

}
