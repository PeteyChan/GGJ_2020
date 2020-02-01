using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildLevel : MonoBehaviour
{
    public List<EnvironmentSettings> Settings = new List<EnvironmentSettings>();
    
    // Start is called before the first frame update
    void Awake()
    {
        Settings.RemoveAll(x => x == null);
        Settings[Random.Range(0, Settings.Count)].GererateMap();
    }
}
