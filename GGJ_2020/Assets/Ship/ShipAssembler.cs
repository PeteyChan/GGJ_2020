using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipAssembler : MonoBehaviour
{
    public GameSettings.Team Team;

    int amount = 0;

    HashSet<int> gatheredParts = new HashSet<int>();

    private void Start()
    {
        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            if (Team == GameSettings.Team.Red)
                renderer.sharedMaterial = Resources.Load<Material>("Red");
            else
                renderer.sharedMaterial = Resources.Load<Material>("Blue");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryFind(out ShipPart part))
        {
            if (!gatheredParts.Contains(part.partID) && part.Holder.Team == Team)
            {
                part.transform.root.position = transform.position + part.snapPosition;
                part.transform.root.rotation = Quaternion.identity;
                gatheredParts.Add(part.partID);

                part.gameObject.transform.DetachChildren();
                Destroy(part);
            }

            if (gatheredParts.Count >= 6)
            {
                GameSettings.WinningTeam = Team;
                SceneLoader.ToEnd();
            }
        }
    }
}
