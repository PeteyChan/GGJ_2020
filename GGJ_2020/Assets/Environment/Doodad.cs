using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
[Attributes.AlwaysRepaint]
public class Doodad : MonoBehaviour
{
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Game/SnapToGrid")]
    public static void Snap()
    {
        foreach (var go in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            var pos = go.transform.position;
            pos.x = Mathf.RoundToInt(pos.x);
            pos.y = Mathf.RoundToInt(pos.y);
            pos.z = Mathf.RoundToInt(pos.z);
        }
    }
#endif
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
                pos.y = 0;
                pos.z = Mathf.RoundToInt(pos.z);
                transform.position = pos;
            }
        }
    }
}
#endif