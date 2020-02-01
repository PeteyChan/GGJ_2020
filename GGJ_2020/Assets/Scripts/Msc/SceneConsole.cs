using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneConsole : GameSystem, Events.IOnConsoleInput
{
    public void OnConsoleInput(string[] consoleInput)
    {
        var message = consoleInput[0];

        switch (message)
        {
            case "main":
                SceneLoader.ToTitleScreen();
                return;
            case "redwin":
                GameSettings.WinningTeam = GameSettings.Team.Red;
                SceneLoader.ToEnd();
                return;
            case "bluewin":
                GameSettings.WinningTeam = GameSettings.Team.Blue;
            SceneLoader.ToEnd();
                break;
            case "game":
                int player = 1;
                if (consoleInput.Length >= 2)
                    int.TryParse(consoleInput[0], out player);
                for (int i = 0; i < player; ++i)
                {
                    var info = GameSettings.GetPlayerInfo(i);
                    info.Playing = true;
                    info.Team = player >= 2 ? GameSettings.Team.Blue : GameSettings.Team.Red;
                }
                SceneLoader.ToGame();
                break;
        }
    }
}
