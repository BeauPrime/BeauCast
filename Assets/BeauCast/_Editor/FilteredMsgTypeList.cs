/*
 * Copyright (C) 2017-2020. Filament Games, LLC. All rights reserved.
 * Author:  Autumn Beauchesne
 * Date:    17 Jan 2017
 * 
 * File:    FilteredMsgTypeList.cs
 * Purpose: Generates lists of message types, filtered by flags
 *          and IUsesMsgType queries.
*/

#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEngine;
using BeauCast.Internal;

namespace BeauCast.Editor
{
    /// <summary>
    /// Filtered list of message types.
    /// </summary>
    internal sealed class FilteredMsgTypeList
    {
        public readonly GUIContent[] PopupContent;

        private List<Metadata> m_EditorValues;

        private FilteredMsgTypeList(List<Metadata> inEditorValues)
        {
            m_EditorValues = inEditorValues;

            PopupContent = new GUIContent[inEditorValues.Count + 1];
            PopupContent[0] = new GUIContent("[Null]", "No message type");

            for (int i = 0; i < m_EditorValues.Count; ++i)
                PopupContent[i + 1] = GetGUIContent(m_EditorValues[i]);
        }

        public int FindIndexByHash(int inHash)
        {
            if (inHash == 0)
                return 0;

            for (int i = 0; i < m_EditorValues.Count; ++i)
                if ((int)(m_EditorValues[i].Type) == inHash)
                    return i + 1;

            return -1;
        }

        public int FindHashByIndex(int inIndex)
        {
            if (inIndex == 0)
                return 0;
            return (int)(m_EditorValues[inIndex - 1].Type);
        }

        static private GUIContent GetGUIContent(Metadata inMetadata)
        {
            GUIContent content = new GUIContent();

            content.text = inMetadata.DisplayName;
            content.tooltip = inMetadata.Tooltip;

            return content;
        }

        #region Static Lists

        static private FilteredMsgTypeList s_ListAll;
        static private FilteredMsgTypeList s_ListNone;
        static private Dictionary<FilterByFlag, FilteredMsgTypeList> s_Lists = new Dictionary<FilterByFlag, FilteredMsgTypeList>();

        static private void Initialize()
        {
            Manager.Get().Database.ProcessForEditor();
        }

        static public FilteredMsgTypeList All()
        {
            Initialize();

            if (s_ListAll == null)
            {
                List<Metadata> types = null;
                Manager.Get().Database.FindAll(ref types);
                s_ListAll = new FilteredMsgTypeList(types);
            }
            return s_ListAll;
        }

        static public FilteredMsgTypeList None()
        {
            Initialize();

            if (s_ListNone == null)
            {
                List<Metadata> types = new List<Metadata>();
                s_ListNone = new FilteredMsgTypeList(types);
            }

            return s_ListNone;
        }

        static public FilteredMsgTypeList Filtered(FilterByFlag inAttribute)
        {
            if (inAttribute == null || (inAttribute.FilterAll == MsgFlags.None && inAttribute.FilterAny == MsgFlags.None))
                return All();

            Initialize();

            FilteredMsgTypeList list;
            if (!s_Lists.TryGetValue(inAttribute, out list))
            {
                List<Metadata> types = null;
                Manager.Get().Database.FindAll(inAttribute.FilterAll, inAttribute.FilterAny, ref types);
                list = new FilteredMsgTypeList(types);
                s_Lists.Add(inAttribute, list);
            }

            return list;
        }

        static public FilteredMsgTypeList Filtered(GameObject inGameObject, FilterByFlag inAttr)
        {
            if (inGameObject == null)
            {
                if (inAttr == null)
                    return All();
                return Filtered(inAttr);
            }
            if (inAttr == null)
                return Filtered(inGameObject, MsgFlags.None, MsgFlags.None);

            return Filtered(inGameObject, inAttr.FilterAll, inAttr.FilterAny);
        }

        static public FilteredMsgTypeList Filtered(GameObject inGameObject, MsgFlags inAll, MsgFlags inAny = MsgFlags.None)
        {
            if (inGameObject == null && (inAll == MsgFlags.None && inAny == MsgFlags.None))
                return All();

            Initialize();

            List<MsgType> types = null;
            MsgUtil.GetAvailableMsgTypes(inGameObject, ref types);
            List<Metadata> metadatas = new List<Metadata>(types.Count);
            for(int i = 0; i < types.Count; ++i)
            {
                Metadata metadata = Manager.Get().Database.Find(types[i]);
                if (metadata == null)
                    continue;

                if ((inAll == MsgFlags.None || metadata.HasFlags(inAll))
                    && (inAny == MsgFlags.None || metadata.HasAnyFlags(inAny))
                    && !metadatas.Contains(metadata))
                    metadatas.Add(metadata);
            }
            metadatas.Sort();

            return new FilteredMsgTypeList(metadatas);
        }

        #endregion
    }
}

#endif // UNITY_EDITOR