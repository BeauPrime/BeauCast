/*
 * Copyright (C) 2017-2020. Filament Games, LLC. All rights reserved.
 * Author:  Autumn Beauchesne
 * Date:    17 Jan 2017
 * 
 * File:    MsgCallEditor.cs
 * Purpose: Custom inspector for MsgCall. Displays a dropdown list
 *          of editor-visible message types, along with a field
 *          for arguments.
*/

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using BeauCast.Internal;

namespace BeauCast.Editor
{
    [CustomPropertyDrawer(typeof(MsgCall), true)]
    internal sealed class MsgCallEditor : PropertyDrawer
    {
        static private FilterByFlag s_CachedAttr = new FilterByFlag(MsgFlags.None, MsgFlags.InspectorArgs);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            FilteredMsgTypeList list = FilteredMsgTypeList.Filtered(s_CachedAttr);
            MsgEditorGUI.MsgCallField(position, property, label, list);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return MsgEditorGUI.GetHeightMsgCallField(property, label);
        }
    }
}

#endif // UNITY_EDITOR