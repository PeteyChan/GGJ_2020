using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
[Attributes.AlwaysRepaint]
public class Doodad : MonoBehaviour
{
    public static Dictionary<int2, GameObject> lookup = new Dictionary<int2, GameObject>();

    public static GameObject GetDoodadAtLocation(int2 position)
    {
        if (lookup.TryGetValue(position, out var val))
            return val;
        return null;
    }

    public static GameObject GetDoodadAtLocation(Vector3 position)
    {
        var pos = new int2(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));
        if (lookup.TryGetValue(pos, out var val))
            return val;
        return null;

    }

    int2 pos => new int2(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));

    private void OnEnable()
    {
        if (lookup.TryGetValue(pos, out var go))
        {
            Destroy(go);
        }
        lookup[pos] = gameObject;
    }

    private void OnDisable()
    {
        lookup[pos] = null;
    }
}

#if UNITY_EDITOR
namespace Inspectors
{
    class DoodadDrawer : DrawClass<Doodad>
    {
        public override void Draw()
        {
            foreach (Transform transform in target.transform)
            {
                if (transform == target.transform) continue;

                var pos = target.transform.position;
                pos.x = Mathf.RoundToInt(pos.x);
                pos.y = transform.position.y;
                pos.z = Mathf.RoundToInt(pos.z);
                transform.position = pos;
            }
        }
    }
}
#endif