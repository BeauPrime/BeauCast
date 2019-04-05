/*
 * Copyright (C) 2017-2019. Filament Games, LLC. All rights reserved.
 * Author:  Alex Beauchesne
 * Date:    1 May 2017
 * 
 * File:    MsgType.cs
 * Purpose: Defines id struct for message types.
*/

using BeauCast.Internal;
using System;
using UnityEngine;

namespace BeauCast
{
    /// <summary>
    /// Handle for a type of message.
    /// </summary>
    [Serializable]
    public partial struct MsgType : IEquatable<MsgType>
    {
        [SerializeField]
        [HideInInspector]
        private int m_Value;

        private MsgType(int inValue)
        {
            m_Value = inValue;
        }

        /// <summary>
        /// Empty/invalid MsgType.
        /// </summary>
        static public readonly MsgType Null = default(MsgType);

        #region Overrides

        public bool Equals(MsgType other)
        {
            return m_Value == other.m_Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is MsgType)
                return Equals((MsgType)obj);
            return false;
        }

        public override int GetHashCode()
        {
            return m_Value;
        }

        public override string ToString()
        {
            return m_Value == 0 ? "Null" : Manager.Get().Database.Find(this).Name;
        }

        static public implicit operator bool(MsgType inHandle)
        {
            return inHandle.m_Value != 0;
        }

        static public bool operator ==(MsgType first, MsgType second)
        {
            return first.m_Value == second.m_Value;
        }

        static public bool operator !=(MsgType first, MsgType second)
        {
            return first.m_Value != second.m_Value;
        }

        static public explicit operator int(MsgType inMsgType)
        {
            return inMsgType.m_Value;
        }

        static public explicit operator MsgType(int inID)
        {
            return new MsgType(inID);
        }

        #endregion

        /// <summary>
        /// Declares a new MsgType with the given ID.
        /// </summary>
        static public MsgTypeGenerator<object> Declare(string inID)
        {
            return Manager.GetDatabase().Declare(inID);
        }

        /// <summary>
        /// Declares a new MsgType with the given ID and argument type.
        /// </summary>
        static public MsgTypeGenerator<T> Declare<T>(string inID)
        {
            return Manager.GetDatabase().Declare<T>(inID);
        }
    }

    /// <summary>
    /// Generic message handle.
    /// Used for compile-time checks for handlers and message arguments.
    /// </summary>
    public struct MsgType<T> : IEquatable<MsgType>
    {
        private int m_Value;

        private MsgType(int inValue)
        {
            m_Value = inValue;
        }

        #region Overrides

        public override int GetHashCode()
        {
            return m_Value;
        }

        public override string ToString()
        {
            return ((MsgType)this).ToString();
        }

        static public implicit operator bool(MsgType<T> inHandle)
        {
            return inHandle.m_Value != 0;
        }

        static public implicit operator MsgType(MsgType<T> inMsgType)
        {
            return (MsgType)inMsgType.m_Value;
        }

        static public explicit operator MsgType<T>(MsgType inMsgType)
        {
            return new MsgType<T>((int)inMsgType);
        }

        public bool Equals(MsgType other)
        {
            return m_Value == (int)other;
        }

        public override bool Equals(object obj)
        {
            if (obj is MsgType || obj is MsgType<T>)
                return Equals((MsgType)obj);
            return false;
        }

        #endregion
    }
}