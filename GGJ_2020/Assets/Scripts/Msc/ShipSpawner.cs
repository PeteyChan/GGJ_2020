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
            go.TryFind(out Player player);
            player.GamePad = new GamePad(playerID);
            player.Team = Team;
            go.transform.position = new Vector3(pos.x > 0 ? pos.x - 1 : pos.x +1 , 1, pos.y + playerID);
        }

        Instantiate(ShipPartPrefabs.Instance.LandingGear, transform.position, Quaternion.identity).Find<ShipAssembler>().Team = Team;
        new GameObject("Part Spawner").AddComponent<PartSpawner>().SpawnerPosition = transform.position;
    }

    class PartSpawner : MonoBehaviour
    {
        public Vector3 SpawnerPosition;

        float elapsed;
        float target;
        private void Update()
        {
            elapsed += Time.deltaTime;
            if (elapsed > target)
            {

                int2 spawn = new int2(Mathf.Abs(Mathf.RoundToInt(SpawnerPosition.x)), Mathf.Abs(Mathf.RoundToInt(SpawnerPosition.z)));
                var mapsize = GameSettings.MapSize;

                int2 targetPos = new int2(Random.Range(-mapsize.x, mapsize.x), Random.Range(0, mapsize.y));

                if (Mathf.Abs(spawn.x - Mathf.Abs(targetPos.x)) < 5)
                    return;
                if (Mathf.Abs(spawn.y - Mathf.Abs(targetPos.y)) < 5)
                    return;
                if (Doodad.GetDoodadAtLocation(targetPos)?.Find<Obstacle>())
                    return;
                var parts = ShipPartPrefabs.Instance.OtherParts;
                Instantiate(parts[Random.Range(0, parts.Count)], new Vector3(targetPos.x, 10, targetPos.y), Quaternion.identity);

                elapsed = 0;
                target = Random.Range(2f, 7f);
            }
        }
    }
}

