using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    int2 position;

    [SerializeField, HideInInspector] bool dynamic;

    [Attributes.GetSet]
    public bool isDynamic
    {
        get => dynamic;
        set
        {
            if (value != dynamic)
            {
                dynamic = value;
                if (dynamic)
                    ObstacleSystem.Add(this);
                else ObstacleSystem.Remove(this);
            }
        }
    }

    int index = -1;

    void OnEnable()
    {
        if (dynamic)
            ObstacleSystem.Add(this);

        var pos = transform.position;
        position = new int2(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.z));
        NavigatorMap.AddObstacle(position);
    }

    // Update is called once per frame
    void UpdatePosition()
    {
        var pos = transform.position;
        var current = new int2(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.z));
        if (current != position)
        {
            NavigatorMap.RemoveObstacle(position);
            NavigatorMap.AddObstacle(current);
            position = current;
        }
    }

    private void OnDisable()
    {
        if (dynamic)
            ObstacleSystem.Remove(this);
        NavigatorMap.RemoveObstacle(position);
    }

    class ObstacleSystem : GameSystem, Events.IOnUpdate, Events.IOnInspect
    {
        static List<Obstacle> obstacles = new List<Obstacle>();

        public static void Add(Obstacle obstacle)
        {
            obstacle.index = obstacles.Count;
            obstacles.Add(obstacle);
        }

        public static void Remove(Obstacle obstacle)
        {
            var index = obstacle.index;
            var last = obstacles.Count - 1;
            obstacles[index] = obstacles[last];
            obstacles[index].index = index;
            obstacles.RemoveAt(last);
        }

        public void OnInspect()
        {
            GUILayout.Label($"Dynamic Obstacles : {obstacles.Count}");
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var obstacle in obstacles)
                obstacle.UpdatePosition();
        }
    }
}
