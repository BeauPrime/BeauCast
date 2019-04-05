/*
 * Copyright (C) 2017-2019. Filament Games, LLC. All rights reserved.
 * Author:  Alex Beauchesne
 * Date:    17 Jan 2017
 * 
 * File:    MsgEditorGUI.cs
 * Purpose: Defines common methods for drawing editors for MsgTypes,
 *          MsgCalls, and MsgTargets.
*/

#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;
using BeauCast.Internal;
using System.Collections.Generic;

namespace BeauCast.Editor
{
    static internal class MsgEditorGUI
    {
        #region MsgType

        static public void MsgTypeField(Rect position, SerializedProperty property, GUIContent label, FilteredMsgTypeList msgTypes)
        {
            var valueProp = property.FindPropertyRelative("m_Value");
            int currentHash = valueProp.intValue;

            EditorGUI.BeginChangeCheck();
            int currentIndex = msgTypes.FindIndexByHash(currentHash);
            int newIndex = EditorGUI.Popup(position, label, currentIndex, msgTypes.PopupContent, EditorStyles.popup);

            if (EditorGUI.EndChangeCheck() && newIndex != currentIndex)
            {
                valueProp.intValue = msgTypes.FindHashByIndex(newIndex);
            }
        }

        static public float GetHeightMsgTypeField(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        #endregion

        #region MsgTarget

        static public void MsgTargetField(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUIUtility.singleLineHeight;

            SerializedProperty target = property.FindPropertyRelative("Target");
            SerializedProperty obj = property.FindPropertyRelative("Object");

            // Split it in two
            if (target.intValue == (int)MsgTarget.Mode.Other)
            {
                float nonLabelSpace = position.width - EditorGUIUtility.labelWidth;
                float halfWidth = nonLabelSpace * 0.5f;
                position.width -= halfWidth;

                EditorGUI.PropertyField(position, target, label);

                position.x += position.width;
                position.width = halfWidth;

                if (obj.objectReferenceValue == null)
                    GUI.color = MsgEditorGUIUtility.ErrorColor;
                obj.objectReferenceValue = EditorGUI.ObjectField(position, obj.objectReferenceValue, typeof(GameObject), true);
                GUI.color = Color.white;
            }
            else
            {
                EditorGUI.PropertyField(position, target, label);
            }
        }

        static public float GetHeightMsgTargetField(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        #endregion

        #region MsgCall

        private enum ArgState
        {
            Required,
            Optional,
            MissingType
        }

        static public void MsgCallField(Rect position, SerializedProperty property, GUIContent label, FilteredMsgTypeList msgTypes)
        {
            position.height = EditorGUIUtility.singleLineHeight;

            SerializedProperty msgTypeProp = property.FindPropertyRelative("m_MsgType");
            SerializedProperty argTypeProp = property.FindPropertyRelative("m_ArgType");
            SerializedProperty sendArgsProp = property.FindPropertyRelative("m_SendArg");

            ArgState argState = ArgState.MissingType;
            MsgCall.ArgType argType = (MsgCall.ArgType)argTypeProp.intValue;
            MsgType msgType = MsgType.Null;

            // If a message type has been selected, make sure to assign our argument type
            int msgTypeValueInt = msgTypeProp.FindPropertyRelative("m_Value").intValue;
            if (msgTypeValueInt != 0)
            {
                msgType = (MsgType)msgTypeValueInt;
                argType = MsgCallGetArgType(msgType, out argState);
            }
            else
            {
                argType = MsgCall.ArgType.Null;
            }

            // If the argument type changes, clear args
            if (argTypeProp.intValue != (int)argType)
            {
                argTypeProp.intValue = (int)argType;

                if (argState != ArgState.MissingType)
                    MsgCallResetArgs(property);
            }

            MsgTypeField(position, msgTypeProp, label, msgTypes);

            if (argType != MsgCall.ArgType.Null)
            {
                MsgEditorGUIUtility.AdvanceLine(ref position);
                MsgEditorGUIUtility.BeginIndent(ref position);

                Rect argsRect = position;

                if (argState == ArgState.Optional)
                {
                    argsRect.width -= 24;
                    Rect toggleRect = new Rect(position.x + argsRect.width + 4, position.y, 20, position.height);
                    sendArgsProp.boolValue = EditorGUI.Toggle(toggleRect, sendArgsProp.boolValue);
                }
                else
                {
                    sendArgsProp.boolValue = true;
                }

                GUI.enabled = sendArgsProp.boolValue;
                MsgCallArgsField(argsRect, property, msgType, argType);
                MsgEditorGUIUtility.EndIndent(ref position);
                GUI.enabled = true;
            }
        }

        static public void MsgCallArgsField(Rect position, SerializedProperty property, MsgType inMsgType, MsgCall.ArgType inArgType)
        {
            Metadata meta = Manager.Get().Database.Find(inMsgType);
            Type realArgType = meta.ArgType;

            SerializedProperty intProp = property.FindPropertyRelative("m_IntArg");
            SerializedProperty floatProp = property.FindPropertyRelative("m_FloatArg");
            SerializedProperty stringProp = property.FindPropertyRelative("m_StringArg");
            SerializedProperty objProp = property.FindPropertyRelative("m_ObjectArg");

            EditorGUI.BeginChangeCheck();

            switch (inArgType)
            {
                case MsgCall.ArgType.Int:
                    {
                        int newVal = EditorGUI.IntField(position, "Arg", intProp.intValue);
                        if (EditorGUI.EndChangeCheck())
                            intProp.intValue = newVal;
                        break;
                    }
                case MsgCall.ArgType.Float:
                    {
                        float newVal = EditorGUI.FloatField(position, "Arg", floatProp.floatValue);
                        if (EditorGUI.EndChangeCheck())
                            floatProp.floatValue = newVal;
                        break;
                    }
                case MsgCall.ArgType.Bool:
                    {
                        int newVal = EditorGUI.Toggle(position, "Arg", intProp.intValue > 0) ? 1 : 0;
                        if (EditorGUI.EndChangeCheck())
                            intProp.intValue = newVal;
                        break;
                    }
                case MsgCall.ArgType.String:
                    {
                        string newVal = EditorGUI.TextField(position, "Arg", stringProp.stringValue);;
                        if (EditorGUI.EndChangeCheck())
                            stringProp.stringValue = newVal;
                        break;
                    }
                case MsgCall.ArgType.UnityObject:
                    {
                        if (GUI.enabled && objProp.objectReferenceValue == null)
                            GUI.color = MsgEditorGUIUtility.ErrorColor;
                        UnityEngine.Object newVal = EditorGUI.ObjectField(position, "Arg", objProp.objectReferenceValue, realArgType, true);
                        if (EditorGUI.EndChangeCheck())
                            objProp.objectReferenceValue = newVal;
                        GUI.color = Color.white;
                        break;
                    }
                case MsgCall.ArgType.Enum:
                    {
                        Enum e = (Enum)Enum.ToObject(realArgType, intProp.intValue);
                        Enum newValue = EditorGUI.EnumPopup(position, "Arg", e);
                        if (EditorGUI.EndChangeCheck())
                            intProp.intValue = Convert.ToInt32(Convert.ChangeType(newValue, realArgType));
                        break;
                    }
            }
        }

        static public float GetHeightMsgCallField(SerializedProperty property, GUIContent label)
        {
            SerializedProperty argTypeProp = property.FindPropertyRelative("m_ArgType");

            float height = EditorGUIUtility.singleLineHeight;

            if (argTypeProp.intValue == (int)MsgCall.ArgType.Null)
                return height;

            return height * 2 + EditorGUIUtility.standardVerticalSpacing;
        }

        #region Internal

        static private void MsgCallResetArgs(SerializedProperty inProperty)
        {
            SerializedProperty intProp = inProperty.FindPropertyRelative("m_IntArg");
            SerializedProperty floatProp = inProperty.FindPropertyRelative("m_FloatArg");
            SerializedProperty stringProp = inProperty.FindPropertyRelative("m_StringArg");
            SerializedProperty objProp = inProperty.FindPropertyRelative("m_ObjectArg");

            intProp.intValue = 0;
            floatProp.floatValue = 0;
            stringProp.stringValue = string.Empty;
            objProp.objectReferenceValue = null;
        }

        static private MsgCall.ArgType MsgCallGetArgType(MsgType inType, out ArgState outOptional)
        {
            Metadata meta = Manager.Get().Database.Find(inType);
            if (meta == null || meta.ArgType == null)
            {
                outOptional = meta == null ? ArgState.MissingType : ArgState.Optional;
                return MsgCall.ArgType.Null;
            }

            outOptional = meta.HasFlags(MsgFlags.OptionalArgs) ? ArgState.Optional : ArgState.Required;

            if (meta.ArgType == typeof(int))
                return MsgCall.ArgType.Int;
            if (meta.ArgType == typeof(float))
                return MsgCall.ArgType.Float;
            if (meta.ArgType == typeof(bool))
                return MsgCall.ArgType.Bool;
            if (meta.ArgType == typeof(string))
                return MsgCall.ArgType.String;
            if (meta.ArgType.IsEnum)
                return MsgCall.ArgType.Enum;
            if (typeof(UnityEngine.Object).IsAssignableFrom(meta.ArgType))
                return MsgCall.ArgType.UnityObject;

            Debug.LogError("No inspector type available for message of type '" + meta.Name + "'!");
            return MsgCall.ArgType.Null;
        }

        #endregion

        #endregion

        #region Messenger

        static public void MsgComponent(MessengerImpl inImpl, ref HashSet<MsgType> ioSelectedTypes)
        {
            EditorGUILayout.BeginVertical();
            {
                inImpl.Active = EditorGUILayout.Toggle("Active", inImpl.Active);
                GUILayout.Space(10);
                EditorGUILayout.LabelField("ID: " + inImpl.ID.ToString(), EditorStyles.boldLabel);
				if (inImpl.Parent == null)
                	EditorGUILayout.ObjectField("Parent: ", null, typeof(GameObject), true);
				else if (inImpl.Parent.IsRoot)
					EditorGUILayout.ObjectField("Parent: ", Manager.Get().Host.gameObject, typeof(GameObject), true);
				else
					EditorGUILayout.ObjectField("Parent: ", inImpl.Parent.Owner, typeof(GameObject), true);
                EditorGUILayout.LabelField("Listening for", inImpl.Handlers.Count.ToString() + (inImpl.Handlers.Count == 1 ? " message type" : " message types"));

                MsgEditorGUIUtility.BeginIndent();
                {
                    foreach (var entry in inImpl.Handlers)
                    {
                        EditorGUILayout.BeginVertical();
                        {
                            List<Delegate> invokeList = new List<Delegate>();
                            entry.Value.GetInvokeList(invokeList);
                            GUIContent content = new GUIContent("(" + invokeList.Count.ToString() + ") " + entry.Key.ToString(),
                                Manager.Get().Database.Find(entry.Key).Tooltip);
                            bool bOldFoldout = ioSelectedTypes.Contains(entry.Key);
                            bool bFoldout = EditorGUILayout.Foldout(bOldFoldout, content);
                            if (bFoldout)
                            {
                                if (!bOldFoldout)
                                    ioSelectedTypes.Add(entry.Key);

                                MsgEditorGUIUtility.BeginIndent();
                                {
                                    foreach (var method in invokeList)
                                    {
                                        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

                                        string targetName = method.Target == null ? "[Null]" : method.Target.ToString();
                                        string methodName = method.Method.Name;

                                        if (method.Target.GetType() == typeof(MessengerImpl) && methodName == "TryDispatch")
                                        {
                                            GUI.enabled = false;
                                            EditorGUILayout.ObjectField(((MessengerImpl)method.Target).Owner, typeof(GameObject), true);
                                            GUI.enabled = true;

                                            EditorGUILayout.LabelField("(Child Messenger)");
                                        }
                                        else if (method.Target is Component)
                                        {
                                            Component c = (Component)method.Target;

                                            GUI.enabled = false;
                                            EditorGUILayout.ObjectField(c, typeof(Component), true);
                                            GUI.enabled = true;

                                            EditorGUILayout.LabelField(methodName);
                                        }
                                        else
                                        {
                                            EditorGUILayout.LabelField(targetName + ": " + methodName);
                                        }

                                        EditorGUILayout.EndHorizontal();
                                    }
                                }
                                MsgEditorGUIUtility.EndIndent();
                            }
                            else if (bOldFoldout)
                            {
                                ioSelectedTypes.Remove(entry.Key);
                            }
                        }
                        EditorGUILayout.EndVertical();
                    }
                }
                MsgEditorGUIUtility.EndIndent();
            }
            EditorGUILayout.EndVertical();
        }

        #endregion
    }
}

#endif // UNITY_EDITOR