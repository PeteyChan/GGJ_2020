using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToTeamSelect : MonoBehaviour
{
    GamePad[] gamepads = new GamePad[4];

    private void Start()
    {
        for (int i = 0; i < gamepads.Length; ++i)
            gamepads[i] = new GamePad(i);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var pad in gamepads)
            if (pad.GetButton(GamePad.Buttons.start).wasPressed)
                SceneLoader.ToCharacterSelect();
    }
}
