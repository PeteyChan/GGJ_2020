using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SceneField : ISerializationCallbackReceiver
{
#if UNITY_EDITOR
    public UnityEditor.SceneAsset sceneAsset;
#endif

#pragma warning disable 414
    [SerializeField, HideInInspector]
    private string sceneName = "";
#pragma warning restore 414

    // Makes it work with the existing Unity methods (LoadLevel/LoadScene)
    public static implicit operator string(SceneField sceneField)
    {
#if UNITY_EDITOR
        return System.IO.Path.GetFileNameWithoutExtension(UnityEditor.AssetDatabase.GetAssetPath(sceneField.sceneAsset));
#else
        return sceneField.sceneName;
#endif
    }

    public void OnBeforeSerialize()
    {
#if UNITY_EDITOR
        sceneName = this;
#endif
    }
    public void OnAfterDeserialize() { }
}

# if UNITY_EDITOR
namespace Inspectors
{
    using UnityEditor;

    [CustomPropertyDrawer(typeof(SceneField))]
    public class SceneFieldPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty sceneAssetProp = property.FindPropertyRelative("sceneAsset");

            EditorGUI.BeginProperty(position, label, sceneAssetProp);
            EditorGUI.PropertyField(position, sceneAssetProp, label);
            EditorGUI.EndProperty();
        }
    }
} 
#endif