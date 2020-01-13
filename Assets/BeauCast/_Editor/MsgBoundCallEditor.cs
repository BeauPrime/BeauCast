/*
 * Copyright (C) 2017-2020. Filament Games, LLC. All rights reserved.
 * Author:  Autumn Beauchesne
 * Date:    17 Jan 2017
 * 
 * File:    MsgBoundCallEditor.cs
 * Purpose: Custom inspector for MsgBoundCall. Filters message types
 *          based on the selected target.
*/

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using BeauCast.Internal;

namespace BeauCast.Editor
{
    [CustomPropertyDrawer(typeof(MsgBoundCall), true)]
    internal sealed class MsgBoundCallEditor : PropertyDrawer
    {
        static private FilterByFlag s_SingleAttr = new FilterByFlag(MsgFlags.Local | MsgFlags.Input, MsgFlags.InspectorArgs);
        static private FilterByFlag s_AllAttr = new FilterByFlag(MsgFlags.Global | MsgFlags.Input, MsgFlags.InspectorArgs);

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.LabelField(position, label);

            MsgEditorGUIUtility.AdvanceLine(ref position);
            MsgEditorGUIUtility.BeginIndent(ref position);

            SerializedProperty targetProp = property.FindPropertyRelative("Target");
            MsgEditorGUI.MsgTargetField(position, targetProp, new GUIContent("Target"));

            MsgEditorGUIUtility.AdvanceLine(ref position);

            FilteredMsgTypeList list;
            int targetType = targetProp.FindPropertyRelative("Target").intValue;
            if (targetType == (int)MsgTarget.Mode.All)
            {
                list = FilteredMsgTypeList.Filtered(s_AllAttr);
            }
            else if (targetType == (int)MsgTarget.Mode.Other)
            {
                GameObject obj = targetProp.FindPropertyRelative("Object").objectReferenceValue as GameObject;
                if (obj == null)
                    list = FilteredMsgTypeList.None();
                else
                    list = FilteredMsgTypeList.Filtered(obj, s_SingleAttr);
            }
            else
            {
                Component component = property.serializedObject.targetObject as Component;
                if (component == null)
                    list = FilteredMsgTypeList.Filtered(s_SingleAttr);
                else
                    list = FilteredMsgTypeList.Filtered(component.gameObject, s_SingleAttr);
            }

            SerializedProperty callProp = property.FindPropertyRelative("Call");
            MsgEditorGUI.MsgCallField(position, callProp, new GUIContent("Message"), list);

            MsgEditorGUIUtility.EndIndent(ref position);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;
            
            SerializedProperty targetProp = property.FindPropertyRelative("Target");
            SerializedProperty callProp = property.FindPropertyRelative("Call");

            height += EditorGUIUtility.standardVerticalSpacing * 2;
            height += MsgEditorGUI.GetHeightMsgTargetField(targetProp, label) + MsgEditorGUI.GetHeightMsgCallField(callProp, label);

            return height;
        }
    }
}

#endif // UNITY_EDITOR