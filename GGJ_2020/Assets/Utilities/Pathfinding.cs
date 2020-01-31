using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Navigator
{
    public int2 StartPosition;
    public int2 EndPosition;
    public List<int2> Path = new List<int2>();
    bool pendingUpdate;

    public void QueueUpdate()
    {
        if (pendingUpdate) return;

        pendingUpdate = true;
        PathfindingManager.navigators.Enqueue(this);
    }

    class PathfindingManager : GameSystem, Events.IOnUpdate, Events.IOnInspect
    {
        public static Queue<Navigator> navigators = new Queue<Navigator>();

        Dictionary<int2, (int distance, int2 previous)> nodes = new Dictionary<int2, (int distance, int2 previous)>();
        PriorityQueue<int2> openSet = new PriorityQueue<int2>();
        
        int2[] directions = new int2[]
        {
            new int2(1, 0),
            new int2(0, 1),
            new int2(-1, 0),
            new int2(0, -1)
        };

        public void OnInspect()
        {
            GUILayout.Label($"Pending Updates: {navigators.Count}");
            GUILayout.Label($"Update Time: {executionTime : 0.000}ms");
        }

        double executionTime;

        static System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        
        public void OnUpdate(float deltaTime)
        {
            watch.Reset();
            watch.Start();

            if (navigators.Count == 0) return;

            nodes.Clear();
            openSet.Clear();

            var navigator = navigators.Dequeue();
            navigator.pendingUpdate = false;
            navigator.Path.Clear();

            var start = navigator.StartPosition;
            var goal = navigator.EndPosition;

            if (start == goal)
                return;

            nodes[start] = (0, start);
            openSet.Add(start, 0);

            int2 closestNode = start;
            int closestNodeDistance = ToGoal(start);

            int count = 0;
            while (count < 100 && closestNodeDistance > 0)
            {
                count++;

                var previous = openSet.Dequeue();
                int distanceTraveled = nodes[previous].distance + 1;

                foreach (var direction in directions)
                {
                    var pos = previous + direction;
                    if (NavigatorMap.IsWalkable(pos) && !nodes.ContainsKey(pos))
                    {
                        nodes[pos] = (distanceTraveled, previous);
                        if (pos == goal)
                        {
                            closestNode = pos;
                            closestNodeDistance = 0;
                            break;
                        }
                        var togoal = ToGoal(pos);
                        if (togoal < closestNodeDistance)
                        {
                            closestNode = pos;
                            closestNodeDistance = togoal;
                        }

                        openSet.Add(pos, togoal + distanceTraveled);
                        nodes[pos] = (distanceTraveled, previous);
                    }                    
                }
            }

            while (true)
            {
                var (distance, previous) = nodes[closestNode];
                if (distance == 0)
                    break;
                navigator.Path.Add(closestNode);
                closestNode = previous;
            }

            int ToGoal(int2 waypoint)
            {
                var diff = navigator.EndPosition - waypoint;
                return Mathf.Abs(diff.x) + Mathf.Abs(diff.y);
            }

            watch.Stop();
            executionTime = watch.Elapsed.TotalMilliseconds;
        }
    }
}

class NavigatorMap : GameSystem, Events.IOnDrawGizmos, Events.IOnInspect
{
    static Dictionary<int2, int> obstacles = new Dictionary<int2, int>();

    public static void AddObstacle(int2 position)
    {
        if (!obstacles.TryGetValue(position, out var val))
            obstacles[position] = 1;
        else
            obstacles[position] = val + 1;
    }

    public static void RemoveObstacle(int2 position)
    {
        if (obstacles.TryGetValue(position, out var val))
        {
            obstacles[position] = val - 1;
        }
    }

    public static bool IsWalkable(int2 position)
    {
        if (obstacles.TryGetValue(position, out var amount))
            return amount < 1;
        return true;
    }

    public void OnDrawGizmos()
    {
        if (DebugWalkable)
            foreach (var item in obstacles)
            {
                if (item.Value > 0)
                {
                    var pos = item.Key;
                    Gizmos.DrawCube( new Vector3(pos.x, 0, pos.y) , Vector3.one);
                }
            }
    }

    public void OnInspect()
    {
        if (GUILayout.Button(DebugWalkable ? "Hide Obstacles" : "Show Obstacles"))
            DebugWalkable = !DebugWalkable;
    }

    bool DebugWalkable;
}