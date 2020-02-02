using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMaterial : MonoBehaviour
{

    public bool propogate;
    public List<Material> materials = new List<Material>();

    // Start is called before the first frame update
    void Start()
    {
        var mat = materials[Random.Range(0, materials.Count)];

        if (propogate)
            foreach (var renderer in GetComponentsInChildren<Renderer>())
                renderer.sharedMaterial = mat;

        if (TryGetComponent(out Renderer r))
        {
            r.sharedMaterial = materials[Random.Range(0, materials.Count)];
        }


    }
}
