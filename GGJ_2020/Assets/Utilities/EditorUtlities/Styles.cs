using System.Collections;
using System.Collections.Generic;
using UnityEngine;

# if UNITY_EDITOR
namespace Inspectors
{
    using UnityEditor;

    public static class Styles
    {
        public static GUIStyle Header
        {
            get
            {
                var style = new GUIStyle(EditorStyles.toolbar);

                style.alignment = TextAnchor.MiddleCenter;
                style.normal.textColor = Color.white;
                style.normal.background = PixelTexture.Get(new Color(.1f, .1f, .1f));
                style.stretchWidth = true;
                style.fontSize = 14;
                style.fontStyle = FontStyle.Bold;
                return style;
            }
        }
        public static GUIStyle Header2
        {
            get
            {
                var h = Header;
                h.normal.background = PixelTexture.Get(new Color(.2f, .2f, .2f));
                return h;
            }
        }
        public static GUIStyle HeaderError
        {
            get
            {
                var h = Header;
                h.normal.background = PixelTexture.Get(new Color(.6f, .1f, .1f));
                return h;
            }
        }
        public static GUIStyle HeaderHighLight
        {
            get
            {
                var h = Header;
                h.normal.background = PixelTexture.Get(new Color(.6f, .6f, .1f));
                return h;
            }
        }
    }
}
#endif