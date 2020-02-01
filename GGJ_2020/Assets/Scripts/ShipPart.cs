using UnityEngine;

public class ShipPart : MonoBehaviour
{
    public Vector3 snapPosition;
    public GameSettings.Team team;
    public int partID;

    public Player Holder;

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

    float fallTime;
    float elapsed;

    private void Update()
    {
        if (Holder)
        {
            transform.position = Holder.transform.position + Vector3.up;
            transform.rotation = Holder.GetComponentInChildren<Animator>().transform.rotation;
            return;
        }

        elapsed += Time.deltaTime;

        if (transform.position.y > 0)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - (10 * fallTime * fallTime), transform.position.z);
            fallTime += Time.deltaTime;
        }

        if (elapsed >= 30)
        {
            transform.position += Vector3.down * 10f * Time.deltaTime;
        }

        if (transform.position.y < -5)
            Destroy(transform.root.gameObject);
    }

    private void OnDestroy()
    {
        transform.rotation = Quaternion.identity;
    }
}
