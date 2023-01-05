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
    private float _spawnRate = 5.0f;
    private bool _stopSpawning = false;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnRoutine(_spawnRate));
    }
    
    IEnumerator SpawnRoutine(float waitTime)
    {
        while (_stopSpawning == false)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-10.14f,10.14f), 12.0f,0);
            GameObject newEnemy = Instantiate(_enemyPrefab, spawnPosition, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(waitTime);
        }
        
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
