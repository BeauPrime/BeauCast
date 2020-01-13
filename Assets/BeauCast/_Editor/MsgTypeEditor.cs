/*
 * Copyright (C) 2017-2020. Filament Games, LLC. All rights reserved.
 * Author:  Autumn Beauchesne
 * Date:    17 Jan 2017
 * 
 * File:    MsgTargetEditor.cs
 * Purpose: Custom inspector for MsgType. Displays a dropdown list
 *          of editor-visible message types.
*/

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using BeauCast.Internal;

namespace BeauCast.Editor
{
    [CustomPropertyDrawer(typeof(MsgType), true)]
    [CustomPropertyDrawer(typeof(FilterByFlag), true)]
    internal sealed class MsgTypeEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            FilterByFlag attr = attribute as FilterByFlag;
            FilteredMsgTypeList list = null;

            if (LimitToSelf.IsDefined(fieldInfo, typeof(LimitToSelf), true))
            {
                Component c = property.serializedObject.targetObject as Component;
                if (c != null)
                    list = FilteredMsgTypeList.Filtered(c.gameObject, attr);
            }

            if (list == null)
                list = FilteredMsgTypeList.Filtered(attr);

            MsgEditorGUI.MsgTypeField(position, property, label, list);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return MsgEditorGUI.GetHeightMsgTypeField(property, label);
        }
    }
}

#endif // UNITY_EDITOR