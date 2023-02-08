using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] _powerups;
    [SerializeField]
    private float _spawnRate = 5.0f;
    private bool _stopSpawning = false;

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine(_spawnRate));
        StartCoroutine(SpawnPowerupRoutine());
    }
 
    IEnumerator SpawnEnemyRoutine(float waitTime)
    {
        yield return new WaitForSeconds(1.5f);
        
        while (_stopSpawning == false)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-10.14f,10.14f), 12.0f,0);
            GameObject newEnemy = Instantiate(_enemyPrefab, spawnPosition, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(waitTime);
        }
        
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(1.5f);
        
        while (_stopSpawning == false) 
        { 
            Vector3 spawnPowPosition = new Vector3(Random.Range(-10.14f, 10.14f), 12.0f, 0);
            int numberOfPowerups = _powerups.Length;
            int powerupSelection = Random.Range(0, numberOfPowerups);
            GameObject player = GameObject.Find("Player");
            GameObject shield = player.transform.GetChild(0).gameObject;

            if (shield.activeSelf && powerupSelection == 2)
            {
                powerupSelection = Random.Range(0, 2);
                Instantiate(_powerups[powerupSelection], spawnPowPosition, Quaternion.identity);
            } else
            {
                Instantiate(_powerups[powerupSelection], spawnPowPosition, Quaternion.identity);
            }

            yield return new WaitForSeconds(Random.Range(5.0f, 7.0f));
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
