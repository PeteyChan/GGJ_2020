#if UNITY_EDITOR
namespace Inspectors.Intenal
{
    using System.Reflection;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    class DrawSpace : DrawAttribute<Attributes.Space>
    {
        public override void Draw()
        {
            EditorGUILayout.LabelField(GUIContent.none);
        }
    }

    class DrawScriptField : DrawAttribute<Attributes.ShowScriptField>
    {
        public override void Draw()
        {
            GUI.enabled = false;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            GUI.enabled = true;
        }
    }

    class DrawLine : DrawAttribute<Attributes.Line>
    {
        public override void Draw()
        {
            EditorGUILayout.LabelField(GUIContent.none, GUI.skin.horizontalSlider);
        }
    }

    class DrawHelpBox : DrawAttribute<Attributes.HelpBox>
    {
        public override void Draw()
        {
            EditorGUILayout.HelpBox(attribute.message, (MessageType)attribute.type);
        }
    }

    class DrawLabel : DrawAttribute<Attributes.Label>
    {
        public override void Draw()
        {
            var target = serializedObject.targetObject;
            string name = memberInfo.Name;
            object value;
            if (memberInfo is PropertyInfo prop && prop.CanRead)
            {
                value = prop.GetValue(target);
            }
            else value = (memberInfo as FieldInfo).GetValue(target);

            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(name, GUILayout.Width(EditorGUIUtility.labelWidth));
                if (GUILayout.Button(value == null ? "" : value.ToString(), EditorStyles.label))
                {
                    if (value is UnityEngine.Object obj)
                        EditorGUIUtility.PingObject(obj);
                }
            }
        }
    }

    class DrawGetSet : DrawAttribute<Attributes.GetSet>
    {
        PropertyInfo prop;

        public override void Init()
        {
            prop = memberInfo as PropertyInfo;
        }

        public override void Draw()
        {
            var target = serializedObject.targetObject;

            if (!(prop.CanRead || prop.CanWrite)) return;

            if (prop.PropertyType == typeof(bool))
                prop.SetValue(target, EditorGUILayout.Toggle(prop.Name, (bool)prop.GetValue(target)));

            if (prop.PropertyType == typeof(int))
                prop.SetValue(target, EditorGUILayout.IntField(prop.Name, (int)prop.GetValue(target)));

            if (prop.PropertyType == typeof(float))
                prop.SetValue(target, EditorGUILayout.FloatField(prop.Name, (float)prop.GetValue(target)));

            if (prop.PropertyType == typeof(string))
                prop.SetValue(target, EditorGUILayout.TextField(prop.Name, (string)prop.GetValue(target)));

            serializedObject.Update();
        }
    }

    class DrawButton : DrawAttribute<Attributes.Button>
    {
        object arguement = null;

        MethodInfo method;

        public override void Init()
        {
            method = memberInfo as MethodInfo;
        }

        public override void Draw()
        {
            var parameters = method.GetParameters();
            if (parameters.Length == 0)
            {
                if (GUILayout.Button(method.Name))
                    method.Invoke(serializedObject.targetObject, null);
                return;
            }

            if (parameters.Length == 1)
            {
                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button(method.Name, GUILayout.Width(EditorGUIUtility.labelWidth)))
                    {
                        method.Invoke(serializedObject.targetObject, new object[] { arguement });
                    }

                    var type = parameters[0].ParameterType;

                    if (type == typeof(bool))
                        arguement = EditorGUILayout.Toggle(arguement == null ? false: (bool)arguement);
                    else if (type == typeof(int))
                        arguement = EditorGUILayout.IntField(arguement == null ? 0: (int)arguement);
                    else if (type == typeof(float))
                        arguement = EditorGUILayout.FloatField(arguement == null ? 0: (float)arguement);
                    else if (type == typeof(string))
                        arguement = EditorGUILayout.TextField((string)arguement);
                    else if (type == typeof(Vector2))
                        arguement = EditorGUILayout.Vector2Field(GUIContent.none, arguement == null ? new Vector2(): (Vector2)arguement);
                    else if (type == typeof(Vector3))
                        arguement = EditorGUILayout.Vector3Field(GUIContent.none, arguement == null ? new Vector3(): (Vector3)arguement);
                    else if (type.IsEnum)
                    {
                        if (arguement == null)
                            arguement = System.Activator.CreateInstance(type);
                        arguement = EditorGUILayout.EnumPopup(GUIContent.none, arguement as System.Enum);
                    }
                    else if (type == typeof(Color))
                    {
                        arguement = EditorGUILayout.ColorField(arguement == null ? Color.white : (Color)arguement);
                    }
                    else if (type == typeof(Vector4))
                        arguement = EditorGUILayout.Vector4Field(GUIContent.none, arguement == null ? (Vector4)arguement : new Vector4());
                    else if (typeof(UnityEngine.Object).IsAssignableFrom(type))
                        arguement = EditorGUILayout.ObjectField(GUIContent.none, arguement as UnityEngine.Object, type, true);
                }
            }
        }
    }
    
    class DrawHeader : DrawAttribute<Attributes.Header>
    {
        public override void Draw()
        {
            EditorGUILayout.LabelField(attribute.message, EditorStyles.boldLabel);
        }
    }

    class DrawQuaternion : DrawField<Quaternion>
    {
        bool foldout;

        public override void Draw()
        {
            foldout = EditorGUILayout.Foldout(foldout, fieldInfo.Name, true);
            EditorGUI.indentLevel++;

            if (foldout)
            {
                var val = fieldValue;
                val.w = EditorGUILayout.FloatField("w", val.w);
                val.x = EditorGUILayout.FloatField("x", val.x);
                val.y = EditorGUILayout.FloatField("y", val.y);
                val.z = EditorGUILayout.FloatField("z", val.z);
                serializedObject.FindProperty(fieldInfo.Name).quaternionValue = val;
            }
            EditorGUI.indentLevel--;
        }
    }
}
#endif