using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeWinningColor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<UnityEngine.UI.Image>().color = GameSettings.WinningTeam == GameSettings.Team.Red ? Color.red : Color.blue;   
    }
}

class ToEndSystem : GameSystem, Events.IOnConsoleInput
{
    public void OnConsoleInput(string[] consoleInput)
    {
        var message = consoleInput[0];
        if (message == "redwin")
        {
            GameSettings.WinningTeam = GameSettings.Team.Red;
            SceneLoader.ToEnd();
        }
        if (message == "bluewin")
        {
            GameSettings.WinningTeam = GameSettings.Team.Blue;
            SceneLoader.ToEnd();
        }
    }
}