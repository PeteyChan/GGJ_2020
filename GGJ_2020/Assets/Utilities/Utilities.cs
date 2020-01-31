using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public static class Utilities
{
    /// <summary>
    /// Attempts to find the first component of type starting from the root gameobject, returns true if found
    /// </summary>
    public static bool TryFind<T>(this GameObject gameObject, out T component) where T : Component
    {
        var t = gameObject.transform.root;
        component = t.GetComponentInChildren<T>();
        return component;
    }

    /// <summary>
    /// Tries to find the first component of type starting from the root gameobject. Adds that component to the root gameobject if none found
    /// </summary>
    public static T Find<T>(this GameObject gameObject) where T : Component
    {
        return gameObject.transform.root.GetComponentInChildren<T>();
    }

    public static T GetOrAddComponent<T>(this GameObject target) where T : Component
    {
        if (!target.TryGetComponent<T>(out var val))
            return target.AddComponent<T>();
        return val;
    }

    public static T GetOrAddComponent<T>(this Component target) where T : Component
    {
        if (!target.TryGetComponent<T>(out var val))
            return target.gameObject.AddComponent<T>();
        return val;
    }

    public static float Tilt(this Vector2 value)
    {
        var x = value.x;
        var y = value.y;
        return new Vector2(x * Mathf.Sqrt(1 - .5f * y * y), y * Mathf.Sqrt(1 - .5f * x * x)).magnitude;
    }

    public static float clamp(this float value, float min, float maxInclusive)
    {
        if (value < min) value = min;
        if (value > maxInclusive) value = maxInclusive;
        return value;
    }

    public static int clamp(this int value, int min, int maxInclusive)
    {
        if (value < min) value = min;
        if (value > maxInclusive) value = maxInclusive;
        return value;
    }

    public static T Random<T>(this List<T> list)
        => list[UnityEngine.Random.Range(0, list.Count)];
}

public static class EnumUtility
{
    static class EnumLookup<T> where T : System.Enum
    {
        static EnumLookup()
        {
            names = Enum.GetNames(typeof(T));
            values = (T[])Enum.GetValues(typeof(T));
        }

        public static T[] values;
        public static string[] names;
    }

    /// <summary>
    /// returns all values in <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T[] GetValues<T>() where T : System.Enum
    {
        return EnumLookup<T>.values;
    }

    /// <summary>
    /// Returns all names in <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static string[] GetNames<T>() where T : System.Enum
    {
        return EnumLookup<T>.names;
    }
}

static class PixelTexture
{
    static Dictionary<Color, Texture2D> textures = new Dictionary<Color, Texture2D>();
    public static Texture2D Get(Color color)
    {
        if (!textures.TryGetValue(color, out var texture) || texture == null)
        {
            textures[color] = texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, color);
            texture.Apply();
        }
        return texture;
    }
}

static class TypeID
{
    static Dictionary<Type, int> Type_ID = new Dictionary<Type, int>();
    static Dictionary<int, Type> ID_Type = new Dictionary<int, Type>();

    public static int Get(Type type)
    {
        if (!Type_ID.TryGetValue(type, out var value))
        {
            Type_ID[type] = value = Type_ID.Count;
            ID_Type[Type_ID.Count] = type;
        }
        return value;
    }

    public static bool TryGetType(int ID, out Type type)
    {
        if (!ID_Type.TryGetValue(ID, out type))
            return false;
        return true;
    }

    class ID<T>
    {
        public static int Value = Get(typeof(T));
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static int Get<T>() => ID<T>.Value;
}

static class Implementors
{
    static Dictionary<Type, Type[]> Lookup = new Dictionary<Type, Type[]>();

    /// <summary>
    /// Returns all non abstract classes that implements T
    /// </summary>
    public static Type[] GetTypes<T>()
    {
        var type = typeof(T);
        if (!Lookup.TryGetValue(type, out var types))
        {
            Lookup[type] = types = type.Assembly.GetTypes().Where(x => !x.IsAbstract && !x.IsGenericType && x.IsSubclassOf(type)).ToArray();
            Array.Sort(types, (x, y) => x.Name.CompareTo(y.Name));
        }
        return types;
    }
}

static class PerformanceTest
{
    static System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

    /// <summary>
    /// Returns time taken to perfrom the test in milliseconds
    /// </summary>
    /// <param name="name">Name of the test</param>
    /// <param name="action">Action to perform</param>
    /// <returns></returns>
    public static double Start(string name, Action action, bool logToConsole = true)
    {
        watch.Reset();
        watch.Start();
        action();
        watch.Stop();
        if (logToConsole)
            Debug.Log($"{name} {watch.Elapsed.TotalMilliseconds}ms");
        return watch.Elapsed.TotalMilliseconds;
    }
}

public class PriorityQueue<T>
{
    Node First;
    public int Count
    {
        get; private set;
    }

    Stack<Node> Pool = new Stack<Node>();
    Node GetFreeNode(T item, float priority)
    {
        Node node;
        Count++;
        if (Pool.Count > 0)
        {
            node = Pool.Pop();
        }
        else node = new Node();

        node.Next = null;
        node.item = item;
        node.priority = priority;
        return node;
    }

    public T Dequeue()
    {
        if (First == null)
            return default;
        var item = First.item;
        First.item = default;
        Pool.Push(First);
        First = First.Next;
        Count--;
        return item;
    }

    public void Clear()
    {
        Node node = First;
        while (node != null)
        {
            Pool.Push(node);
            node.item = default;
            node = node.Next;
        }
        Count = 0;
        First = null;
    }

    public void Add(T item, float priority)
    {
        var node = GetFreeNode(item, priority);
        if (First == null)
        {
            First = node;
            return;
        }

        if (node.priority <= First.priority)
        {
            node.Next = First;
            First = node;
            return;
        }

        First.AddNode(node);
    }

    public List<T> ToList()
    {
        var list = new List<T>();

        Node node = First;
        while (node != null)
        {
            list.Add(node.item);
            node = node.Next;
        }
        return list;
    }

    class Node
    {
        public T item;
        public float priority;
        public Node Next;

        public void AddNode(Node node)
        {
            if (Next == null)
            {
                Next = node;
                return;
            }
            if (node.priority <= Next.priority)
            {
                node.Next = Next;
                Next = node;
                return;
            }

            Next.AddNode(node);
        }
    }
}