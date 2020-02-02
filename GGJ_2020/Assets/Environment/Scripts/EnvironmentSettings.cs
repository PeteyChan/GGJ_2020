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
    public int TerrainSeed;
    public int DoodadSeed;

    public bool MirrorTerrain;
    public bool MirrorDoodads;

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

    // Map //

    public void GererateMap()
    {
        GenerateTerrain();
        GenerateDoodads();
    }

    public void ClearMap()
    {
        ClearTerrain();
        ClearDoodads();
    }

    // Terrain //

    public void GenerateTerrain()
    {
        ClearTerrain();
        GenerateBounds();
        Random.InitState(TerrainSeed);
        int totalWeight = 0;
        foreach (var item in Fields)
            totalWeight += item.Weighting;

        void SpawnTerrain(int x, int y, bool mirror = false)
        {
            var target = Random.Range(0, totalWeight);
            var count = 0;
            foreach (var item in Fields)
            {
                count += item.Weighting;
                if (count > target)
                {
                    if (item.TerrainType)
                    {
                        var go = Instantiate(item.TerrainType, new Vector3(x, 0, y), Quaternion.identity);
                        if (mirror && x != 0)
                        {
                            go = Instantiate(go, new Vector3(-x, 0, y), Quaternion.identity);
                        }
                    }
                    break;
                }
            }
        }

        for (int x = 0; x < SizeX; ++x)
            for (int y = 0; y < SizeY; ++y)
            {
                if (MirrorTerrain)
                    SpawnTerrain(x, y, true);
                else
                {
                    SpawnTerrain(x, y);
                    if (x != 0)
                        SpawnTerrain(-x, y);
                }
            }
    }

    public void ClearTerrain()
    {
        foreach (GameObject obj in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (obj.layer != Layers.Terrain) continue;
            if (Application.isPlaying)
                Destroy(obj);
            else DestroyImmediate(obj);
        }
    }

    // Doodads //

    public void GenerateDoodads()
    {
        ClearDoodads();
        Random.InitState(DoodadSeed);

        var totalWeight = 0;

        void SpawnDoodad(int x, int y, bool mirror = false)
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
                        var go = Instantiate(item.Doodad, new Vector3(x, 0, y), Quaternion.identity);
                        go.AddComponent<Doodad>();
                        if (!item.Walkable)
                        {
                            var collider = new GameObject();
                            collider.AddComponent<Obstacle>();
                            collider.AddComponent<CapsuleCollider>().height = 2f;
                            collider.transform.parent = go.transform;
                            collider.transform.localPosition = Vector3.zero;
                            go.layer = Layers.Doodads;
                        }
                        if (mirror && x != 0)
                        {
                            Instantiate(go, new Vector3(-x, 0, y), Quaternion.identity);
                        }
                    }
                    break;
                }
            }
        }

        foreach (var item in Doodads)
            totalWeight += item.Weighting;

        for (int x = 0; x < SizeX; ++x)
            for (int y = 0; y < SizeY; ++y)
            {
                if (MirrorDoodads && x != 0)
                {
                    SpawnDoodad(x, y, true);
                }
                else
                {
                    SpawnDoodad(x, y);
                    if (x != 0)
                        SpawnDoodad(-x, y);
                }
            }
    }
    
    public void ClearDoodads()
    {
        foreach (var obj in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (obj.layer != Layers.Doodads) continue;
            if (Application.isPlaying)
                Destroy(obj);
            else DestroyImmediate(obj);
        }
    }

    // Bounds //

    void GenerateBounds()
    {
        //right bounds
        var barrier = new GameObject().AddComponent<BoxCollider>();
        barrier.gameObject.layer = Layers.Terrain;
        barrier.name = "Bounds";
        barrier.transform.position = new Vector3(SizeX, 0, SizeY/2);
        barrier.size = new Vector3(1, 5, SizeY + 2);
        //left bounds;
        barrier = Instantiate(barrier, new Vector3(-SizeX, 0, SizeY/2), Quaternion.identity);
        //top bounds;
        barrier = Instantiate(barrier, new Vector3(0, 0, SizeY), Quaternion.identity);
        barrier.size = new Vector3((SizeX * 2) + 2, 5, 1);
        //bot bounds
        barrier = Instantiate(barrier, new Vector3(0, 0, -1), Quaternion.identity);

        var cliff = Resources.Load<GameObject>("Cliff");
        for (int x = -SizeX; x < SizeX+25; x += 50)
        {
            Instantiate(cliff, new Vector3(x, 0, SizeY), Quaternion.identity);
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
                GUILayout.Label("Build", EditorStyles.boldLabel);

                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Generate Map"))
                    {
                        target.GererateMap();
                    }

                    if (GUILayout.Button("Randomize Map"))
                    {
                        target.TerrainSeed = Random.Range(int.MinValue, int.MaxValue);
                        target.DoodadSeed = Random.Range(int.MinValue, int.MaxValue);
                        target.GererateMap();
                    }

                    if (GUILayout.Button("Clear Map"))
                    {
                        target.ClearMap();
                    }

                }

                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Generate Terrain"))
                        target.GenerateTerrain();
                    if (GUILayout.Button("Randomize Terrain"))
                    {
                        target.TerrainSeed = Random.Range(int.MinValue, int.MaxValue);
                        target.GenerateTerrain();
                    }
                    if (GUILayout.Button("Clear Terrain"))
                    {
                        target.ClearTerrain();
                    }
                }

                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Generate Doodads"))
                        target.GenerateDoodads();
                    if (GUILayout.Button("Randomize Doodads"))
                    {
                        target.DoodadSeed = Random.Range(int.MinValue, int.MaxValue);
                        target.GenerateDoodads();
                    }
                    if (GUILayout.Button("Clear Doodads"))
                        target.ClearDoodads();
                }
            }

            GUILayout.Label("");
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Map Settings", EditorStyles.boldLabel);

                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Terrain Seed", GUILayout.Width(EditorGUIUtility.labelWidth)))
                    {
                        target.TerrainSeed = Random.Range(int.MinValue, int.MaxValue);
                    }

                    target.TerrainSeed = EditorGUILayout.IntField(target.TerrainSeed);
                }

                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Doodad Seed", GUILayout.Width(EditorGUIUtility.labelWidth)))
                    {
                        target.DoodadSeed = Random.Range(int.MinValue, int.MaxValue);
                    }

                    target.DoodadSeed = EditorGUILayout.IntField(target.DoodadSeed);
                }


                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("Map Size ", GUILayout.Width(EditorGUIUtility.labelWidth));
                    var size = Screen.width - EditorGUIUtility.labelWidth;
                    target.SizeX = EditorGUILayout.IntField(target.SizeX).clamp(10, 500);
                    target.SizeY = EditorGUILayout.IntField(target.SizeY).clamp(10, 500);
                }

                target.MirrorTerrain = EditorGUILayout.Toggle("Mirror Terrain", target.MirrorTerrain);
                target.MirrorDoodads = EditorGUILayout.Toggle("Mirror Doodads", target.MirrorDoodads);
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
                        var type = target.Fields[i].TerrainType;
                        target.Fields[i].TerrainType = EditorGUILayout.ObjectField(GUIContent.none, type, typeof(GameObject), false, GUILayout.Width(Screen.width / 2f)) as GameObject;
                        if (type != target.Fields[i].TerrainType)
                        {
                            foreach (Transform item in target.Fields[i].TerrainType.GetComponentInChildren<Transform>())
                                item.gameObject.layer = Layers.Terrain;
                        }

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
                    GUILayout.Label("Weighting", EditorStyles.boldLabel);
                    GUILayout.Label("Walkable", EditorStyles.boldLabel);
                    GUILayout.Label("", GUILayout.Width(48));
                }

                if (target.Doodads.Count == 0)
                    target.Doodads.Add(new EnvironmentSettings.DoodadItem());

                for (int i = 0; i < target.Doodads.Count; ++i)
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        var type = target.Doodads[i].Doodad;
                        target.Doodads[i].Doodad = EditorGUILayout.ObjectField(GUIContent.none, type, typeof(GameObject), false, GUILayout.Width(Screen.width / 2f)) as GameObject;
                        if (type != target.Doodads[i].Doodad)
                        {
                            foreach (Transform item in target.Doodads[i].Doodad.GetComponentInChildren<Transform>())
                                item.gameObject.layer = Layers.Doodads;
                        }


                        target.Doodads[i].Weighting = EditorGUILayout.IntField(target.Doodads[i].Weighting);
                        target.Doodads[i].Walkable = EditorGUILayout.Toggle(target.Doodads[i].Walkable);
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

    class EnvironmentBUilder : EditorWindow
    {
        IDrawer drawer;

        [MenuItem("Game/Environment Builder")]
        static void OpenWindow()
        {
            var window = EditorWindow.CreateWindow<EnvironmentBUilder>();
        }

        Editor Editor;

        private void OnGUI()
        {
            if (drawer == null)
            {
                var so = new UnityEditor.SerializedObject(Resources.Load("EnvironmentBuilder"));
                drawer = new EnvironmentSettingsDrawer();
                drawer.SetUp(so);
            }
            else drawer.Draw();
        }
    }
}
#endif