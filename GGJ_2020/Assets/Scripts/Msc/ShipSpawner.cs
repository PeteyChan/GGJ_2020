using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSpawner : MonoBehaviour
{
    public static List<ShipSpawner> spawners = new List<ShipSpawner>();
    

    public GameSettings.Team Team;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return null;
        yield return null;
        transform.position = new Vector3(GameSettings.MapSize.x / 2, 0, GameSettings.MapSize.y / 2);

        if (Team == GameSettings.Team.Red)
            transform.position = new Vector3(-transform.position.x, 0, transform.position.z);

        var pos = new int2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
        for (int x = pos.x - 3; x < pos.x + 4; ++x)
            for (int y = pos.y - 3; y < pos.y + 4; ++y)
            {
                var go = Doodad.GetDoodadAtLocation(new int2(x, y));
                if (go)
                    Destroy(go);
            }

        foreach (var (playerID, info) in GameSettings.GetPlayers(Team))
        {
            var go = Instantiate(Resources.Load<GameObject>("Player"));
            go.Find<Player>().GamePad = new GamePad(playerID);
            go.transform.position = new Vector3(pos.x > 0 ? pos.x - 1 : pos.x +1 , 1, pos.y + playerID);
        }

        Instantiate(ShipPartPrefabs.Instance.LandingGear, transform.position, Quaternion.identity).AddComponent<ShipAssembler>().Team = Team;
    }
}
