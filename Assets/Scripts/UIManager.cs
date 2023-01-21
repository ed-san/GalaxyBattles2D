using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{

    [SerializeField]
    private TMP_Text _scoreText;

    // Start is called before the first frame update
    void Start()
    {
        _scoreText.text = "Score: " + 0;
        
    }

    public string UpdateScore(int score)
    {
        return _scoreText.text = "Score: " + score;
    }
}
