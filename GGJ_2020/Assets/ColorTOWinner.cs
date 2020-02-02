using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTOWinner : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var mat = GameSettings.WinningTeam == GameSettings.Team.Blue ? Resources.Load<Material>("Blue") : Resources.Load<Material>("Red");
        foreach (var renderer in GetComponentsInChildren<Renderer>())
            renderer.sharedMaterial = mat;
    }
    
}
