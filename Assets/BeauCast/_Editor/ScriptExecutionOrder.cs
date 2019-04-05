/*
 * Copyright (C) 2017-2019. Filament Games, LLC. All rights reserved.
 * Author:  Alex Beauchesne
 * Date:    10 Jan 2017
 * 
 * File:    ScriptExecutionOrder.cs
 * Purpose: Adjusts script execution order for SetMessengerParent
 *          and UnityMsgListener to avoid excess unregister/register
 *          calls involved in switching parents.
*/

#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BeauCast.Editor
{
    static internal class ScriptExecutionOrder
    {
        static private readonly Dictionary<string, int> SCRIPT_PRIORITIES = new Dictionary<string, int>()
        {
            { "SetMessengerParent", -1000 },
            { "UnityMessageHandler", -999 },
            { "MessengerUnityHost", 1000 }
        };

        [InitializeOnLoadMethod]
        static private void SetExecutionOrder()
        {
            MonoScript[] allScripts = MonoImporter.GetAllRuntimeMonoScripts();
            foreach(var script in allScripts)
            {
                string name = script.name;
                int priority;
                if (SCRIPT_PRIORITIES.TryGetValue(name, out priority))
                    AttemptSetExecution(script, priority);
            }
        }

        static private void AttemptSetExecution(MonoScript inScript, int inOrder)
        {
            if (MonoImporter.GetExecutionOrder(inScript) != inOrder)
            {
                Debug.Log("Execution Order for '" + inScript.name + "' changed from " + MonoImporter.GetExecutionOrder(inScript) + " to " + inOrder);
                MonoImporter.SetExecutionOrder(inScript, inOrder);
            }
        }
    }
}

#endif // UNITY_EDITOR