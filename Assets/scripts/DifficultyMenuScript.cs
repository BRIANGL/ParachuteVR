using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyMenuScript : MonoBehaviour
{
    public string loadLevel;
    public void startGame()
    {
        Application.LoadLevel(loadLevel);
    }
    public void BigTarget (){
        loadLevel = "BigTarget";
        startGame();
    }
    public void MediumTarget()
    {
        loadLevel = "MediumTarget";
        startGame();
    }
    public void SmallTarget()
    {
        loadLevel = "SmallTarget";
        startGame();
    }

    public void quitGame()
    {

        Application.Quit();


    }
}
