using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipAssembler : MonoBehaviour
{
    public GameSettings.Team Team;

    HashSet<int> gatheredParts = new HashSet<int>();

    private void Start()
    {
        AudioSource audio = Instantiate(new GameObject()).AddComponent<AudioSource>();
        audio.clip = GameSounds.Instance.BackgroundMusic;
        audio.loop = true;
        audio.Play();

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
        if (other.gameObject.TryFind(out Player player))
        {
            var part = player.HeldPart;
            if (!part) return;

            if (!gatheredParts.Contains(part.partID) && part.Holder.Team == Team)
            {
                part.transform.root.position = transform.position + part.snapPosition;
                part.transform.root.rotation = Quaternion.identity;
                gatheredParts.Add(part.partID);

                AudioSource.PlayClipAtPoint(GameSounds.Instance.PartPlacement, transform.position);
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
