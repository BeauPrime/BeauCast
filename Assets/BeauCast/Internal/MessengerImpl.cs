/*
 * Copyright (C) 2017-2019. Filament Games, LLC. All rights reserved.
 * Author:  Alex Beauchesne
 * Date:    1 May 2017
 * 
 * File:    MessengerImpl.cs
 * Purpose: Implementation of a messenger.
*/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace BeauCast.Internal
{
    /// <summary>
    /// Implementation of Messenger communciation.
    /// </summary>
    internal sealed class MessengerImpl
    {
        private Manager m_Manager;
        private Dictionary<MsgType, DelegateBlock> m_Handlers;

        private MessengerImpl m_Parent;

        private readonly int m_ID = 0;
        private bool m_IsRoot = false;
        private bool m_Destroyed = false;

        private GameObject m_GameObject;
        private MessengerComponent m_UnityComponent;
        private bool m_UnityActive = false;

        private List<MessengerImpl> m_Children;

        /// <summary>
        /// Indicaates if messages should be processed.
        /// </summary>
        public bool Active = true;

        /// <summary>
        /// ID of the current messenger.
        /// </summary>
        public int ID
        {
            get { return m_ID; }
        }

        /// <summary>
        /// Returns the interface for this messenger.
        /// </summary>
        public readonly Messenger Interface;

        /// <summary>
        /// Indicates if the messenger has not been destroyed.
        /// </summary>
        public bool IsValid
        {
            get { return !m_Destroyed; }
        }

        /// <summary>
        /// Indicates if the messenger is the root messenger.
        /// </summary>
        public bool IsRoot
        {
            get { return m_IsRoot; }
        }

        /// <summary>
        /// Returns the handler map.
        /// </summary>
        public Dictionary<MsgType, DelegateBlock> Handlers
        {
            get { return m_Handlers; }
        }

        /// <summary>
        /// Returns the GameObject this is attached to.
        /// </summary>
        public GameObject Owner
        {
            get { return m_GameObject; }
        }

        #region Lifecycle

        /// <summary>
        /// Constructs the root messenger.
        /// </summary>
        public MessengerImpl(Manager inManager)
        {
            m_Manager = inManager;
            m_Handlers = new Dictionary<MsgType, DelegateBlock>(64);

            m_Parent = null;

            // We can expect the root to have many children,
            // so we should pre-allocate a larger list to avoid
            // expanding at runtime
            m_Children = new List<MessengerImpl>(256);

            m_ID = 0;
            m_IsRoot = true;
            m_Destroyed = false;

            m_GameObject = null;
            m_UnityComponent = null;
            m_UnityActive = true;

            Interface = new Messenger(this);

            m_Manager.RegisterMessenger(this);
        }

        /// <summary>
        /// Constructs a messenger for the given GameObject.
        /// </summary>
        public MessengerImpl(Manager inManager, GameObject inUnityObject)
        {
            m_Manager = inManager;
            m_Handlers = new Dictionary<MsgType, DelegateBlock>();
            
			m_Parent = inManager.Root;

            // Non-root messengers do not tend to have children,
            // so we can allocate a small list.
            m_Children = new List<MessengerImpl>(2);
            m_Parent.m_Children.Add(this);

            m_ID = inUnityObject.GetInstanceID();
            m_IsRoot = false;
            m_Destroyed = false;

            m_GameObject = inUnityObject;
            m_UnityComponent = inUnityObject.GetComponent<MessengerComponent>();
            if (!m_UnityComponent)
                m_UnityComponent = inUnityObject.AddComponent<MessengerComponent>();
            m_UnityComponent.Initialize(this);

            m_UnityActive = inUnityObject.activeInHierarchy;

            Interface = new Messenger(this);

            m_Manager.RegisterMessenger(this);
        }

        /// <summary>
        /// Destroys the messenger.
        /// </summary>
        public void Destroy(bool inbRemove = true)
        {
            if (m_Destroyed)
                return;

            m_Destroyed = true;

            if (m_Parent != null)
            {
                foreach (MsgType type in m_Handlers.Keys)
                    m_Parent.Deregister(type, this);
                m_Parent.m_Children.Remove(this);
                m_Parent = null;
            }

            foreach(var handler in m_Handlers.Values)
                handler.Dispose();
            m_Handlers.Clear();

            if (inbRemove)
                m_Manager.DeregisterMessenger(this);

            for (int i = m_Children.Count - 1; i >= 0; --i)
                m_Children[i].OnParentDestroyed();
            m_Children.Clear();
        }

        #endregion

        #region Unity Events

        /// <summary>
        /// Updates the active status from the Unity object.
        /// </summary>
        public void SetUnityActive(bool inbActive)
        {
            m_UnityActive = inbActive;
        }

        #endregion

        #region Register

        /// <summary>
        /// Registers a handler for the given MsgType.
        /// </summary>
        public void Register(MsgType inType, MsgHandler inHandler)
        {
            ValidateRegisterArgs(inType, inHandler);

            DelegateBlock handler;
            if (m_Handlers.TryGetValue(inType, out handler))
            {
                handler.Register(inHandler);
            }
            else
            {
                handler = m_Handlers[inType] = new DelegateBlock(this, m_Manager, inType);
                handler.Register(inHandler);
                if (m_Parent != null)
                    m_Parent.Register(inType, this);
            }
        }

        /// <summary>
        /// Registers a handler for the given MsgType.
        /// </summary>
        public void Register(MsgType inType, MessengerImpl inHandler)
        {
            ValidateRegisterArgs(inType, inHandler);

            DelegateBlock handler;
            if (m_Handlers.TryGetValue(inType, out handler)) 
            {
                handler.Register(inHandler);
            }
            else
            {
                handler = m_Handlers[inType] = new DelegateBlock(this, m_Manager, inType);
                handler.Register(inHandler);
                if (m_Parent != null)
                    m_Parent.Register(inType, this);
            }
        }

        /// <summary>
        /// Registers a handler for the given MsgType.
        /// </summary>
        public void Register(MsgType inType, MsgActionHandler inHandler)
        {
            ValidateRegisterArgs(inType, inHandler);

            DelegateBlock handler;
            if (m_Handlers.TryGetValue(inType, out handler))
            {
                handler.Register(inHandler);
            }
            else
            {
                handler = m_Handlers[inType] = new DelegateBlock(this, m_Manager, inType);
                handler.Register(inHandler);
                if (m_Parent != null)
                    m_Parent.Register(inType, this);
            }
        }

        /// <summary>
        /// Registers a handler for the given MsgType.
        /// </summary>
        public void Register<T>(MsgType inType, MsgTypedHandler<T> inHandler)
        {
            ValidateRegisterArgs(inType, inHandler);

            DelegateBlock handler;
            if (m_Handlers.TryGetValue(inType, out handler))
            {
                handler.Register(inHandler);
            }
            else
            {
                handler = m_Handlers[inType] = new DelegateBlock(this, m_Manager, inType);
                handler.Register(inHandler);
                if (m_Parent != null)
                    m_Parent.Register(inType, this);
            }
        }

        /// <summary>
        /// Deregisters a handler for the given MsgType.
        /// </summary>
        public void Deregister(MsgType inType, MsgHandler inHandler)
        {
            ValidateRegisterArgs(inType, inHandler);

            DelegateBlock handler;
            if (m_Handlers.TryGetValue(inType, out handler))
            {
                handler.Deregister(inHandler);
                if (handler.IsEmpty())
                {
                    handler.Dispose();
                    m_Handlers.Remove(inType);
                    if (m_Parent != null)
                        m_Parent.Deregister(inType, this);
                }
            }
        }

        /// <summary>
        /// Deregisters a handler for the given MsgType.
        /// </summary>
        public void Deregister(MsgType inType, MsgActionHandler inHandler)
        {
            ValidateRegisterArgs(inType, inHandler);

            DelegateBlock handler;
            if (m_Handlers.TryGetValue(inType, out handler))
            {
                handler.Deregister(inHandler);
                if (handler.IsEmpty())
                {
                    handler.Dispose();
                    m_Handlers.Remove(inType);
                    if (m_Parent != null)
                        m_Parent.Deregister(inType, this);
                }
            }
        }

        /// <summary>
        /// Deregisters a handler for the given MsgType.
        /// </summary>
        public void Deregister<T>(MsgType inType, MsgTypedHandler<T> inHandler)
        {
            ValidateRegisterArgs(inType, inHandler);

            DelegateBlock handler;
            if (m_Handlers.TryGetValue(inType, out handler))
            {
                handler.Deregister(inHandler);
                if (handler.IsEmpty())
                {
                    handler.Dispose();
                    m_Handlers.Remove(inType);
                    if (m_Parent != null)
                        m_Parent.Deregister(inType, this);
                }
            }
        }

        /// <summary>
        /// Deregisters a handler for the given MsgType.
        /// </summary>
        public void Deregister(MsgType inType, MessengerImpl inHandler)
        {
            ValidateRegisterArgs(inType, inHandler);

            DelegateBlock handler;
            if (m_Handlers.TryGetValue(inType, out handler))
            {
                handler.Deregister(inHandler);
                if (handler.IsEmpty())
                {
                    handler.Dispose();
                    m_Handlers.Remove(inType);
                    if (m_Parent != null)
                        m_Parent.Deregister(inType, this);
                }
            }
        }

        /// <summary>
        /// Returns if any handlers are registered for the given MsgType.
        /// </summary>
        public bool IsRegistered(MsgType inType)
        {
            if (m_Manager.DebugMode)
            {
                if (m_Destroyed)
                    throw new InvalidOperationException("Messenger has already been destroyed");
                if (!inType)
                    throw new ArgumentException("Provided MsgType is null.");
            }

            return m_Handlers.ContainsKey(inType);
        }

        #endregion

        #region Parent

        /// <summary>
        /// Gets/sets the parent messenger.
        /// </summary>
        public MessengerImpl Parent
        {
            get { return m_Parent; }
            set
            {
                if (m_Manager.DebugMode)
                {
                    if (m_IsRoot)
                        throw new InvalidOperationException("Cannot set parent of root!");
                    if (value == this)
                        throw new ArgumentException("Cannot set parent to self!");
                }

                if (value != m_Parent)
                {
                    if (m_Manager.DebugMode)
                        ValidateGraph(value);

                    foreach (MsgType type in m_Handlers.Keys)
                    {
                        if (m_Parent != null)
                            m_Parent.Deregister(type, this);
                        if (value != null)
                            value.Register(type, this);
                    }

                    if (m_Parent != null)
                        m_Parent.m_Children.Remove(this);

                    m_Parent = value;

                    if (m_Parent != null)
                        m_Parent.m_Children.Add(this);
                }
            }
        }

        #endregion

        #region Send

        /// <summary>
        /// Sends a message to the given messenger.
        /// </summary>
        public void Send(MessengerImpl inTarget, Message inMessage)
        {
            if (m_Manager.DebugMode)
            {
                if (m_Destroyed)
                    throw new InvalidOperationException("Messenger has already been destroyed");
                if (!inMessage.Type)
                    throw new ArgumentException("Provided MsgType is null.");
                if (inTarget == null)
                    throw new ArgumentNullException();
            }

            MessageWrapper wrapper = new MessageWrapper(inTarget, inMessage);
            if (m_Manager.DebugMode)
            {
                if (wrapper.Metadata.CanLog(LogFlags.Dispatch))
                    m_Manager.Log(Owner.name + " sending message " + inMessage.ToString() + " to " + inTarget.Owner.name);
            }

            if ((wrapper.Metadata.Dispatch & MsgDispatch.Immediate) != 0)
                inTarget.TryDispatch(ref wrapper);
            else
                m_Manager.QueueMessage(ref wrapper);
        }

        /// <summary>
        /// Sends a message to the messenger on the given GameObject.
        /// </summary>
        public void Send(GameObject inTarget, Message inMessage)
        {
            if (m_Manager.DebugMode)
            {
                if (m_Destroyed)
                    throw new InvalidOperationException("Messenger has already been destroyed");
                if (!inMessage.Type)
                    throw new ArgumentException("Provided MsgType is null.");
                if (inTarget == null)
                    throw new ArgumentNullException();
            }

            MessengerImpl messenger = m_Manager.Find(inTarget);
            if (messenger == null)
            {
                if (m_Manager.DebugMode)
                {
                    if (m_Manager.Database.Find(inMessage.Type).HasFlags(MsgFlags.RequireHandler))
                        throw new Exception("MsgType " + inMessage.Type.ToString() + " requires a handler");
                }
                return;
            }

            MessageWrapper wrapper = new MessageWrapper(messenger, inMessage);
            if (m_Manager.DebugMode)
            {
                if (wrapper.Metadata.CanLog(LogFlags.Dispatch))
                    m_Manager.Log(Owner.name + " sending message " + inMessage.ToString() + " to " + inTarget.name);
            }

            if ((wrapper.Metadata.Dispatch & MsgDispatch.Immediate) != 0)
                messenger.TryDispatch(ref wrapper);
            else
                m_Manager.QueueMessage(ref wrapper);
        }

        /// <summary>
        /// Broadcasts a message from the root messenger.
        /// </summary>
        public void Broadcast(Message inMessage)
        {
            if (m_Parent == null)
            {
                Send(this, inMessage);
                return;
            }

            if (m_Manager.DebugMode)
            {
                if (m_Destroyed)
                    throw new InvalidOperationException("Messenger has already been destroyed");
                if (!inMessage.Type)
                    throw new ArgumentException("Provided MsgType is null.");
            }

            MessengerImpl root = m_Parent;
            while (root.m_Parent != null)
                root = root.m_Parent;

            MessageWrapper wrapper = new MessageWrapper(root, inMessage);
            if (m_Manager.DebugMode)
            {
                if (wrapper.Metadata.CanLog(LogFlags.Dispatch))
                    m_Manager.Log(Owner.name + " broadcasting message " + inMessage.ToString());
            }

            if ((wrapper.Metadata.Dispatch & MsgDispatch.Immediate) != 0)
                root.TryDispatch(ref wrapper);
            else
                m_Manager.QueueMessage(ref wrapper);
        }

        #endregion

        #region Dispatch

        /// <summary>
        /// Attempts to dispatch the given message.
        /// </summary>
        public void TryDispatch(ref MessageWrapper inWrapper)
        {
            if ((inWrapper.Metadata.Dispatch & MsgDispatch.Force) != 0 || (Active && m_UnityActive))
            {
                DelegateBlock handler;
                if (m_Handlers.TryGetValue(inWrapper.Message.Type, out handler))
                {
                    handler.Invoke(ref inWrapper);
                }
                else if (m_Manager.DebugMode)
                {
                    if (inWrapper.Metadata.HasFlags(MsgFlags.RequireHandler))
                        throw new Exception("MsgType " + inWrapper.Message.Type.ToString() + " requires a handler");
                }
            }
            else if ((inWrapper.Metadata.Dispatch & MsgDispatch.Discard) == 0 && inWrapper.CanRequeue())
            {
                m_Manager.RequeueMessage(ref inWrapper, this);
            }
        }

        #endregion

        #region Internal

        private void OnParentDestroyed()
        {
            m_Parent = null;
        }

        private void ValidateRegisterArgs(MsgType inType, object inHandler)
        {
            if (m_Manager.DebugMode)
            {
                if (m_Destroyed)
                    throw new InvalidOperationException("Messenger has already been destroyed");
                if (!inType)
                    throw new ArgumentException("Provided MsgType is null.");
                if (inHandler == null)
                    throw new ArgumentException("Provided handler is null.");
            }
        }

        // Validates that the graph has no loops
        private void ValidateGraph(MessengerImpl inNewParent)
        {
            HashSet<MessengerImpl> visited = new HashSet<MessengerImpl>();
            visited.Add(this);

            MessengerImpl current = inNewParent;
            while (current != null)
            {
                if (visited.Contains(current))
                    throw new Exception("Invalid or looping Messenger graph, starting with " + (ReferenceEquals(m_GameObject, null) ? "[Root]" : m_GameObject.name));
                visited.Add(current);
                current = current.m_Parent;
            }
        }

        #endregion
    }
}
