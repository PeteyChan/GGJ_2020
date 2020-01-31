using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
[Attributes.AlwaysRepaint]
public class Doodad : MonoBehaviour
{

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