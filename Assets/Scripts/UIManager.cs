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
    private Image _livesImage;
    [SerializeField]
    private Sprite[] _liveSprites;

    // Start is called before the first frame update
    void Start()
    {
        _scoreText.text = "Score: " + 0;
        
    }

    public string UpdateScore(int score)
    {
        return _scoreText.text = "Score: " + score;
    }

    public void UpdateLives(int currentLives)
    {
        //display image sprite
        //give it a new image based on currentLives index value
        _livesImage.sprite = _liveSprites[currentLives];

    }
}
