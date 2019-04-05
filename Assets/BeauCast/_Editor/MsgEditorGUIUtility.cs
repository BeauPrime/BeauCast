/*
 * Copyright (C) 2017-2019. Filament Games, LLC. All rights reserved.
 * Author:  Alex Beauchesne
 * Date:    17 Jan 2017
 * 
 * File:    MsgEditorGUIUtility.cs
 * Purpose: Defines common methods for rendering IMGUI.
*/

#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace BeauCast.Editor
{
    static internal class MsgEditorGUIUtility
    {
        static public readonly Color ErrorColor = Color.red;

        static public void AdvanceLine(ref Rect ioRect)
        {
            ioRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        static public void BeginIndent(ref Rect ioRect)
        {
            ioRect.x += 16;
            ioRect.width -= 16;
            EditorGUIUtility.labelWidth -= 16;
        }

        static public void EndIndent(ref Rect ioRect)
        {
            ioRect.x -= 16;
            ioRect.width += 16;
            EditorGUIUtility.labelWidth += 16;
        }

        static public void BeginIndent(float inSpace = 16)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(16);
            EditorGUILayout.BeginVertical();
        }

        static public void EndIndent()
        {
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
    }
}

#endif // UNITY_EDITOR