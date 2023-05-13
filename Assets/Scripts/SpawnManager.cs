using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField] 
    private GameObject _aoeEnemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] _powerups;
    [SerializeField]
    private float _spawnRate = 5.0f;
    private bool _stopSpawning = false;
    [SerializeField] 
    private float _specialShotSpawnRate = 30.0f;
    [SerializeField] 
    private float _aoeEnemySpawnRate = 30.0f;
    [SerializeField] 
    private GameObject _specialShotPrefab;
    private float _waveDuration = 10.0f;
    private int _wave = 1;
    private float _maxSpawnRate = .25f;
    private float _spawnDecreaseRate = 0.05f;
    private UIManager _uiManager;

    private void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        
        if(_uiManager == null)
        {
            Debug.LogError("UIManager is NULL!");
        }
        
        
    }

    public void StartSpawning()
    {
        StartCoroutine(AoeEnemySpawnRoutine(_aoeEnemySpawnRate));
        StartCoroutine(WaveManagement(_wave, _waveDuration));
        StartCoroutine(SpawnPowerupRoutine());
        StartCoroutine(SpecialBlastSpawnRoutine(_specialShotSpawnRate));
    }

    IEnumerator WaveManagement(int _wave, float _waveDuration)
    {
        if (_stopSpawning)
        {
            yield break;
        }
        
        float updatedSpawnRate = SpawnRateAssignment();
        _spawnRate = updatedSpawnRate;
        Enemy.MovementType  movementType = MovementTypeAssignment(_wave);
        StartCoroutine(SpawnEnemyRoutine(_spawnRate, _waveDuration, movementType));
        

        yield return new WaitForSeconds(_waveDuration);

        _waveDuration = WaveDurationAssignment(_waveDuration);
        _wave++;

        _uiManager.UpdateWaveCount(_wave);

        // Call the WaveManagement() again after the specified waveDuration
        StartCoroutine(WaveManagement(_wave, _waveDuration));
    }

    public float SpawnRateAssignment()
    {
        if (_spawnRate - _spawnDecreaseRate >= _maxSpawnRate)
        {
            _spawnRate -= _spawnDecreaseRate;
        }

        return _spawnRate;
    }

    public Enemy.MovementType MovementTypeAssignment(int wave)
    {
        Enemy.MovementType movementType;

        switch (wave)
        {
            case int w when w >= 1 && w <= 5: //original values: 1:5
                movementType = Enemy.MovementType.StraightDown;
                break;
            case int w when w >= 6 && w <= 10://original values: 6:10
                movementType = Enemy.MovementType.Circle;
                break;
            case int w when w >= 11 && w <= 15://original values: 11:15
                movementType = Enemy.MovementType.Angle;
                break;
            case int w when w >= 16 && w <= 20://original values: 16:20
                movementType = Enemy.MovementType.SineWave;
                break;
            case int w when w > 20:
                movementType = (Enemy.MovementType)Random.Range(0, 4); // Choose a random MovementType for waves greater than 20
                break;
            default:
                Debug.LogError("Invalid wave number!");
                movementType = Enemy.MovementType.StraightDown;
                break;
        }

        return movementType;
    }
    
    private float WaveDurationAssignment(float waveDuration)
    {
        const float increment = 2.0f;
        const float maxWaveDuration = 60.0f;

        float newWaveDuration = waveDuration + increment;
        if (newWaveDuration > maxWaveDuration)
        {
            newWaveDuration = maxWaveDuration;
        }

        return newWaveDuration;
    }

 
    /// <summary>
    ///Method spawns enemies at random positions within the specified wave duration,
    ///assigning them a given movement type while maintaining a defined time interval between spawns.
    /// The method stops spawning enemies once the total spawning time reaches the wave duration.
    /// </summary>
    IEnumerator SpawnEnemyRoutine(float spawnRate, float waveDuration, Enemy.MovementType movementType)
    {
        float timeSpentSpawning = 0.0f;

        while (timeSpentSpawning < waveDuration)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-10.14f, 10.14f), 12.0f, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, spawnPosition, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;

            // Assign the given movement type to the spawned enemy
            Enemy enemyScript = newEnemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.SetMovementType(movementType);
            }

            // Wait for the specified spawn rate
            yield return new WaitForSeconds(_spawnRate);
            timeSpentSpawning += _spawnRate;
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

    IEnumerator SpecialBlastSpawnRoutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        
        while (_stopSpawning == false) 
        { 
            Vector3 spawnPowPosition = new Vector3(Random.Range(-10.14f, 10.14f), 12.0f, 0);
            Instantiate(_specialShotPrefab, spawnPowPosition, Quaternion.identity);
            yield return new WaitForSeconds(30.0f);
        }
        
    }
    
    private IEnumerator AoeEnemySpawnRoutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        
        while (_stopSpawning == false) 
        { 
            Vector3 spawnPosition = new Vector3(Random.Range(-10.14f, 10.14f), 12.0f, 0);
            GameObject aoeEnemy = Instantiate(_aoeEnemyPrefab, spawnPosition, Quaternion.identity);
            aoeEnemy.transform.parent = _enemyContainer.transform;
            //Debug.Log("AoeEnemy Spawned called at: " + Time.time);
            Enemy aoeEnemyScript = aoeEnemy.GetComponent<Enemy>();
            if (aoeEnemyScript != null)
            {
                aoeEnemyScript.SetMovementType(Enemy.MovementType.Angle); // Set the desired movement type
            }
            yield return new WaitForSeconds(30.0f);
        }
        
    }
    
    public int GetWave
    {
        get { return _wave; }
    }
    
}
