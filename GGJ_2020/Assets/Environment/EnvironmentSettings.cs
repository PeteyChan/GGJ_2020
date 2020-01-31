using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Environment")]
public class EnvironmentSettings : ScriptableObject
{
    [SerializeField] List<FieldItem> Fields = new List<FieldItem>();

    int totalWeight;
    

    [System.Serializable]
    public struct FieldItem
    {
        public int SpawnWieght;
        public GameObject SpawnType;
    }
}
