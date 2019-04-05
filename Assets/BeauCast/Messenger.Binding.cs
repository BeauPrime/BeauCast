/*
 * Copyright (C) 2017-2019. Filament Games, LLC. All rights reserved.
 * Author:  Alex Beauchesne
 * Date:    5 Jan 2017
 * 
 * File:    Messenger.Binding.cs
 * Purpose: Temporary callback bound to a Messenger.
*/

using System;

namespace BeauCast
{
    public partial class Messenger
    {
        /// <summary>
        /// Temporarily binds a callback to a type.
        /// Will record the number of times the callback was hit.
        /// </summary>
        public sealed class Binding : IDisposable
        {
            private Messenger m_Messenger;
            private MsgType m_Type;

            private MsgHandler m_Handler;
            private int m_Counter;

            public Binding(Messenger inMessenger, MsgType inType, MsgHandler inHandler)
            {
                if (Settings.IsDebugMode)
                {
                    if (inMessenger == null)
                        throw new ArgumentException("Messenger is null!");
                    if (inHandler == null)
                        throw new ArgumentException("Handler is null!");
                }

                m_Messenger = inMessenger;
                m_Type = inType;
                m_Handler = inHandler;
                m_Counter = 0;

                m_Messenger.m_Impl.Register(m_Type, OnMessage);
            }

            private void OnMessage(Message inMessage)
            {
                ++m_Counter;
                m_Handler(inMessage);
            }

            /// <summary>
            /// Number of times the message was received.
            /// </summary>
            public int Counter
            {
                get { return m_Counter; }
            }

            /// <summary>
            /// If the message was received.
            /// </summary>
            public bool WasHit
            {
                get { return m_Counter > 0; }
            }

            /// <summary>
            /// Resets the counter to zero.
            /// </summary>
            public void Reset()
            {
                m_Counter = 0;
            }

            /// <summary>
            /// Unregisters and disposes the binding.
            /// </summary>
            public void Dispose()
            {
                if (m_Messenger.m_Impl != null && m_Messenger.m_Impl.IsValid)
                    m_Messenger.Deregister(m_Type, OnMessage);

                m_Messenger = null;
                m_Handler = null;
                m_Type = MsgType.Null;
                m_Counter = 0;
            }
        }
    }
}
