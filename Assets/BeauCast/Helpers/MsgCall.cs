/*
 * Copyright (C) 2017-2019. Filament Games, LLC. All rights reserved.
 * Author:  Alex Beauchesne
 * Date:    16 Jan 2017
 * 
 * File:    MsgCall.cs
 * Purpose: Struct containing information about a message type
 *          and the argument to dispatch with.
*/

using BeauCast.Internal;
using System;
using UnityEngine;

namespace BeauCast
{
    /// <summary>
    /// Contains a type and argument.
    /// </summary>
    [Serializable]
    public struct MsgCall
    {
        /// <summary>
        /// Represents a valid argument type.
        /// </summary>
        public enum ArgType
        {
            Null,
            Int,
            Float,
            String,
            Bool,
            Enum,
            UnityObject
        }

        /// <summary>
        /// Message type to dispatch.
        /// </summary>
        public MsgType Type
        {
            get { return m_MsgType; }
            set { m_MsgType = value; }
        }

        /// <summary>
        /// Arguments to dispatch with.
        /// </summary>
        public object Args
        {
            get
            {
                if (!m_SendArg || m_ArgType == ArgType.Null)
                    return null;

                switch(m_ArgType)
                {
                    case ArgType.Int:
                        return m_IntArg;
                    case ArgType.Float:
                        return m_FloatArg;
                    case ArgType.Bool:
                        return m_IntArg > 0;
                    case ArgType.String:
                        return m_StringArg;
                    case ArgType.UnityObject:
                        return m_ObjectArg;
                    case ArgType.Enum:
                        {
                            Type type = Manager.Get().Database.Find(Type).ArgType;
                            return Enum.ToObject(type, m_IntArg);
                        }
                    default:
                        return null;
                }
            }
            set
            {
                if (value == null)
                {
                    m_ArgType = ArgType.Null;
                    m_SendArg = false;
                    return;
                }

                m_SendArg = true;
                if (value is int)
                {
                    m_ArgType = ArgType.Int;
                    m_IntArg = (int)value;

                    m_FloatArg = 0;
                    m_ObjectArg = null;
                    m_StringArg = string.Empty;
                }
                else if (value is float)
                {
                    m_ArgType = ArgType.Float;
                    m_FloatArg = (float)value;

                    m_IntArg = 0;
                    m_ObjectArg = null;
                    m_StringArg = string.Empty;
                }
                else if (value is string)
                {
                    m_ArgType = ArgType.String;
                    m_StringArg = (string)value;

                    m_IntArg = 0;
                    m_FloatArg = 0;
                    m_ObjectArg = null;
                }
                else if (value is bool)
                {
                    m_ArgType = ArgType.Bool;
                    m_IntArg = (bool)value ? 1 : 0;

                    m_FloatArg = 0;
                    m_ObjectArg = null;
                    m_StringArg = string.Empty;
                }
                else if (value is UnityEngine.Object)
                {
                    m_ArgType = ArgType.UnityObject;
                    m_ObjectArg = (UnityEngine.Object)value;

                    m_IntArg = 0;
                    m_FloatArg = 0;
                    m_StringArg = string.Empty;
                }
                else if (value.GetType().IsEnum)
                {
                    m_ArgType = ArgType.Enum;
                    m_IntArg = Convert.ToInt32(value);

                    m_FloatArg = 0;
                    m_ObjectArg = null;
                    m_StringArg = string.Empty;
                }
                else
                {
                    Debug.LogError("Unable to set message argument: " + value.ToString() + " (type '" + value.GetType().Name + "')");
                    m_ArgType = ArgType.Null;
                    m_IntArg = 0;
                    m_FloatArg = 0;
                    m_ObjectArg = null;
                    m_StringArg = string.Empty;
                    m_SendArg = false;
                }
            }
        }

        [SerializeField]
        [FilterByFlag(MsgFlags.None, MsgFlags.InspectorArgs)]
        private MsgType m_MsgType;

        [SerializeField]
        [HideInInspector]
        private ArgType m_ArgType;

        [SerializeField]
        [HideInInspector]
        private bool m_SendArg;

        [SerializeField]
        [HideInInspector]
        private int m_IntArg;

        [SerializeField]
        [HideInInspector]
        private float m_FloatArg;

        [SerializeField]
        [HideInInspector]
        private string m_StringArg;

        [SerializeField]
        [HideInInspector]
        private UnityEngine.Object m_ObjectArg;

        static public implicit operator bool(MsgCall inCall)
        {
            return inCall.m_MsgType;
        }
    }
}
