/*
 * Copyright (C) 2017-2020. Filament Games, LLC. All rights reserved.
 * Author:  Autumn Beauchesne
 * Date:    10 Jan 2017
 * 
 * File:    UnityMessageHandler.cs
 * Purpose: Registers UnityEvents as handlers for message types.
*/

using System;
using UnityEngine;
using UnityEngine.Events;

namespace BeauCast
{
    /// <summary>
    /// Registers UnityEvents as message handlers.
    /// </summary>
    [AddComponentMenu("BeauCast/Unity Message Handler")]
    public class UnityMessageHandler : MonoBehaviour
    {
        [Serializable]
        private class MsgEvent : UnityEvent<Message> { }

        [Serializable]
        private class Binding
        {
            [LimitToSelf]
            public MsgType Type = MsgType.Null;
            public MsgEvent Response = null;
        }

        [SerializeField]
        private Binding[] m_Listeners = new Binding[0];

        private void Awake()
        {
            Messenger node = Messenger.Require(this);

            for (int i = 0; i < m_Listeners.Length; ++i)
            {
                if (m_Listeners[i].Type && m_Listeners[i].Response.GetPersistentEventCount() > 0)
                {
                    node.Register(m_Listeners[i].Type, m_Listeners[i].Response.Invoke);
                }
            }
        }
    }
}
