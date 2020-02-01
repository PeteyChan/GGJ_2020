using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/ShipParts")]
public class ShipPartPrefabs : ScriptableObject
{
    static ShipPartPrefabs instance;
    public static ShipPartPrefabs Instance
    {
        get {
            if (!instance) instance = Resources.Load<ShipPartPrefabs>("ShipParts");
            return instance;
        }
    }

    public GameObject LandingGear;
    public List<GameObject> OtherParts = new List<GameObject>();

}
