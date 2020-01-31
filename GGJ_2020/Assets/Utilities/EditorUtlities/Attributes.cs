using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class UseDefaultInspector : Attribute { }

    [AttributeUsage(AttributeTargets.Class)]
    public class ShowScriptField : Attribute { }

    [AttributeUsage(AttributeTargets.Method)]
    public class Button : Attribute { }

    [AttributeUsage(AttributeTargets.Property)]
    public class GetSet : Attribute { }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class Label : Attribute { }

    [AttributeUsage(AttributeTargets.Class)]
    public class AlwaysRepaint : Attribute { }

    public class Space : Attribute { }

    public class Line : Attribute { }

    public class HelpBox : Attribute
    {
        public HelpBox(string message, MessageType type = MessageType.None)
        {
            this.message = message;
            this.type = type;
        }

        public string message = "";
        public MessageType type = MessageType.None;

        public enum MessageType
        {
            None,
            Info,
            Warning,
            Error
        }
    }

    public class Header : Attribute
    {
        public Header(string message) => this.message = message;
        public string message;
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public class Visible : Attribute
    {
        public Option option;

        public enum Option
        {
            Always,
            PlayMode,
            EditorMode,
            Never
        }

        public static implicit operator bool(Visible args)
        {
            if (args == null) return true;
            switch (args.option)
            {
                case Option.PlayMode:
                    return Application.isPlaying;
                case Option.EditorMode:
                    return !Application.isPlaying;
                case Option.Never:
                    return false;
            }
            return true;
        }
    }
}