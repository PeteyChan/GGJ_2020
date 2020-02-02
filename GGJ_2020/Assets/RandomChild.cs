using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomChild : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        var childCount = transform.childCount;
        foreach (Transform child in transform)
        {
            if (child == transform) continue;
            child.gameObject.SetActive(false);
        }
        transform.GetChild(Random.Range(0, childCount)).gameObject.SetActive(true);
    }
}
