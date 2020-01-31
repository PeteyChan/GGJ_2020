using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Environment")]
public class EnvironmentSettings : ScriptableObject
{
    public List<TerrainItem> Fields = new List<TerrainItem>();
    public List<DoodadItem> Doodads = new List<DoodadItem>();

    public int SizeX;
    public int SizeY;
    public int Seed;

    [System.Serializable]
    public class TerrainItem
    {
        public int Weighting;
        public GameObject TerrainType;
    }

    [System.Serializable]
    public class DoodadItem
    {
        public int Weighting;
        public GameObject Doodad;
        public bool Walkable;
    }

    List<GameObject> spawned = new List<GameObject>();

    public void GererateMap()
    {
        int totalWeight = 0;
        foreach (var item in Fields)
            totalWeight += item.Weighting;

        Random.InitState(Seed);

        for (int x = 0; x < SizeX; ++x)
            for (int z = 0; z < SizeY; ++z)
            {
                var target = Random.Range(0, totalWeight);
                var count = 0;
                foreach (var item in Fields)
                {
                    count += item.Weighting;
                    if (count > target)
                    {
                        if (item.TerrainType)
                            spawned.Add(Instantiate(item.TerrainType, new Vector3(x, 0, z), Quaternion.identity));
                        break;
                    }
                }
            }

        totalWeight = 0;
        foreach (var item in Doodads)
            totalWeight += item.Weighting;
        
        for (int x = 0; x < SizeX; ++x)
            for (int z = 0; z < SizeY; ++z)
            {
                var target = Random.Range(0, totalWeight);
                var count = 0;
                foreach (var item in Doodads)
                {
                    count += item.Weighting;
                    if (count > target)
                    {
                        if (item.Doodad)
                        {
                            var go = Instantiate(item.Doodad, new Vector3(x, 0, z), Quaternion.identity);
                            if (!item.Walkable)
                            {
                                go.AddComponent<Obstacle>();
                                go.AddComponent<BoxCollider>();
                            }
                            spawned.Add(go);
                        }
                        break;
                    }
                }
            }
    }

    public void ClearMap()
    {
        foreach (var obj in spawned)
        {
            if (Application.isPlaying)
                Destroy(obj);
            else DestroyImmediate(obj);
        }
    }
}

#if UNITY_EDITOR
namespace Inspectors
{
    using UnityEditor;

    class EnvironmentSettingsDrawer : DrawClass<EnvironmentSettings>
    {
        bool collapse;

        public override void Draw()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                if (GUILayout.Button("Generate Map"))
                {
                    target.ClearMap();
                    target.GererateMap();
                }

                if (GUILayout.Button("Clear Map"))
                {
                    target.ClearMap();
                }
            }

            GUILayout.Label("");
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Map Settings", EditorStyles.boldLabel);

                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Generate Seed", GUILayout.Width(EditorGUIUtility.labelWidth)))
                    {
                        target.Seed = Random.Range(int.MinValue, int.MaxValue);
                    }

                    target.Seed = EditorGUILayout.IntField(target.Seed);
                }
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("Map Size ", GUILayout.Width(EditorGUIUtility.labelWidth));
                    var size = Screen.width - EditorGUIUtility.labelWidth;
                    target.SizeX = EditorGUILayout.IntField(target.SizeX).clamp(10, 500);
                    target.SizeY = EditorGUILayout.IntField(target.SizeY).clamp(10, 500);
                }
            }

            GUILayout.Label("");
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("Terrain Type", EditorStyles.boldLabel, GUILayout.Width(Screen.width / 2f));
                    GUILayout.Label("Weighting", EditorStyles.boldLabel);
                }

                if (target.Fields.Count == 0)
                    target.Fields.Add(new EnvironmentSettings.TerrainItem());

                for (int i = 0; i < target.Fields.Count; ++i)
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        target.Fields[i].TerrainType = EditorGUILayout.ObjectField(GUIContent.none, target.Fields[i].TerrainType, typeof(GameObject), false, GUILayout.Width(Screen.width / 2f)) as GameObject;
                        target.Fields[i].Weighting = EditorGUILayout.IntField(target.Fields[i].Weighting);
                        if (target.Fields[i].Weighting < 1)
                            target.Fields[i].Weighting = 1;
                        if (GUILayout.Button("+", GUILayout.Width(24)))
                        {
                            target.Fields.Insert(i, new EnvironmentSettings.TerrainItem());
                            return;
                        }
                        if (GUILayout.Button("-", GUILayout.Width(24)))
                        {
                            target.Fields.RemoveAt(i);
                            return;
                        }
                    }
                }
            }

            GUILayout.Label("");

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                var size = (Screen.width / 2) - 90;
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("Doodad Type", EditorStyles.boldLabel, GUILayout.Width(Screen.width / 2f));
                    GUILayout.Label("Weighting", EditorStyles.boldLabel, GUILayout.Width(size / 2f));
                    GUILayout.Label("Walkable", EditorStyles.boldLabel, GUILayout.Width(size / 2f));
                }

                if (target.Doodads.Count == 0)
                    target.Doodads.Add(new EnvironmentSettings.DoodadItem());

                for (int i = 0; i < target.Doodads.Count; ++i)
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        target.Doodads[i].Doodad = EditorGUILayout.ObjectField(GUIContent.none, target.Doodads[i].Doodad, typeof(GameObject), false, GUILayout.Width(Screen.width / 2f)) as GameObject;
                        target.Doodads[i].Weighting = EditorGUILayout.IntField(target.Doodads[i].Weighting, GUILayout.Width(size / 2f));
                        target.Doodads[i].Walkable = EditorGUILayout.Toggle(target.Doodads[i].Walkable, GUILayout.Width(size/2f));
                        if (target.Doodads[i].Weighting < 1)
                            target.Doodads[i].Weighting = 1;
                        if (GUILayout.Button("+", GUILayout.Width(24)))
                        {
                            target.Doodads.Insert(i, new EnvironmentSettings.DoodadItem());
                            return;
                        }
                        if (GUILayout.Button("-", GUILayout.Width(24)))
                        {
                            target.Doodads.RemoveAt(i);
                            return;
                        }
                    }
                }
            }
            
            EditorUtility.SetDirty(target);
        }
    }
}
#endif