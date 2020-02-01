using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Winner : MonoBehaviour
{
    private void Start()
    {
        GetComponent<UnityEngine.UI.Text>().text = $"{(GameSettings.WinningTeam == GameSettings.Team.Red ? "Red" : "Blue")} WINS!!";
        for (int i = 0; i < 4; ++i)
            pads.Add(new GamePad(i));
    }
    float exitTIme;

    List<GamePad> pads = new List<GamePad>();

    private void Update()
    {
        exitTIme += Time.deltaTime;
        if (exitTIme > 15)
            SceneLoader.ToTitleScreen();
        if (exitTIme > 4)
            foreach (var pad in pads)
                if (pad.GetButton(GamePad.Buttons.start).wasPressed)
                    SceneLoader.ToTitleScreen();

    }
}
