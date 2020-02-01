using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
namespace Inspectors
{
    using System;
    using System.Reflection;
    using UnityEditor;

    public abstract class DrawField<T> : IDrawer
    {
        public SerializedObject serializedObject;
        public FieldInfo fieldInfo;

        public T fieldValue
        {
            get => (T)fieldInfo.GetValue(serializedObject.targetObject);
            set => fieldInfo.SetValue(serializedObject.targetObject, value);
        }

        public virtual void Init() { }

        public virtual void Draw()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(fieldInfo.Name));
        }

        void IDrawer.SetUp(params object[] parameters)
        {
            serializedObject = parameters[0] as SerializedObject;
            fieldInfo = parameters[1] as FieldInfo;
            Init();
        }
    }

    public abstract class DrawClass<T> : IDrawer where T : UnityEngine.Object
    {
        protected SerializedObject serializedObject;
        protected T target { get; private set; }

        public virtual void Init() { }

        IDrawer drawer;
        public virtual void Draw()
        {
            if (drawer == null)
            {
                drawer = new Inspectors.Intenal.DrawDefaultClass();
                drawer.SetUp(serializedObject);
            }
            drawer.Draw();
        }

        void IDrawer.SetUp(params object[] parameters)
        {
            serializedObject = parameters[0] as SerializedObject;
            target = serializedObject.targetObject as T;
            Init();
        }
    }

    public abstract class DrawAttribute<T> : IDrawer
        where T : Attribute
    {
        public T attribute;
        public MemberInfo memberInfo;
        public SerializedObject serializedObject;

        public virtual void Init()
        { }

        public virtual void Draw()
        { }

        void IDrawer.SetUp(params object[] parameters)
        {
            attribute = (T)parameters[0];
            memberInfo = (MemberInfo)parameters[1];
            serializedObject = (SerializedObject)parameters[2];
            Init();
        }
    }

    public interface IDrawer
    {
        void SetUp(params object[] parameters);
        void Draw();
    }

    namespace Intenal
    {
            static class Drawers
            {
                static Drawers()
                {
                    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                        foreach (var type in assembly.GetTypes())
                        {
                            if (!type.IsAbstract && type.BaseType != null && type.BaseType.IsGenericType)
                            {
                                var arguments = type.BaseType.GetGenericArguments()[0];
                                var definition = type.BaseType.GetGenericTypeDefinition();
                                if (definition == typeof(DrawAttribute<>))
                                    AttributeDrawers.Add(arguments, type);
                                else if (definition == typeof(DrawField<>))
                                    FieldDrawers.Add(arguments, type);
                                else if (definition == typeof(DrawClass<>))
                                    ClassDrawers.Add(arguments, type);
                            }
                        }
                    //Debug.Log($" attr: {AttributeDrawers.Count} field: {FieldDrawers.Count} class: {ClassDrawers.Count}");
                }

                static Dictionary<Type, Type> AttributeDrawers = new Dictionary<Type, Type>();
                static Dictionary<Type, Type> ClassDrawers = new Dictionary<Type, Type>();
                static Dictionary<Type, Type> FieldDrawers = new Dictionary<Type, Type>();

                public static bool TryGetAttributeDrawer(Type type, out IDrawer drawer)
                {
                    if (AttributeDrawers.TryGetValue(type, out var drawType))
                    {
                        drawer = Activator.CreateInstance(drawType) as IDrawer;
                        return true;
                    }
                    drawer = null;
                    return false;
                }

                public static bool TryGetClassDrawer(Type type, out IDrawer drawer)
                {
                    while (true)
                    {
                        if (ClassDrawers.TryGetValue(type, out var drawType))
                        {
                            drawer = Activator.CreateInstance(drawType) as IDrawer;
                            return true;
                        }
                        type = type.BaseType;
                    }
                }

                public static bool TryGetFieldDrawer(Type type, out IDrawer drawer)
                {
                    if (!FieldDrawers.TryGetValue(type, out var drawerType))
                    {
                        if (type.IsValueType)
                        {
                            drawer = new DrawDefaultField();
                            return true;
                        }
                        else
                        {
                            while (drawerType == null)
                            {
                                type = type.BaseType;
                                FieldDrawers.TryGetValue(type, out drawerType);
                            }
                        }
                    }
                    drawer = Activator.CreateInstance(drawerType) as IDrawer;
                    return true;
                }
            }

#if true//false
        [CustomEditor(typeof(UnityEngine.Object), true)]
        class CustomUnityInspector : Editor
        {

            public bool alwaysRepaint;
            bool useDefault;

            private void OnEnable()
            {
                alwaysRepaint = target.GetType().GetCustomAttribute<Attributes.AlwaysRepaint>() != null;
                useDefault = (target.GetType().GetCustomAttribute<Attributes.UseDefaultInspector>() != null);
                if (useDefault)
                    return;

                foreach (var attribute in target.GetType().GetCustomAttributes())
                {
                    if (Drawers.TryGetAttributeDrawer(attribute.GetType(), out var drawer))
                    {
                        drawer.SetUp(attribute, null, serializedObject);
                        DrawnElements.Add(drawer);
                    }
                }

                if (Drawers.TryGetClassDrawer(target.GetType(), out var Drawer))
                {
                    Drawer.SetUp(serializedObject);
                    DrawnElements.Add(Drawer);
                }
            }

            public override bool RequiresConstantRepaint() => alwaysRepaint ? true : base.RequiresConstantRepaint();

            List<IDrawer> DrawnElements = new List<IDrawer>();

            public override void OnInspectorGUI()
            {
                if (useDefault)
                    base.OnInspectorGUI();
                else
                {
                    serializedObject.Update();
                    foreach (var item in DrawnElements)
                    {
                        item.Draw();
                    }
                    serializedObject.ApplyModifiedProperties();
                }
            }

        }

#endif
        class DrawDefaultField : DrawField<object>
            {
                public override void Draw()
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(fieldInfo.Name));
                }
            }
        public class DrawDefaultClass : DrawClass<UnityEngine.Object>
    {
        public override void Init()
        {
            foreach (var member in serializedObject.targetObject.GetType().GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (!member.GetCustomAttribute<Attributes.Visible>())
                    continue;
                if (member.GetCustomAttribute<HideInInspector>() != null)
                    continue;

                foreach (var attribute in member.GetCustomAttributes())
                {
                    if (Drawers.TryGetAttributeDrawer(attribute.GetType(), out var drawer))
                    {
                        drawer.SetUp(attribute, member, serializedObject);
                        drawers.Add(drawer);
                    }
                }

                if (member is FieldInfo field && serializedObject.FindProperty(field.Name) != null)
                {
                    if (Drawers.TryGetFieldDrawer(field.FieldType, out var drawer))
                    {
                        drawer.SetUp(serializedObject, member);
                        drawers.Add(drawer);
                    }
                }
            }
        }

        List<IDrawer> drawers = new List<IDrawer>();

        public override void Draw()
        {
            foreach (var drawer in drawers)
                drawer.Draw();
        }
    }
    }


}
#endif