using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    public string loadLevel;
    public void startGame()
    {
        Application.LoadLevel(loadLevel);
    }

    public void quitGame()
    {

        Application.Quit();


    }
}
