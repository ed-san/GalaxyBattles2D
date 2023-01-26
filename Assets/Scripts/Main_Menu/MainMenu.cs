using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void LoadGame()
    {
        //Load game
        SceneManager.LoadScene(1); // 1 - "Game" Scene
    }
}
