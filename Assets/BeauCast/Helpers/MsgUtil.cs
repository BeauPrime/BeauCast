/*
 * Copyright (C) 2017-2019. Filament Games, LLC. All rights reserved.
 * Author:  Alex Beauchesne
 * Date:    17 Jan 2017
 * 
 * File:    MsgUtil.cs
 * Purpose: Contains helper functions for dealing with messages.
*/

using System.Collections.Generic;
using UnityEngine;

namespace BeauCast
{
    /// <summary>
    /// Helper objects and functions.
    /// </summary>
    static public class MsgUtil
    {
        /// <summary>
        /// Empty message handler.
        /// </summary>
        static public readonly MsgHandler EmptyHandler = (m) => { };

        /// <summary>
        /// Retrieves all MsgTypes available on the IUsesMsgTypes components of the given GameObject.
        /// </summary>
        static public void GetAvailableMsgTypes(GameObject inGameObject, ref List<MsgType> outMsgTypes)
        {
            if (outMsgTypes == null)
                outMsgTypes = new List<MsgType>();

            foreach (var component in inGameObject.GetComponents<MonoBehaviour>())
                GetAvailableMsgTypes(component, ref outMsgTypes);

            var parent = inGameObject.GetComponent<SetMessengerParent>();
            if (parent != null && parent.Parent != null && parent.Parent != inGameObject)
                GetAvailableMsgTypes(parent.Parent, ref outMsgTypes);
        }

        /// <summary>
        /// Retrieves all MsgTypes available on the MonoBehaviour, if it is an IUsesMsgTypes.
        /// </summary>
        static public void GetAvailableMsgTypes(MonoBehaviour inBehaviour, ref List<MsgType> outMsgTypes)
        {
            if (outMsgTypes == null)
                outMsgTypes = new List<MsgType>();

            IUsesMsgTypes uses = inBehaviour as IUsesMsgTypes;
            if (uses != null)
                outMsgTypes.AddRange(uses.GetMsgTypes());
        }
    }
}
