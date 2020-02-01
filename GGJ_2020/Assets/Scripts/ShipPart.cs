using UnityEngine;

public class ShipPart : MonoBehaviour
{
    public Vector3 snapPosition;
    public GameSettings.Team team;
    public int partID;

    Player holder;
    public Player Holder
    {
        get => holder;
        set {
            holder = value;
            currentState = state.pickedup;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryFind(out Player player))
        {
            player.NearbyParts.Add(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryFind(out Player player))
        {
            player.NearbyParts.Remove(this);
        }
    }

    float elapsed;

    public enum state
    {
        falling,
        pickedup,
        grounded,
        despawn
    }

    public state currentState = state.falling;

    private void Update()
    {
        switch (currentState)
        {
            case state.pickedup:
                if (Holder == null)
                {
                    currentState = state.falling;
                    break;
                }
                transform.position = Holder.transform.position + Vector3.up;
                transform.rotation = Holder.GetComponentInChildren<Animator>().transform.rotation;
                return;

            case state.falling:
                {
                    var pos = transform.position;
                    pos.y -= Time.deltaTime * 7f;
                    transform.position = pos;
                    if (pos.y < 0)
                    {
                        pos.y = 0;
                        currentState = state.grounded;
                    }
                }
                return;

            case state.grounded:
                elapsed += Time.deltaTime;
                if (elapsed > 20)
                {
                    currentState = state.despawn;
                }
                return;

            case state.despawn:
                {
                    var pos = transform.position;
                    pos.y -= Time.deltaTime * 2f;
                    if (pos.y < -5)
                        Destroy(transform.root.gameObject);
                    transform.position = pos;
                }
                return;
        }
    }

    private void OnDestroy()
    {
        transform.rotation = Quaternion.identity;
    }
}
