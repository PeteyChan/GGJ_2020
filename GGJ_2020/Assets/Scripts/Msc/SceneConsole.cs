using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneConsole : GameSystem, Events.IOnConsoleInput
{
    public void OnConsoleInput(string[] consoleInput)
    {
        if (consoleInput[0] == "main")
            SceneLoader.ToTitleScreen();

    }
}
