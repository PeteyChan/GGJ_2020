using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSpawner : MonoBehaviour
{
    public GameSettings.Team Team;
    // Start is called before the first frame update
    void Start()
    {
        var pos = new int2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
        for (int x = pos.x - 1; x < pos.x + 2; ++x)
            for (int y = pos.y - 1; y < pos.y + 2; ++y)
            {
                var go = Doodad.GetDoodadAtLocation(new int2(x, y));
                if (go)
                    Destroy(go);
            }
        GameSettings.GetPlayers(Team);
    }
}
