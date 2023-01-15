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
   
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemyRoutine(_spawnRate));
        StartCoroutine(SpawnPowerupRoutine());
    }
 
    IEnumerator SpawnEnemyRoutine(float waitTime)
    {
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
        while (_stopSpawning == false) 
        { 
            Vector3 spawnPowPosition = new Vector3(Random.Range(-10.14f, 10.14f), 12.0f, 0);
            int numberOfPowerups = _powerups.Length;
            int powerupSelection = Random.Range(0, numberOfPowerups);

            GameObject newPowerup = Instantiate(_powerups[powerupSelection], spawnPowPosition, Quaternion.identity);

            //Lines: 54-61 Code below destroys a spawned powerup if it's a shield and the player already has one enabled.
            GameObject player = GameObject.Find("Player");
            GameObject shield = player.transform.GetChild(0).gameObject;
            GameObject powerup = GameObject.FindWithTag("Powerups");
            Powerup powerupScriptValues = powerup.transform.GetComponent<Powerup>();
            int powerupID = powerupScriptValues.GetPowerupID();

            if (shield.activeSelf && powerupID == 2)
            {
                Destroy(newPowerup);    
            }
            yield return new WaitForSeconds(Random.Range(3.0f, 6.0f));
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }
}
