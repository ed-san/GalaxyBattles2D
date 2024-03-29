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
    private Image _livesImage;
    [SerializeField] 
    private Sprite[] _liveSprites;
    [SerializeField] 
    private TMP_Text _gameOverText;
    [SerializeField] 
    private TMP_Text _restartText;
    private GameManager _gameManager;
    [SerializeField]
    private Player _player;

    void Start()
    {
        _scoreText.text = "Score: " + 0;
        _ammoText.text = $"Energy: {_player.AmmoCount}";
        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<GameManager>();

        if (_gameManager == null)
        {
            Debug.LogError("Game_Manager is NULL");
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
        

    public void UpdateLives(int currentLives)
    {
        int clampedLives = Mathf.Clamp(currentLives, 0, _liveSprites.Length - 1);
        _livesImage.sprite = _liveSprites[clampedLives];

        if (currentLives == 0)
        {
            GameOverSequence();
        }
    }

    private void GameOverSequence()
    {
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());
        _gameManager.GameOver();
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

}
