using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipAssembler : MonoBehaviour
{
    public GameSettings.Team Team;

    int amount = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.Find<ShipPart>())
        {
            Destroy(other.transform.root.gameObject);
            amount++;
            if (amount > 6)
            {
                GameSettings.WinningTeam = Team;
                SceneLoader.ToEnd();
            }
        }
    }
}
