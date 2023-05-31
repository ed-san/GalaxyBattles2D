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
    private bool _isStopped = false;
    private float _stoppedMovementTime = 5.0f;
    private bool _isShieldActive = false;
    [SerializeField]
    private GameObject _shieldVisualizer;
    private bool _hasFrontalLineOfSight = false;
    private bool _hasRearLineOfSight = false;
    private bool _hasPowerupLineOfSight = false;
    private Vector3 _direction = Vector3.zero;
    private bool _isOriginallyAngle = false;
    private float _rotationSpeed = 180f;  // 180 degrees per second
    private float _pauseDuration = 1.5f;  // 1.5 seconds
    [SerializeField] private float dodgeSpeed = 25f;
    [SerializeField] private float dodgeDistance = 100.0f;
    




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

        // Check the original movement type and set _isOriginallyAngle accordingly
        _isOriginallyAngle = _movementType == MovementType.Angle;


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
             
             if(_isDestroyed)
             {
                 return;
             }
             
                 if (!_isStopped)
                 {
                     CalculateMovement();
                     CheckFrontalLineOfSight();
                     CheckRearLineOfSight();
                     CheckPowerUpLineOfSight();
                     // Detect and dodge player's laser
                     DetectLaserAndDodge();
     
                     if (_hasRearLineOfSight && gameObject.CompareTag("Enemy"))
                     {
                         StartCoroutine(CounterAttack());
                     }
                     
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
     
     
                     if (_hasFrontalLineOfSight == true && _movementType == MovementType.StraightDown)
                     {
                         Debug.Log("Player is within view");
                         _movementType = MovementType.Angle;
                         
                             if (_player != null)
                             {
                                 if (_player.transform.position.x > transform.position.x)
                                 {
                                     // Player is to the right of the enemy. Move diagonally down-right.
                                     _direction = new Vector3(1, -1, 0).normalized;
                                 }
                                 else
                                 {
                                     // Player is to the left of the enemy. Move diagonally down-left.
                                     _direction = new Vector3(-1, -1, 0).normalized;
                                 }
                             }
                     }
                     
     
                 }
                 
                 float targetY = 0.0f;
                 float threshold = 0.1f;
                 if (Mathf.Abs(transform.position.y - targetY) < threshold && gameObject.CompareTag("AoeEnemy"))
                 {
                     StartCoroutine(StopMovementForSeconds(_stoppedMovementTime));
                 }
                 
                 // Check if the enemy has a clear line of sight to the player before firing
                 if (!_isDestroyed && Time.time > _canFire)
                 {
                     SetFireRateForMovementType();
                     _canFire = Time.time + _fireRate;
                     
                     // Wait for a short delay before firing the laser
                     StartCoroutine(DelayedFireLaser());
        
                 }
                 
                
         }
     
     
     private IEnumerator DelayedFireLaser()
     {
         yield return new WaitForSeconds(0.1f);  // Delay can be adjusted based on your needs
         
         // Instantiate the laser
         GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
         enemyLaser.layer = LayerMask.NameToLayer("Enemy Laser"); // Set the instantiated laser's layer to "Enemy Laser"
         Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

         foreach (Laser laser in lasers)
         {
             laser.AssignEnemyLaser();
             if (gameObject.CompareTag("Enemy")) // For RegularEnemy
             {
                 if (_hasRearLineOfSight)
                 {
                     // If the player is detected behind, let's set the direction towards the player.
                     laser.AssignDirection(_direction = Vector3.down);
                 }
                 else
                 {
                     // If the player is not detected behind, let's set the direction to downward.
                     laser.AssignDirection(Vector3.up);
                 }
                         
                 if (_hasPowerupLineOfSight)
                 {
                     laser.AssignDirection(Vector3.up);
                 }
                         
             }
             else if (gameObject.CompareTag("AoeEnemy")) // For AoeEnemy
             {
                 // Update _canFire value when the enemy is stopped and is an AoeEnemy
                 if (_isStopped && gameObject.CompareTag("AoeEnemy"))
                 {
                     _canFire = Time.time;
                 }
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
                // If line of sight is true, use _direction for movement. Otherwise, use the fixed angle.
                if (_hasFrontalLineOfSight && _player != null)
                {
                    transform.Translate(_direction * currentEnemySpeed * Time.deltaTime);
                }
                else
                {
                    newPosition = new Vector3(
                        transform.position.x + Mathf.Cos(Mathf.Deg2Rad * _angle) * currentEnemySpeed * Time.deltaTime,
                        transform.position.y + Mathf.Sin(Mathf.Deg2Rad * _angle) * currentEnemySpeed * Time.deltaTime,
                        transform.position.z);
                    transform.position = newPosition;
                }
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


        // Check if the enemy is originally an Angle enemy
        if (_isOriginallyAngle)
        {
            // Checks if enemy has gone off-screen, enable shield.
            if (transform.position.y <= -6.5f)
            {
                _isShieldActive = true;
                _shieldVisualizer.SetActive(true);
            }
        }
        else
        {
            // Check if the enemy has passed the Y-axis threshold, revert back to StraightDown
            if (transform.position.y <= -6.5f)
            {
                _movementType = MovementType.StraightDown;
            }
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
        if(_isDestroyed)
        {
            return;
        }

        // Handles enemy collisions so that they don't destroy each other with their more dynamic movements.
        if (other.CompareTag("Enemy") || other.CompareTag("AoeEnemy") || other.CompareTag("DodgeEnemy"))
        {
            StartCoroutine(PauseAndChangeMovement());
        }

        if (other.CompareTag("Player"))
        {
            if (_player != null)
            {
                _player.Damage(1);
                _cameraShake.Shake(_cameraShakeStrength);
                _isDestroyed = true;
            }
            
            if (gameObject.CompareTag("AoeEnemy"))
            {
                //Destroy(other.gameObject);
                _isDestroyed = true;

                _anim.SetTrigger("OnAoeEnemyDeath");
                _enemySpeed = 0;
                _audioSource[0].Play();

                Destroy(GetComponent<Collider2D>());
                Destroy(this.gameObject, 2.2f);
            }
            else if (gameObject.CompareTag("DodgeEnemy"))
            {
                //Destroy(other.gameObject);
                _isDestroyed = true;

                _anim.SetTrigger("OnDodgeEnemyDeath");
                _enemySpeed = 0;
                _audioSource[0].Play();

                Destroy(GetComponent<Collider2D>());
                Destroy(this.gameObject, 2.2f);
            }
            else
            {
                _anim.SetTrigger("OnEnemyDeath");
                _enemySpeed = 0;
                _audioSource[0].Play();
                Destroy(this.gameObject, 2.2f); 
            }

         
        }
        

        if (other.CompareTag("Laser"))
        {
            Destroy(other.gameObject);
            
            if (_isShieldActive)
            {
                _audioSource[2].Play();
                _isShieldActive = false;
                _shieldVisualizer.SetActive(false);
                return;
            }

            if (gameObject.CompareTag("AoeEnemy"))
                {
                    //Destroy(other.gameObject);
                    _isDestroyed = true;

                    _anim.SetTrigger("OnAoeEnemyDeath");
                    _enemySpeed = 0;
                    _audioSource[0].Play();

                    Destroy(GetComponent<Collider2D>());
                    Destroy(this.gameObject, 2.2f);
                }
                else if (gameObject.CompareTag("DodgeEnemy"))
                {
                    _isDestroyed = true;

                    _anim.Play("Dodge_Enemy_Destroyed_anim");
                    _enemySpeed = 0;
                    _audioSource[0].Play();

                    Destroy(GetComponent<Collider2D>());
                    Destroy(this.gameObject, 2.2f);
                }
                else
                {
                    //Destroy(other.gameObject);
                    _isDestroyed = true;

                    _anim.SetTrigger("OnEnemyDeath");
                    _enemySpeed = 0;
                    _audioSource[0].Play();

                    Destroy(GetComponent<Collider2D>());
                    Destroy(this.gameObject, 2.2f);
                }
            

            if (_player != null)
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

        if (other.CompareTag("SpecialShot"))
        {
            Destroy(other.gameObject);
            
            if (_isShieldActive)
            {
                _audioSource[2].Play();
                _isShieldActive = false;
                _shieldVisualizer.SetActive(false);
                return;
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
                _isDestroyed = true;

                if (gameObject.CompareTag("AoeEnemy"))
                {
                    _anim.SetTrigger("OnAoeEnemyDeath");
                    _enemySpeed = 0;
                    _audioSource[0].Play();

                    Destroy(GetComponent<Collider2D>());
                    Destroy(this.gameObject, 2.2f);
                }

                if (gameObject.CompareTag("Enemy"))
                {
                    _anim.SetTrigger("OnEnemyDeath");
                    _enemySpeed = 0;
                    _audioSource[0].Play();

                    Destroy(GetComponent<Collider2D>());
                    Destroy(this.gameObject, 2.2f);
                }

                if (_player != null)
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
    
    private IEnumerator StopMovementForSeconds(float waitTime)
    {
        _isStopped = true;
        // Store the original speed, movement type, and fire rate
        float originalSpeed = _enemySpeed;
        MovementType originalMovementType = _movementType;
        float originalFireRate = _fireRate;

        if (_isStopped)
        {
            // Set enemy speed to 0 to stop movement
            _enemySpeed = 0;
            // Quadruple the fire rate
            _fireRate = .25f;
        }
        
            // Wait for the specified duration
            yield return new WaitForSeconds(waitTime);
            
            // Restore the original speed, movement type, and fire rate
            _enemySpeed = originalSpeed;
            _movementType = originalMovementType;
            _fireRate = originalFireRate;
            _isStopped = false;

    }
    

        private void CheckFrontalLineOfSight()
        {
            // Get the position of the enemy
            Vector3 enemyPos = transform.position;

            // Cast rays diagonally down to the left and right | X-Axis widens the raycast
            Vector3[] directions = { new Vector3(-0.75f, -1, 0).normalized, new Vector3(0.75f, -1, 0).normalized };

            // Create a layer mask for the player layer (assuming the player is on layer 0)
            int playerLayerMask = 1 << 9;

            // Initialize line of sight flag to false
            _hasFrontalLineOfSight = false;
            
            // The distance to cast the rays
            float raycastDistance = 2.5f;

            foreach (var direction in directions)
            {
                // Cast a ray from the enemy towards the player
                RaycastHit2D hit = Physics2D.Raycast(enemyPos, direction, raycastDistance, playerLayerMask);
                
                // Draw the ray in the Scene view (for debug purposes only)
                Debug.DrawRay(enemyPos, direction * raycastDistance, Color.red);

                
                // If the raycast hits the player
                if (hit.collider != null && hit.collider.gameObject.CompareTag("Player") && !_player.IsShieldActive())
                {
                    // Set the line of sight flag to true
                    _hasFrontalLineOfSight = true;
                    break;
                }
            }
        }
        
        private void CheckRearLineOfSight()
        {
            // Get the position of the enemy
            Vector3 enemyPos = transform.position;

            // Cast rays behind the enemy (opposite of the forward direction)
            Vector3 direction = transform.up;

            // Create a layer mask for the player layer (assuming the player is on layer 9)
            int playerLayerMask = 1 << 9;

            // Initialize line of sight flag to false
            _hasRearLineOfSight = false;

            // The distance to cast the rays
            float raycastDistance = 4.0f;

            // Cast a ray from the enemy towards the player
            RaycastHit2D hit = Physics2D.Raycast(enemyPos, direction, raycastDistance, playerLayerMask);
    
            // Draw the ray in the Scene view (for debug purposes only)
            Debug.DrawRay(enemyPos, direction * raycastDistance, Color.red);

            // If the raycast hits the player
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Player") && !_player.IsShieldActive())
            {
                // Set the line of sight flag to true
                _hasRearLineOfSight = true;
            }
        }

        private void CheckPowerUpLineOfSight()
        {
            // Get the position of the enemy
            Vector3 enemyPos = transform.position;

            // Cast rays infront the enemy
            Vector3 direction = -transform.up;

            // Create a layer mask for the powerup layer (assuming the powerup is on layer 9)
            int playerLayerMask = 1 << 11;

            // Initialize line of sight flag to false
            _hasPowerupLineOfSight = false;

            // The distance to cast the rays
            float raycastDistance = 5.25f;

            // Cast a ray from the enemy towards the player
            RaycastHit2D hit = Physics2D.Raycast(enemyPos, direction, raycastDistance, playerLayerMask);
    
            // Draw the ray in the Scene view (for debug purposes only)
            Debug.DrawRay(enemyPos, direction * raycastDistance, Color.green);

            // If the raycast hits the player
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Powerups") && hit.collider.gameObject.layer == LayerMask.NameToLayer("Positive Powerup"))
            {
                // Set the line of sight flag to true
                _hasPowerupLineOfSight = true;
                Debug.Log("Powerup has been detected!");
            }
        }

        private IEnumerator CounterAttack()
        {
            // First, set the enemy to stop moving by setting a flag, speed to 0, etc.
            _isStopped = true;

            // Next, calculate the target angle to face upwards
            float targetAngle = -180;

            // Use a loop to gradually rotate the enemy to face upwards
            while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.z, targetAngle)) > 0.05f)
                {
                    // Calculate the new rotation for this frame
                    float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, _rotationSpeed * Time.deltaTime);

                    // Apply the new rotation
                    transform.eulerAngles = new Vector3(0, 0, angle);

                    // Wait until next frame
                    yield return null;
                }
            
                // Pause to allow the laser to be fired before turning back
                yield return new WaitForSeconds(_pauseDuration);

                // Now, calculate the target angle to face downwards again
                targetAngle = 0;

            // Use another loop to gradually rotate the enemy to face downwards
            while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.z, targetAngle)) > 0.05f)
                {
                    // Calculate the new rotation for this frame
                    float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, _rotationSpeed * Time.deltaTime);

                    // Apply the new rotation
                    transform.eulerAngles = new Vector3(0, 0, angle);

                    // Wait until next frame
                    yield return null;
                }

                // Finally, allow the enemy to move again
                _isStopped = false;
        }
        
       /* private void DetectLaserAndDodge()
        {
            // Set the detection range
            float detectionRange = 8.0f;

            // Cast a ray to the right and left
            RaycastHit2D hitInfoRight = Physics2D.Raycast(transform.position, Vector2.right, detectionRange, LayerMask.GetMask("Player Laser"));
            RaycastHit2D hitInfoLeft = Physics2D.Raycast(transform.position, Vector2.left, detectionRange, LayerMask.GetMask("Player Laser"));

            // Debug lines
            Debug.DrawRay(transform.position, Vector2.right * detectionRange, Color.yellow);
            Debug.DrawRay(transform.position, Vector2.left * detectionRange, Color.yellow);

            // Get the animator component
            Animator animator = GetComponent<Animator>();

            // Check if the right ray hit a laser
            if (hitInfoRight.collider != null)
            {
                // Laser detected on the right side, dodge to the left
                Dodge(-dodgeDistance);  // Move to the left
                animator.SetTrigger("LeftDodge");  // Trigger the LeftDodge animation
            }
            // Check if the left ray hit a laser
            else if (hitInfoLeft.collider != null)
            {
                // Laser detected on the left side, dodge to the right
                Dodge(dodgeDistance);  // Move to the right
                animator.SetTrigger("RightDodge");  // Trigger the RightDodge animation
            }
        }
        
        */
       private void DetectLaserAndDodge()
       {
           // Set the detection range and the number of rays
           float detectionRange = 10.0f;
           int numOfRays = 30;

           // Get the animator component
           Animator animator = GetComponent<Animator>();

           bool shouldDodge = false;

           for (int i = 0; i < numOfRays; i++)
           {
               // Calculate the ray's direction
               float angle = 270f - 15f + (30f / (numOfRays - 1)) * i; // Adjusting the starting angle
               Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

               // Cast the ray
               RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, direction, detectionRange, LayerMask.GetMask("Player Laser"));

               // Debug line
               Debug.DrawRay(transform.position, direction * detectionRange, Color.yellow);

               // Check if the ray hit a laser
               if (hitInfo.collider != null)
               {
                   shouldDodge = true;

                   if (hitInfo.collider.gameObject.CompareTag("Player"))
                   {
                       // if it's the player, don't dodge
                       animator.ResetTrigger("LeftDodge");
                       animator.ResetTrigger("RightDodge");
                   }
                   else if (hitInfo.point.x < transform.position.x)
                   {
                       // Laser detected on the left side, dodge to the right
                       Dodge(dodgeDistance);  // Move to the right
                       animator.SetTrigger("RightDodge");  // Trigger the RightDodge animation
                   }
                   else
                   {
                       // Laser detected on the right side, dodge to the left
                       Dodge(-dodgeDistance);  // Move to the left
                       animator.SetTrigger("LeftDodge");  // Trigger the LeftDodge animation
                   }
                   break;
               }
           }

           if (!shouldDodge)
           {
               animator.ResetTrigger("LeftDodge");
               animator.ResetTrigger("RightDodge");
           }
       }

        private void Dodge(float distance)
        {
            // Calculate the dodge direction and target position
            Vector3 direction = (distance > 0) ? Vector3.right : Vector3.left;
            Vector3 targetPosition = transform.position + direction * Mathf.Abs(distance);

            // Move towards the target position
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, dodgeSpeed * Time.deltaTime);
        }
        
     




}
