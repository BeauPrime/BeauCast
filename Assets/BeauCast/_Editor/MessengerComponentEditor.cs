/*
 * Copyright (C) 2017-2019. Filament Games, LLC. All rights reserved.
 * Author:  Alex Beauchesne
 * Date:    17 Jan 2017
 * 
 * File:    MessengerComponentEditor.cs
 * Purpose: Displays the listeners registered to a given Messenger.
*/

#if UNITY_EDITOR

using System.Collections.Generic;
using BeauCast.Internal;
using UnityEditor;

namespace BeauCast.Editor
{
    [CustomEditor(typeof(MessengerComponent))]
    internal sealed class MessengerComponentEditor : UnityEditor.Editor
    {
        private HashSet<MsgType> m_SelectedTypes = new HashSet<MsgType>();

        public override void OnInspectorGUI()
        {
            MessengerComponent component = ((MessengerComponent)target);
            MessengerImpl m = component.Messenger;

            if (m == null)
            {
                EditorGUILayout.HelpBox("Runtime only", MessageType.Info);
                return;
            }

            MsgEditorGUI.MsgComponent(m, ref m_SelectedTypes);
        }
    }
}

#endif // UNITY_EDITOR