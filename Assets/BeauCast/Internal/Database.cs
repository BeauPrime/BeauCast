/*
 * Copyright (C) 2017-2020. Filament Games, LLC. All rights reserved.
 * Author:  Autumn Beauchesne
 * Date:    1 May 2017
 * 
 * File:    Database.cs
 * Purpose: Maintains list of message types and their metadata.
 *          In editor, it scans for declared types.
*/

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR && !UNITY_WEBGL
using System.Runtime.CompilerServices;
#endif

namespace BeauCast.Internal
{
    internal sealed class Database
    {
        static private readonly IntPtr TYPEHANDLE_MSG_TYPE = typeof(MsgType).TypeHandle.Value;
        static private readonly IntPtr TYPEHANDLE_MSG_TYPE_GENERIC = typeof(MsgType<>).TypeHandle.Value;

        private List<Metadata> m_EditorTypes = new List<Metadata>(64);
        private Dictionary<MsgType, Metadata> m_Metadata = new Dictionary<MsgType, Metadata>(64);

        #region Declaring message types Messages

        public MsgTypeGenerator<object> Declare(string inID)
        {
            MsgType type = (MsgType)(Animator.StringToHash(inID));
            Metadata meta = new Metadata(this, type, inID, null);

            if (m_Metadata.ContainsKey(type))
                Debug.LogError("Duplicate message declarations for " + inID);

            m_Metadata[type] = meta;
            
            return new MsgTypeGenerator<object>(meta);
        }

        public MsgTypeGenerator<T> Declare<T>(string inID)
        {
            MsgType type = (MsgType)(Animator.StringToHash(inID));
            Metadata meta = new Metadata(this, type, inID, typeof(T));

            if (m_Metadata.ContainsKey(type))
                Debug.LogError("Duplicate message declarations for " + inID);

            m_Metadata[type] = meta;

            return new MsgTypeGenerator<T>(meta);
        }

        #endregion

        #region Processing

        public void Process(Metadata inMetadata)
        {
            if (inMetadata.Priority != 0)
                RequiresSorting = true;
        }

        public void ProcessForEditor()
        {
#if UNITY_EDITOR
            if (m_EditorTypes.Count > 0)
                return;

            var msgTypeAssembly = Assembly.GetAssembly(typeof(MsgType));
            ProcessAssembly(msgTypeAssembly);

            foreach(var data in m_Metadata)
            {
                data.Value.Process();
                if (data.Value.HasFlags(MsgFlags.VisibleInEditor))
                    m_EditorTypes.Add(data.Value);
            }

            m_EditorTypes.Sort();
#endif
        }

        #region Processing

#if UNITY_EDITOR

        // Processes the contents of an assembly for message types
        private void ProcessAssembly(Assembly inAssembly)
        {
            var types = inAssembly.GetTypes();
            for (int i = 0; i < types.Length; ++i)
                ProcessType(types[i]);

            m_EditorTypes.Sort();
        }

        // Processes a type for message types
        private void ProcessType(Type inType)
        {
            FieldInfo[] fields = inType.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            for (int i = 0; i < fields.Length; ++i)
            {
                if (ProcessField(inType, fields[i]))
                    break;
            }
        }

        // Processes a field on a type
        private bool ProcessField(Type inType, FieldInfo inField)
        {
            bool bCorrectType = inField.IsInitOnly;
            bCorrectType = bCorrectType & (
                inField.FieldType.TypeHandle.Value == TYPEHANDLE_MSG_TYPE
                || (inField.FieldType.IsGenericType && inField.FieldType.GetGenericTypeDefinition().TypeHandle.Value == TYPEHANDLE_MSG_TYPE_GENERIC)
                );

            if (!bCorrectType)
                return false;

#if !UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS || UNITY_WEBGL
            inField.GetValue(null);
#else
            RuntimeHelpers.RunClassConstructor(inType.TypeHandle);
#endif
            return true;
        }

#endif

        #endregion

        #endregion

        #region Accessors

        /// <summary>
        /// Returns metadata associated with the given MsgType.
        /// </summary>
        public Metadata Find(MsgType inType)
        {
            Metadata meta;
            m_Metadata.TryGetValue(inType, out meta);
            return meta;
        }

        /// <summary>
        /// Retrieves a list of all editor-visible message types.
        /// </summary>
        public void FindAll(ref List<Metadata> ioResults)
        {
            if (ioResults == null)
                ioResults = new List<Metadata>(m_EditorTypes);
            else
                ioResults.AddRange(m_EditorTypes);
        }

        /// <summary>
        /// Retrieves a list of all editor-visible message types that match the given flags.
        /// </summary>
        public void FindAll(MsgFlags inAll, ref List<Metadata> ioResults)
        {
            if (inAll == MsgFlags.None)
            {
                FindAll(ref ioResults);
                return;
            }

            if (ioResults == null)
                ioResults = new List<Metadata>();

            for (int i = 0; i < m_EditorTypes.Count; ++i)
            {
                Metadata type = m_EditorTypes[i];
                if (type.HasFlags(inAll))
                    ioResults.Add(type);
            }
        }

        /// <summary>
        /// Retrieves a list of all editor-visible message types that match the given flags.
        /// </summary>
        public void FindAll(MsgFlags inAll, MsgFlags inAny, ref List<Metadata> ioResults)
        {
            if (inAny == MsgFlags.None)
            {
                FindAll(inAll, ref ioResults);
                return;
            }

            if (ioResults == null)
                ioResults = new List<Metadata>();

            for(int i = 0; i < m_EditorTypes.Count; ++i)
            {
                Metadata type = m_EditorTypes[i];
                if ((inAll == MsgFlags.None || type.HasFlags(inAll))
                    && type.HasAnyFlags(inAny))
                    ioResults.Add(type);
            }
        }

        public bool RequiresSorting { get; private set; }

        #endregion
    }
}
