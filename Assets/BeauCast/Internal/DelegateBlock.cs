/*
 * Copyright (C) 2017-2019. Filament Games, LLC. All rights reserved.
 * Author:  Alex Beauchesne
 * Date:    5 Jan 2017
 * 
 * File:    DelegateBlock.cs
 * Purpose: Delegates for registering and invoking message handlers.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BeauCast.Internal
{
    /// <summary>
    /// Interface for invokable delegates.
    /// </summary>
    internal interface IMsgDelegate : IDisposable
    {
        void Invoke(ref MessageWrapper inMessage);
        int Count { get; }
        bool IsEmpty();
        void GetInvokeList(List<Delegate> outDelegates);
    }

    /// <summary>
    /// Combined delegates for MsgHandler, Action, typed Action, and child Messengers.
    /// </summary>
    internal sealed class DelegateBlock : IMsgDelegate
    {
        private MessengerImpl m_Owner;
        private Manager m_Manager;
        private bool m_DebugMode;

        private Metadata m_Metadata;
        private Type m_AllowedArgType;
        private IntPtr m_AllowedArgTypePtr;

        private MsgHandler m_Handlers;
        private MsgActionHandler m_Actions;
        private IMsgDelegate m_Typed;
        private List<MessengerImpl> m_Children;

        private int m_DispatchChildrenDepthAndVersion;

        public DelegateBlock(MessengerImpl inOwner, Manager inManager, MsgType inType)
        {
            m_Manager = inManager;
            m_Owner = inOwner;
            m_DebugMode = inManager.DebugMode;

            m_Metadata = inManager.Database.Find(inType);
            m_AllowedArgType = m_Metadata.ArgType;
            if (m_AllowedArgType != null)
                m_AllowedArgTypePtr = m_AllowedArgType.TypeHandle.Value;
        }

        public void Dispose()
        {
            m_Owner = null;
            m_Manager = null;
            m_DebugMode = false;

            m_Metadata = null;
            m_AllowedArgType = null;
            m_Handlers = null;
            m_Actions = null;
            if (m_Typed != null)
            {
                m_Typed.Dispose();
                m_Typed = null;
            }
        }

        public void Register(MsgHandler inHandler)
        {
            if (m_DebugMode && m_Metadata.CanLog(LogFlags.Register))
                m_Manager.Log(m_Owner.Owner.name + " registered listener for " + m_Metadata.Name);
            
            m_Handlers += inHandler;
        }

        public void Register(MsgActionHandler inAction)
        {
            if (m_DebugMode && m_Metadata.CanLog(LogFlags.Register))
                m_Manager.Log(m_Owner.Owner.name + " registered listener for " + m_Metadata.Name);
            
            m_Actions += inAction;
        }

        public void Register<T>(MsgTypedHandler<T> inAction)
        {
            if (m_DebugMode)
            {
                if (m_AllowedArgType == null)
                    throw new ArgumentException("Message type '" + m_Metadata.Name + "' does not accept arguments, but handler for type '" + typeof(T).Name + "' provided!");
                else if (typeof(T).TypeHandle.Value != m_AllowedArgTypePtr)
                    throw new ArgumentException("Message type '" + m_Metadata.Name + "' accepts arguments of type '" + m_AllowedArgType.Name + "', but handler for type '" + typeof(T).Name + "' provided!");

                if (m_Metadata.CanLog(LogFlags.Register))
                    m_Manager.Log(m_Owner.Owner.name + " registered listener for " + m_Metadata.Name);
            }

            TypedMsgDelegate<T> msgDelegate;
            if (m_Typed == null)
                m_Typed = msgDelegate = new TypedMsgDelegate<T>();
            else
                msgDelegate = (TypedMsgDelegate<T>)m_Typed;

            msgDelegate.Register(inAction);
        }

        public void Register(MessengerImpl inHandler)
        {
            if (m_Children == null)
                m_Children = new List<MessengerImpl>(8);
            m_Children.Add(inHandler);
        }

        public void Deregister(MsgHandler inHandler)
        {
            if (m_DebugMode && m_Metadata.CanLog(LogFlags.Deregister))
                m_Manager.Log(m_Owner.Owner.name + " deregistered listener for " + m_Metadata.Name);

            m_Handlers -= inHandler;
        }

        public void Deregister(MsgActionHandler inAction)
        {
            if (m_DebugMode && m_Metadata.CanLog(LogFlags.Deregister))
                m_Manager.Log(m_Owner.Owner.name + " deregistered listener for " + m_Metadata.Name);

            m_Actions -= inAction;
        }

        public void Deregister<T>(MsgTypedHandler<T> inAction)
        {
            if (m_DebugMode && m_Metadata.CanLog(LogFlags.Deregister))
                m_Manager.Log(m_Owner.Owner.name + " deregistered listener for " + m_Metadata.Name);

            if (m_Typed != null)
            {
                TypedMsgDelegate<T> msgDelegate = (TypedMsgDelegate<T>)m_Typed;
                msgDelegate.Unregister(inAction);
            }
        }

        public void Deregister(MessengerImpl inHandler)
        {
            if (m_Children != null)
            {
                if (m_DispatchChildrenDepthAndVersion > 0)
                {
                    int index = m_Children.IndexOf(inHandler);
                    if (index >= 0)
                        m_Children[index] = null;

                    // Incrementing this means that we've changed the list
                    ++m_DispatchChildrenDepthAndVersion;
                }
                else
                {
                    m_Children.Remove(inHandler);
                }
            }
        }

        [DebuggerStepThrough]
        public void Invoke(ref MessageWrapper inMessage)
        {
            if (m_Handlers != null)
                m_Handlers(inMessage.Message);
            if (m_Actions != null)
                m_Actions();

            if (m_Typed != null)
                m_Typed.Invoke(ref inMessage);

            if (m_Children != null)
            {
                int prevDispatchChildren = ++m_DispatchChildrenDepthAndVersion;

                int childLength = m_Children.Count;
                for (int i = 0; i < childLength; ++i)
                {
                    MessengerImpl impl = m_Children[i];
                    if (impl != null)
                        impl.TryDispatch(ref inMessage);
                }

                --m_DispatchChildrenDepthAndVersion;

                // Call earliest in the stack will have prevDispatchChildren set to 1.
                // If this is the earliest call and it's not the expected value of 0 by this point,
                // that means the list has changed and we should clean up the null children
                if (prevDispatchChildren == 1 && m_DispatchChildrenDepthAndVersion != 0)
                {
                    for (int i = m_Children.Count - 1; i >= 0; --i)
                        if (m_Children[i] == null)
                            m_Children.RemoveAt(i);
                    
                    m_DispatchChildrenDepthAndVersion = 0;
                }
            }
        }

        public bool IsEmpty()
        {
            return m_Handlers == null && m_Actions == null && (m_Typed == null || m_Typed.IsEmpty()) && (m_Children == null || m_Children.Count == 0);
        }

        public int Count
        {
            get
            {
                int count = 0;

                if (m_Handlers != null)
                    count += m_Handlers.GetInvocationList().Length;
                if (m_Actions != null)
                    count += m_Actions.GetInvocationList().Length;
                if (m_Typed != null)
                    count += m_Typed.Count;
                if (m_Children != null)
                    count += m_Children.Count;

                return count;
            }
        }

        public void GetInvokeList(List<Delegate> outDelegates)
        {
            if (m_Handlers != null)
                outDelegates.AddRange(m_Handlers.GetInvocationList());
            if (m_Actions != null)
                outDelegates.AddRange(m_Actions.GetInvocationList());
            if (m_Typed != null)
                m_Typed.GetInvokeList(outDelegates);
            if (m_Children != null)
            {
                for (int i = 0; i < m_Children.Count; ++i)
                    outDelegates.Add(new MsgWrapperHandler(m_Children[i].TryDispatch));
            }
        }
    }

    /// <summary>
    /// Delegate that casts to type before invoking the handler.
    /// </summary>
    internal sealed class TypedMsgDelegate<T> : IMsgDelegate
    {
        private MsgTypedHandler<T> m_Action;

        public void Dispose()
        {
            m_Action = null;
        }

        public void Register(MsgTypedHandler<T> inAction)
        {
            m_Action += inAction;
        }

        public void Unregister(MsgTypedHandler<T> inAction)
        {
            m_Action -= inAction;
        }

        public void Invoke(ref MessageWrapper inMessage)
        {
            if (m_Action != null)
                m_Action((T)inMessage.Message.Arg);
        }

        public int Count
        {
            get { return m_Action == null ? 0 : m_Action.GetInvocationList().Length; }
        }

        public bool IsEmpty()
        {
            return m_Action == null;
        }

        public void GetInvokeList(List<Delegate> outDelegates)
        {
            if (m_Action != null)
                outDelegates.AddRange(m_Action.GetInvocationList());
        }
    }
}
