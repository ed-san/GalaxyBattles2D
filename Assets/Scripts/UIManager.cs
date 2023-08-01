using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{

    [SerializeField] 
    private TMP_Text _scoreText;
    [SerializeField]
    private TMP_Text _ammoText;
    [SerializeField]
    private TMP_Text _waveText;
    [SerializeField]
    private TMP_Text _waveTextCounter;
    [SerializeField] 
    private Image _livesImage;
    [SerializeField] 
    private Sprite[] _liveSprites;
    [SerializeField] 
    private TMP_Text _gameOverText;
    [SerializeField] 
    private TMP_Text _restartText;
    [SerializeField] 
    private TMP_Text _restartBossText;
    [SerializeField] 
    private TMP_Text _quitToMainMenuText;
    [SerializeField] 
    private TMP_Text _wonTheLevelText;
    
    private GameManager _gameManager;
    [SerializeField]
    private Player _player;
    private SpawnManager _spawnManager;
    [SerializeField] 
    private TextUIEffects _waveTextPulse;
    private BossController _bossController;

    void Start()
    {
        _scoreText.text = "Score: " + 0;
        _ammoText.text = $"Energy: {_player.AmmoCount}";
        _waveTextCounter.text = "Wave: " + 0;
        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);
        _restartBossText.gameObject.SetActive(false);
        _quitToMainMenuText.gameObject.SetActive(false);
        _wonTheLevelText.gameObject.SetActive(false);
        
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

        if (_gameManager == null)
        {
            Debug.LogError("Game_Manager is NULL");
        }
        
        if (_spawnManager == null)
        {
            Debug.LogError("Spawn_Manager is NULL");
        }
        
    }
    

    public string UpdateScore(int score)
    {
        return _scoreText.text = "Score: " + score;
    }

    public string UpdateAmmoCount(int ammo)
    {
        return _ammoText.text = $"Energy: {ammo}";
    }

    public string UpdateWaveCount(int wave)
    {
        _waveText.text = $"Wave: {wave}";
        _waveTextCounter.text = $"Wave: {wave}";
        // Call the ShowAndPulseWaveText() method to display and animate the wave text
        if (_waveTextPulse != null)
        {
            _waveTextPulse.ShowAndPulseWaveText();
        }

        return _waveText.text;
        
    }
        

    public void UpdateLives(int currentLives)
    {
        int clampedLives = Mathf.Clamp(currentLives, 0, _liveSprites.Length - 1);
        _livesImage.sprite = _liveSprites[clampedLives];

        if (currentLives <= 0)
        {
            GameOverSequence();
        }
    }

    private void GameOverSequence()
    {
        if (_spawnManager.IsBossSpawned == true)
        {
            _gameOverText.gameObject.SetActive(true);
            _restartBossText.gameObject.SetActive(true);
            _quitToMainMenuText.gameObject.SetActive(true);
            _gameManager.GameOver();
        }
        else
        {
            _gameOverText.gameObject.SetActive(true);
            _restartText.gameObject.SetActive(true);
            _gameManager.GameOver();
        }
        
        StartCoroutine(GameOverFlickerRoutine());
    }

    IEnumerator GameOverFlickerRoutine()
    {
        while (true)
        {
            _gameOverText.text = "GAME OVER";
            yield return new WaitForSeconds(.5f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(.5f);
        }
    }
    
    IEnumerator LevelCompleteFlickerRoutine()
    {
        while (true)
        {
            _wonTheLevelText.text = "Victory - Level 1 Complete!";
            yield return new WaitForSeconds(.5f);
            _wonTheLevelText.text = "";
            yield return new WaitForSeconds(.5f);
        }
    }
    
    public void ResetGameOverUI()
    {
        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);
        _restartBossText.gameObject.SetActive(false);
        _quitToMainMenuText.gameObject.SetActive(false);
    }

    public void DisplayLevelWon()
    {
        _wonTheLevelText.gameObject.SetActive(true);
        _quitToMainMenuText.gameObject.SetActive(true);
        StartCoroutine(LevelCompleteFlickerRoutine());
    }


}
