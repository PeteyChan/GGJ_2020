using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(StateMachine))]

public class Player : MonoBehaviour
{
    static List<Player> players = new List<Player>();

    private void Awake()
    {
        players.Add(this);
    }

    private void OnDestroy()
    {
        players.Remove(this);
    }

    private void Start()
    {
        if (players.Count >= 3)
        {
            var rect = new Rect();
            rect.width = .5f;
            rect.height = .5f;
            switch (GamePad.player)
            {
                case 0:
                    rect.position = new Vector2(0, 0);
                    break;
                case 1:
                    rect.position = new Vector2(.5f, 0);
                    break;
                case 2:
                    rect.position = new Vector2(0, .5f);
                    break;
                case 3:
                    rect.position = new Vector2(0.5f, .5f);
                    break;
            }
            gameObject.Find<Camera>().rect = rect;
        }

        if (players.Count == 2)
        {
            Rect rect = new Rect();
            rect.width = 1f;
            rect.height = .5f;
            if (players[0] == this)
                rect.position = new Vector2(0, 0);
            else rect.position = new Vector2(0, .5f);

            gameObject.Find<Camera>().rect = rect;
        }

        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            if (Team == GameSettings.Team.Blue)
                renderer.sharedMaterial = Resources.Load<Material>("Blue");
            else renderer.sharedMaterial = Resources.Load<Material>("Red");
        }

        if (Physics.Raycast(new Ray(transform.position, Vector3.down),out var hit, 10f))
        {
            transform.position = hit.point;
        }
    }


    public GamePad GamePad = new GamePad(0);

    public GameSettings.Team Team;

    public ShipPart HeldPart;

    public List<ShipPart> NearbyParts = new List<ShipPart>();

    public ShipPart GetPart()
    {
        NearbyParts.RemoveAll(x => x == null);
        foreach (var part in NearbyParts)
            if (part.Holder == null)
                return part;
        return null;
    }
}
