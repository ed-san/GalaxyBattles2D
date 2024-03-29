using System;
using Unity.VisualScripting;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _projectileSpeed = 10.0f;

    private bool _isEnemyLaser = false;
    // Add reference to CameraShake script
    private CameraShake _cameraShake;
    [SerializeField]
    private float _cameraShakeStrength = 5.0f;

    // Initialize _cameraShake reference in Start() method
    private void Start()
    {
        _cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
    }
    
    void Update()
    {
        
        if (_isEnemyLaser == false)
        {
            MoveUp();
        }
        else
        {
            MoveDown();
        }
        
    }

    void MoveUp()
    {
        transform.Translate(Time.deltaTime * _projectileSpeed * Vector3.up );
        
        // if Y-Axis position is greater than 9.5, destroy laser object
        if (transform.position.y > 9.5f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }

            Destroy(this.gameObject);
        }
    }
    
    void MoveDown()
    {
        transform.Translate(Time.deltaTime * _projectileSpeed * Vector3.down );

        // if Y-Axis position is greater than -9.5, destroy laser object
        if (transform.position.y < -9.5f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }

            Destroy(this.gameObject);
        }
    }

    public void AssignEnemyLaser()
    {
        _isEnemyLaser = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && _isEnemyLaser == true)
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                // Call the Shake() method on _cameraShake reference
                _cameraShake.Shake(_cameraShakeStrength);
               player.Damage(); 
            }
        }
    }

    public float GetProjectileSpeed()
    {
        return _projectileSpeed;
    }
    
    public void SetProjectileSpeed(float speed)
    {
        _projectileSpeed = speed;
    }
}
