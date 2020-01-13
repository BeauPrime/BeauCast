/*
 * Copyright (C) 2017-2020. Filament Games, LLC. All rights reserved.
 * Author:  Autumn Beauchesne
 * Date:    17 Jan 2017
 * 
 * File:    MsgTargetEditor.cs
 * Purpose: Custom inspector for MsgTarget. Displays all on
 *          one line, with a GameObject field if targetting
 *          a specific object with MsgTarget.Mode.Other
*/

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace BeauCast.Editor
{
    [CustomPropertyDrawer(typeof(MsgTarget))]
    internal sealed class MsgTargetEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            MsgEditorGUI.MsgTargetField(position, property, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return MsgEditorGUI.GetHeightMsgTargetField(property, label);
        }
    }
}

#endif // UNITY_EDITOR