/*
 * Copyright (C) 2017-2020. Filament Games, LLC. All rights reserved.
 * Author:  Autumn Beauchesne
 * Date:    17 Jan 2017
 * 
 * File:    MessengerManagerEditor.cs
 * Purpose: Displays the listeners registered to the root Messenger.
*/

#if UNITY_EDITOR

using System.Collections.Generic;
using BeauCast.Internal;
using UnityEditor;

namespace BeauCast.Editor
{
    [CustomEditor(typeof(MessengerUnityHost))]
    internal sealed class MessengerManagerEditor : UnityEditor.Editor
    {
        private HashSet<MsgType> m_SelectedTypes = new HashSet<MsgType>();

        public override void OnInspectorGUI()
        {
            MessengerUnityHost component = ((MessengerUnityHost)target);
            Manager m = component.Manager;

            if (m == null)
            {
                EditorGUILayout.HelpBox("Runtime only", MessageType.Info);
                return;
            }

            MsgEditorGUI.MsgComponent(m.Root, ref m_SelectedTypes);
        }
    }
}

#endif // UNITY_EDITOR