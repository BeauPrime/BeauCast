/*
 * Copyright (C) 2017-2019. Filament Games, LLC. All rights reserved.
 * Author:  Alex Beauchesne
 * Date:    5 Jan 2017
 * 
 * File:    Messenger.cs
 * Purpose: Public interface for sending messages between objects.
*/

using BeauCast.Internal;
using System;
using System.Collections;
using UnityEngine;

namespace BeauCast
{
    /// <summary>
    /// Allows passing of messages between objects.
    /// </summary>
	public partial class Messenger : IEquatable<Messenger>
    {
        // Reference to actual implementation
        private readonly MessengerImpl m_Impl;

        // Internal constructor
        internal Messenger(MessengerImpl inImpl)
        {
            m_Impl = inImpl;
        }

        /// <summary>
        /// Returns the GameObject associated with this Messenger.
        /// </summary>
        public GameObject GetOwner()
        {
            return m_Impl.Owner;
        }

        /// <summary>
        /// Returns if this Messenger is the root Messenger.
        /// </summary>
        public bool IsRoot()
        {
            return m_Impl.IsRoot;
        }

        #region Active

        /// <summary>
        /// Returns if the Messenger is active.
        /// </summary>
        public bool GetActive()
        {
            if (m_Impl.IsValid)
                return m_Impl.IsValid;
            return false;
        }

        /// <summary>
        /// Activates/deactivates the Messenger.
        /// </summary>
        public void SetActive(bool inbActive)
        {
            if (m_Impl.IsValid)
                m_Impl.Active = inbActive;
        }

        #endregion

        #region Parent

        /// <summary>
        /// Gets the parent Messenger.
        /// </summary>
        public Messenger GetParent()
        {
            if (m_Impl.IsValid)
            {
                MessengerImpl parent = m_Impl.Parent;
                return m_Impl.Parent == null ? null : parent.Interface;
            }
            return null;
        }

        /// <summary>
        /// Sets the parent Messenger.
        /// </summary>
        public Messenger SetParent(Messenger inParent)
        {
            if (m_Impl.IsValid)
                m_Impl.Parent = inParent.m_Impl;
            return this;
        }

        #endregion

        #region Registration

        /// <summary>
        /// Registers a handler for the given message type.
        /// </summary>
        public Messenger Register(MsgType inType, MsgHandler inHandler)
        {
            if (m_Impl.IsValid)
                m_Impl.Register(inType, inHandler);
            return this;
        }

        /// <summary>
        /// Registers a handler for the given message type.
        /// </summary>
        public Messenger Register(MsgType inType, MsgActionHandler inHandler)
        {
            if (m_Impl.IsValid)
                m_Impl.Register(inType, inHandler);
            return this;
        }

        /// <summary>
        /// Registers a handler for the given message type.
        /// </summary>
        public Messenger Register<T>(MsgType<T> inType, MsgTypedHandler<T> inHandler)
        {
            if (m_Impl.IsValid)
                m_Impl.Register(inType, inHandler);
            return this;
        }

        /// <summary>
        /// Registers a batch of handlers.
        /// </summary>
        public Messenger Register(MsgTable inTable)
        {
            inTable.Apply(this);
            return this;
        }

        /// <summary>
        /// Deregisters a handler for the given message type.
        /// </summary>
        public Messenger Deregister(MsgType inType, MsgHandler inHandler)
        {
            if (m_Impl.IsValid)
                m_Impl.Deregister(inType, inHandler);
            return this;
        }

        /// <summary>
        /// Deregisters a handler for the given message type.
        /// </summary>
        public Messenger Deregister(MsgType inType, MsgActionHandler inHandler)
        {
            if (m_Impl.IsValid)
                m_Impl.Deregister(inType, inHandler);
            return this;
        }

        /// <summary>
        /// Deregisters a handler for the given message type.
        /// </summary>
        public Messenger Deregister<T>(MsgType<T> inType, MsgTypedHandler<T> inHandler)
        {
            if (m_Impl.IsValid)
                m_Impl.Deregister(inType, inHandler);
            return this;
        }

        /// <summary>
        /// Deregisters a batch of handlers.
        /// </summary>
        public Messenger Deregister(MsgTable inTable)
        {
            inTable.Remove(this);
            return this;
        }

        /// <summary>
        /// Returns if any handlers are registered for the given message type.
        /// </summary>
        public bool IsRegistered(MsgType inType)
        {
            if (m_Impl.IsValid)
                return m_Impl.IsRegistered(inType);
            return false;
        }

        /// <summary>
        /// Registers a temporary handler for the given message type.
        /// </summary>
        public Binding RegisterTemp(MsgType inType, MsgHandler inHandler)
        {
            if (m_Impl.IsValid)
                return new Binding(this, inType, inHandler);
            return null;
        }

        /// <summary>
        /// Registers a temporary handler for the given message type.
        /// </summary>
        public Binding RegisterTemp(MsgType inType, MsgActionHandler inHandler)
        {
            if (m_Impl.IsValid)
                return new Binding(this, inType, (m) => inHandler());
            return null;
        }

        /// <summary>
        /// Registers a temporary handler for the given message type.
        /// </summary>
        public Binding RegisterTemp<T>(MsgType<T> inType, MsgTypedHandler<T> inHandler)
        {
            if (m_Impl.IsValid)
                return new Binding(this, inType, (m) => inHandler((T)m.Arg));
            return null;
        }

        /// <summary>
        /// Registers a temporary handler for the given message type.
        /// </summary>
        public Binding RegisterTemp(MsgType inType)
        {
            if (m_Impl.IsValid)
                return new Binding(this, inType, MsgUtil.EmptyHandler);
            return null;
        }

        #endregion

        #region Send

        /// <summary>
        /// Sends a message to this Messenger and its children.
        /// </summary>
        /// <param name="inType">Type of message to send.</param>
        public Messenger Send(MsgType inType)
        {
            return Send(this, inType);
        }

        /// <summary>
        /// Sends a message and argument to this Messenger and its children.
        /// </summary>
        /// <param name="inType">Type of message to send.</param>
        /// <param name="inArg">Argument to send with the message.</param>
        public Messenger Send<T>(MsgType<T> inType, T inArg)
        {
            return Send(this, inType, inArg);
        }

        /// <summary>
        /// Sends a message to the given Messenger and its children.
        /// </summary>
        /// <param name="inTarget">Target messenger to send to.</param>
        /// <param name="inType">Type of message to send.</param>
        public Messenger Send(Messenger inTarget, MsgType inType)
        {
            if (m_Impl.IsValid)
                m_Impl.Send(inTarget.m_Impl, new Message(this, inType, null));
            return this;
        }

        /// <summary>
        /// Sends a message and argument to the given Messenger and its children.
        /// </summary>
        /// <param name="inTarget">Target messenger to send to.</param>
        /// <param name="inType">Type of message to send.</param>
        /// <param name="inArg">Argument to send with the message.</param>
        public Messenger Send<T>(Messenger inTarget, MsgType<T> inType, T inArg)
        {
            if (m_Impl.IsValid)
                m_Impl.Send(inTarget.m_Impl, new Message(this, inType, inArg));
            return this;
        }

        /// <summary>
        /// Sends a message to the Messenger on the given Component's GameObject.
        /// </summary>
        /// <param name="inTarget">Target messenger to send to.</param>
        /// <param name="inType">Type of message to send.</param>
        public Messenger Send(Component inTarget, MsgType inType)
        {
            if (m_Impl.IsValid)
                m_Impl.Send(inTarget.gameObject, new Message(this, inType, null));
            return this;
        }

        /// <summary>
        /// Sends a message and argument to the Messenger on the given Component's GameObject.
        /// </summary>
        /// <param name="inTarget">Target messenger to send to.</param>
        /// <param name="inType">Type of message to send.</param>
        /// <param name="inArg">Argument to send with the message.</param>
        public Messenger Send<T>(Component inTarget, MsgType<T> inType, T inArg)
        {
            if (m_Impl.IsValid)
                m_Impl.Send(inTarget.gameObject, new Message(this, inType, inArg));
            return this;
        }

        /// <summary>
        /// Sends a message to the Messenger on the given GameObject.
        /// </summary>
        /// <param name="inTarget">Target messenger to send to.</param>
        /// <param name="inType">Type of message to send.</param>
        public Messenger Send(GameObject inTarget, MsgType inType)
        {
            if (m_Impl.IsValid)
                m_Impl.Send(inTarget, new Message(this, inType, null));
            return this;
        }

        /// <summary>
        /// Sends a message and argument to the Messenger on the given GameObject.
        /// </summary>
        /// <param name="inTarget">Target messenger to send to.</param>
        /// <param name="inType">Type of message to send.</param>
        /// <param name="inArg">Argument to send with the message.</param>
        public Messenger Send<T>(GameObject inTarget, MsgType<T> inType, T inArg)
        {
            if (m_Impl.IsValid)
                m_Impl.Send(inTarget, new Message(this, inType, inArg));
            return this;
        }

        /// <summary>
        /// Broadcasts a message from the root Messenger.
        /// </summary>
        /// <param name="inType">Type of message to broadcast.</param>
        public Messenger Broadcast(MsgType inType)
        {
            if (m_Impl.IsValid)
                m_Impl.Broadcast(new Message(this, inType, null));
            return this;
        }

        /// <summary>
        /// Broadcasts a message and argument from the root Messenger.
        /// </summary>
        /// <param name="inType">Type of message to broadcast.</param>
        /// <param name="inArg">Argument to send with the message.</param>
        public Messenger Broadcast<T>(MsgType<T> inType, T inArg)
        {
            if (m_Impl.IsValid)
                m_Impl.Broadcast(new Message(this, inType, inArg));
            return this;
        }

        #endregion

        #region Dispatch

        /// <summary>
        /// Dispatches a message to the given target.
        /// </summary>
        public Messenger Dispatch(MsgTarget inTarget, MsgType inType)
        {
            if (m_Impl.IsValid)
            {
                switch (inTarget.Target)
                {
                    case MsgTarget.Mode.Self:
                        return Send(inType);
                    case MsgTarget.Mode.Other:
                        if (inTarget.Object == null)
                            throw new ArgumentNullException("Null target for MsgTarget.Mode.Other!");
                        else
                            return Send(inTarget.Object, inType);
                    case MsgTarget.Mode.All:
                        return Broadcast(inType);
                    default:
                        throw new InvalidOperationException("Unknown value for MsgTarget.Mode: " + inTarget.Target.ToString());
                }
            }
            return this;
        }

        /// <summary>
        /// Dispatches a message to the given target.
        /// </summary>
        public Messenger Dispatch<T>(MsgTarget inTarget, MsgType<T> inType, T inArgs)
        {
            if (m_Impl.IsValid)
            {
                switch (inTarget.Target)
                {
                    case MsgTarget.Mode.Self:
                        return Send(inType, inArgs);
                    case MsgTarget.Mode.Other:
                        if (inTarget.Object == null)
                            throw new ArgumentNullException("Null target for MsgTarget.Mode.Other!");
                        else
                            return Send(inTarget.Object, inType, inArgs);
                    case MsgTarget.Mode.All:
                        return Broadcast(inType, inArgs);
                    default:
                        throw new InvalidOperationException("Unknown value for MsgTarget.Mode: " + inTarget.Target.ToString());
                }
            }
            return this;
        }

        /// <summary>
        /// Dispatches a message to the given target.
        /// </summary>
        public Messenger Dispatch(MsgTarget inTarget, MsgCall inCall)
        {
            if (m_Impl.IsValid)
                return Dispatch(inTarget, (MsgType<object>)inCall.Type, inCall.Args);
            return this;
        }

        /// <summary>
        /// Dispatches a message.
        /// </summary>
        public Messenger Dispatch(MsgBoundCall inBoundCall)
        {
            if (m_Impl.IsValid)
                return Dispatch(inBoundCall.Target, (MsgType<object>)inBoundCall.Call.Type, inBoundCall.Call.Args);
            return this;
        }

        #endregion

        #region Coroutine

        /// <summary>
        /// Waits to receive a message of the given type.
        /// </summary>
        public IEnumerator WaitFor(MsgType inType)
        {
            if (m_Impl.IsValid)
            {
                using (Binding binding = RegisterTemp(inType))
                {
                    while (!binding.WasHit)
                        yield return null;
                }
            }
        }

        #endregion

        #region Overrides

        public bool Equals(Messenger other)
        {
			if (m_Impl == null)
				return other == null;
            return m_Impl == other.m_Impl;
        }

        public override bool Equals(object obj)
        {
            if (obj is Messenger)
                return Equals((Messenger)obj);
            return false;
        }

        public override int GetHashCode()
        {
            return m_Impl == null ? 0 : m_Impl.ID;
        }

        static public implicit operator bool(Messenger messenger)
        {
			if (messenger == null)
				return false;
            return messenger.m_Impl != null && messenger.m_Impl.IsValid;
        }

        #endregion
    }
}
