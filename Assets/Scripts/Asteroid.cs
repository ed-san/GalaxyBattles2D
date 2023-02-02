using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _astroRotationSpeed = 20.0f;

    [SerializeField] private GameObject _asteroidExplosionPrefab;

    // Update is called once per frame
    void Update()
    {
        RotateAsteroid();
    }

    void RotateAsteroid()
    {
        //transform.Translate(Time.deltaTime * _astroRotationSpeed * Vector2.down, Space.World);
        //transform.Rotate(0,0, 180 * Time.deltaTime);
        transform.Rotate(Vector3.forward * _astroRotationSpeed * Time.deltaTime);
    }
    
    //Check for Laser collision (Trigger)
    //Instantiate the explosion object at the position of the asteroid (us).

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Laser"))
        {   
            Destroy(this.gameObject);    
            Vector3 spawnPosition = this.transform.position;
            Instantiate(_asteroidExplosionPrefab, spawnPosition, Quaternion.identity);
        }
    }
}
