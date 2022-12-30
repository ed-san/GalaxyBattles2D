using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{   

    [SerializeField]
    private float _projectileSpeed = 10.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CalculateLaserMovement();
        
        // if Y-Axis position is greater than 8.0, destroy laser object
        if (transform.position.y >= 8.0f)
        {
            Destroy(gameObject);
        }
    }

    void CalculateLaserMovement()
    {
        transform.Translate(Time.deltaTime * _projectileSpeed * Vector3.up );
    }
    
}
