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
