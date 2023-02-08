using System;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _astroRotationSpeed = 20.0f;

    [SerializeField] 
    private GameObject _asteroidExplosionPrefab;

    private SpawnManager _spawnManager;

    private void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
    }
    
    void Update()
    {
        RotateAsteroid();
    }

    void RotateAsteroid()
    {
        transform.Rotate(Time.deltaTime * _astroRotationSpeed * Vector3.forward);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser"))
        {
            Vector3 spawnPosition = this.transform.position;
            Instantiate(_asteroidExplosionPrefab, spawnPosition, Quaternion.identity);
            Destroy(other.gameObject);
            _spawnManager.StartSpawning();
            Destroy(this.gameObject, 0.2f); 
        }
    }
}
