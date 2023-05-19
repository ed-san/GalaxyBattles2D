using System;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;


public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _projectileSpeed = 10.0f;
    private bool _isEnemyLaser = false;
    // Add reference to CameraShake script
    private CameraShake _cameraShake;
    [SerializeField]
    private float _cameraShakeStrength = 5.0f;
    // Add a new field to store the direction in which the laser should move
    private Vector3 _direction;


    // Initialize _cameraShake reference in Start() method
    private void Start()
    {
        _cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();

        // Initialize direction based on whether this is an enemy laser or not
        if (_isEnemyLaser)
        {
            _direction = Vector3.down;
        }
        else
        {
            _direction = Vector3.up;
        }
    }
    
    void Update()
    {
        // Move in the direction specified by _direction
        Move();
    }
    
    public void AssignDirection(Vector3 direction)
    {
        _direction = direction;
        // Update the rotation of the laser object
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;  // Subtract 90 to orient the laser sprite correctly
        transform. rotation = Quaternion.Euler(0, 0, angle);
    }
    
    
    void Move()
    {
        transform.Translate(Time.deltaTime * _projectileSpeed * _direction);

        // Check if the laser is out of bounds and, if so, destroy it
        if (transform.position.y > 9.5f || transform.position.y < -9.5f)
        {
            DestroySelfAndParent();
        }
        

        // if gameObject is a laser from AoeEnemy and X-Axis position is less than -13.5 or greater than 13.5, destroy laser object
        if (gameObject.CompareTag("AoeLaser"))
        {
            // Check if X-Axis position is less than -13.5, destroy laser object
            if (transform.position.x < -16.0f)
            {
                Destroy(this.gameObject);
                DetachChildrenFromParent("AoeEnemyLaserParent");
            }
        
            // Check if X-Axis position is greater than 13.5, destroy laser object
            if (transform.position.x > 16.0f)
            {
                Destroy(this.gameObject);
                DetachChildrenFromParent("AoeEnemyLaserParent");
            }
        }
        
    }

    void DestroySelfAndParent()
    {
        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }

        Destroy(gameObject);
    }


    void DetachChildrenFromParent(string tag)
    {
        GameObject[] objectsToDetach = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject obj in objectsToDetach)
        {
            if (obj.transform.childCount > 0)
            {
                for (int i = obj.transform.childCount - 1; i >= 0; i--)
                {
                    obj.transform.GetChild(i).parent = null;
                }
                // Destroy the parent object after detaching its children
                Destroy(obj);
            }
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
               player.Damage(1); 
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
