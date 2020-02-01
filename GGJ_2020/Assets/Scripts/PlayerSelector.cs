using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelector : MonoBehaviour
{
    public int Player;
    GamePad pad;

    RectTransform rect;

    // Start is called before the first frame update
    void Start()
    {
        GameSettings.GetPlayerInfo(Player).Playing = false;

        pad = new GamePad(Player);
        foreach (Transform t in transform)
        {
            if (t == transform) continue;
            t.gameObject.SetActive(false);
        }

        rect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (pad.GetButton(GamePad.Buttons.start).wasPressed)
        {
            if (GameSettings.GetPlayerInfo(Player).Playing && GameSettings.CanStart())
            {
                SceneLoader.ToGame();
                return;
            }
            foreach (Transform t in transform)
                t.gameObject.SetActive(true);

            GameSettings.GetPlayerInfo(Player).Playing = true;
        }
        var pos = transform.localPosition;

        float delta = Time.deltaTime * 10;

        if (GameSettings.GetPlayerInfo(Player).Team == GameSettings.Team.Red)
        {
            pos = Vector3.Lerp(pos, new Vector3(-200, pos.y, pos.z), delta);
        }
        else pos = Vector3.Lerp(pos, new Vector3(200, pos.y, pos.z), delta);

        transform.localPosition = pos;

        if (pad.GetButton(GamePad.Buttons.lJoyStick_left).wasPressed)
            ChangeTeam(GameSettings.Team.Red);
        if (pad.GetButton(GamePad.Buttons.dpad_left).wasPressed)
            ChangeTeam(GameSettings.Team.Red);
        if (pad.GetButton(GamePad.Buttons.dpad_right).wasPressed)
            ChangeTeam(GameSettings.Team.Blue);
        if (pad.GetButton(GamePad.Buttons.lJoyStick_right).wasPressed)
        {
            ChangeTeam(GameSettings.Team.Blue);
        }
            
    }

    void ChangeTeam(GameSettings.Team team)
    {
        GameSettings.GetPlayerInfo(Player).Team = team;
    }
}
