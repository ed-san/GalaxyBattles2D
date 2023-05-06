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
