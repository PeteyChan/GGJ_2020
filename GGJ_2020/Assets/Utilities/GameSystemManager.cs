using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using Events;

[Attributes.AlwaysRepaint]
public class GameManager : MonoBehaviour
{
    static GameManager instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        if (instance) return;
        instance = new GameObject("GameManager").AddComponent<GameManager>();
        DontDestroyOnLoad(instance);

        var types = new List<Type>(Implementors.GetTypes<GameSystem>());
        types.Sort((x, y) => ((int)x.GetCustomAttribute<Priority>()).CompareTo((int)y.GetCustomAttribute<Priority>()));

        AllGroups.Add(typeof(GameSystem), new Group<GameSystem>());

        foreach (var systemType in types)
        {
            var system = Activator.CreateInstance(systemType) as GameSystem;
            Group<GameSystem>.value.Add(system);

            foreach (var Interface in systemType.GetInterfaces())
            {
                if (!AllGroups.TryGetValue(Interface, out var group))
                {
                    AllGroups[Interface] = group = Activator.CreateInstance(typeof(Group<>).MakeGenericType(Interface)) as Group;
                }
                group.Add(system);
            }
        }

        foreach (var system in SystemsWith<IOnInitialize>())
            system.OnInitialize();
    }

    public static List<T> SystemsWith<T>() => Group<T>.value;
    static Dictionary<Type, Group> AllGroups = new Dictionary<Type, Group>();

    class Group
    {
        public virtual void Add(object obj) { }
        public virtual IList List { get; }
    }

    class Group<T> : Group
    {
        public override IList List => value;
        public override void Add(object obj) => value.Add((T)obj);
        public static List<T> value = new List<T>();
    }

    private void Update()
    {
        float delta = Time.deltaTime;
        foreach (var system in SystemsWith<IOnUpdate>())
            system.OnUpdate(delta);
    }

    private void FixedUpdate()
    {
        float delta = Time.fixedDeltaTime;
        foreach (var system in SystemsWith<IOnFixedUpdate>())
            system.OnFixedUpdate(delta);
    }

    private void OnGUI()
    {
        foreach (var system in SystemsWith<IOnGUI>())
            system.OnGUI();
    }

    private void OnDestroy()
    {
        foreach (var system in SystemsWith<IOnQuit>())
            system.OnQuit();
    }

    private void OnDrawGizmos()
    {
        foreach (var system in SystemsWith<IOnDrawGizmos>())
            system.OnDrawGizmos();
    }

#if UNITY_EDITOR
    class GameManagerWindow : UnityEditor.EditorWindow
    {
        [UnityEditor.MenuItem("Managers/GameManager")]
        static void ShowWindow()
        {
            var window = UnityEditor.EditorWindow.GetWindow<GameManagerWindow>();
            window.titleContent = new GUIContent("Game Manager");
            window.Show();
        }


        private void Update()
        {
            Repaint();
        }

        private void OnEnable()
        {
            so = null;
        }

        UnityEditor.SerializedObject so;
        GameManagerInspector inspector;

        private void OnGUI()
        {
            if (Application.isPlaying)
            {
                if (so == null)
                {
                    so = new UnityEditor.SerializedObject(instance);
                    inspector = new GameManagerInspector();
                    (inspector as Inspectors.IDrawer).SetUp(so);
                }
                inspector.Draw();
            }
            else
            {
                UnityEditor.EditorGUILayout.HelpBox("Game Manager is Runtime Only", UnityEditor.MessageType.Warning);
            }
        }
    }

    class GameManagerInspector : Inspectors.DrawClass<GameManager>
    {
        public string search = "";
        public Type filter = typeof(GameSystem);

        HashSet<Events.IOnInspect> inspected = new HashSet<IOnInspect>();
        public override void Draw()
        {
            float width = UnityEditor.EditorGUIUtility.labelWidth;
            
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Search Name", GUILayout.Width(width));
                search = GUILayout.TextField(search);
            }
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Filter", GUILayout.Width(width));
                if (GUILayout.Button(filter == null ? "No Filter" : filter.Name, UnityEditor.EditorStyles.toolbarDropDown))
                {
                    var menu = new UnityEditor.GenericMenu();
                    var items = new List<Type>(AllGroups.Keys);
                    items.Remove(typeof(GameSystem));
                    items.Sort((x, y) => y.Name.CompareTo(x.Name));
                    items.Add(typeof(GameSystem));

                    for (int i = items.Count - 1; i >= 0; --i)
                    {
                        var item = items[i];
                        menu.AddItem(new GUIContent(item.Name), filter == item, () => filter = item);
                    }
                    menu.ShowAsContext();
                }
            }

            if (AllGroups.TryGetValue(filter, out var group))
            {
                using (new GUILayout.VerticalScope(UnityEditor.EditorStyles.helpBox))
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        GUILayout.Label("Priority", UnityEditor.EditorStyles.boldLabel, GUILayout.Width(64f));
                        GUILayout.Label("Game System", UnityEditor.EditorStyles.boldLabel);
                    }

                    foreach (var item in group.List)
                    {
                        if (item.GetType().Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            using (new GUILayout.HorizontalScope())
                            {
                                GUILayout.Label(((int)item.GetType().GetCustomAttribute<Priority>()).ToString(), GUILayout.Width(64f));

                                using (new GUILayout.VerticalScope())
                                {
                                    if (GUILayout.Button(item.GetType().Name, UnityEditor.EditorStyles.label))
                                    {
                                        if (item is IOnInspect inspectable)
                                        {
                                            if (inspected.Contains(inspectable))
                                                inspected.Remove(inspectable);
                                            else inspected.Add(inspectable);
                                        }

                                    }

                                    if (inspected.Contains(item as IOnInspect))
                                        using (new GUILayout.VerticalScope(UnityEditor.EditorStyles.helpBox))
                                        {
                                            (item as IOnInspect).OnInspect();
                                        }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
#endif
}

public abstract class GameSystem
{
    /// <summary>
    /// Returns a list of all systems with interface. Use GameSystem to return all GameSystems
    /// </summary>
    public IReadOnlyList<T> SystemsWith<T>() =>
        GameManager.SystemsWith<T>();
}

/// <summary>
/// Orders system execution from lowest to highest priority
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Class)]
class Priority : System.Attribute
{
    public Priority(int Value)
    {
        this.Value = Value;
    }

    public int Value;
    public static implicit operator int(Priority priority)
    {
        if (priority == null) return 0;
        return priority.Value;
    }
}

namespace Events
{
    /// <summary>
    /// Adds an Update callback to the game system
    /// </summary>
    public interface IOnUpdate { void OnUpdate(float deltaTime); }

    /// <summary>
    /// Adds a fixed update callback to the game system
    /// </summary>
    public interface IOnFixedUpdate { void OnFixedUpdate(float deltaTime); }

    /// <summary>
    /// Adds an OnGUI callback to the game system
    /// </summary>
    public interface IOnGUI { void OnGUI(); }

    /// <summary>
    /// How to draw the System when clicked on in the inspector
    /// </summary>
    public interface IOnInspect { void OnInspect(); }

    /// <summary>
    /// Called once just after system creation
    /// </summary>
    public interface IOnInitialize { void OnInitialize(); }

    /// <summary>
    /// Called once just before application quits
    /// </summary>
    public interface IOnQuit { void OnQuit(); }

    /// <summary>
    /// Adds OnDrawGizmos Callback to game system
    /// </summary>
    public interface IOnDrawGizmos { void OnDrawGizmos(); }
}