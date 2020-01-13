/*
 * Copyright (C) 2017-2020. Filament Games, LLC. All rights reserved.
 * Author:  Autumn Beauchesne
 * Date:    10 August 2017
 * 
 * File:    MsgTable.cs
 * Purpose: Table for batched register/deregister calls.
*/

using System.Collections.Generic;

namespace BeauCast
{
    /// <summary>
    /// Batched table of register/deregister calls.
    /// </summary>
    public class MsgTable
    {
        private interface IEntry
        {
            void Register(Messenger inMessenger);
            void Deregister(Messenger inMessenger);
        }

        private class HandlerEntry : IEntry
        {
            public MsgType Type;
            public MsgHandler Handler;

            public HandlerEntry(MsgType inType, MsgHandler inHandler)
            {
                Type = inType;
                Handler = inHandler;
            }

            public void Register(Messenger inMessenger)
            {
                inMessenger.Register(Type, Handler);
            }

            public void Deregister(Messenger inMessenger)
            {
                inMessenger.Deregister(Type, Handler);
            }
        }

        private class ActionHandlerEntry : IEntry
        {
            public MsgType Type;
            public MsgActionHandler Handler;

            public ActionHandlerEntry(MsgType inType, MsgActionHandler inHandler)
            {
                Type = inType;
                Handler = inHandler;
            }

            public void Register(Messenger inMessenger)
            {
                inMessenger.Register(Type, Handler);
            }

            public void Deregister(Messenger inMessenger)
            {
                inMessenger.Deregister(Type, Handler);
            }
        }

        private class TypedHandlerEntry<T> : IEntry
        {
            public MsgType<T> Type;
            public MsgTypedHandler<T> Handler;

            public TypedHandlerEntry(MsgType<T> inType, MsgTypedHandler<T> inHandler)
            {
                Type = inType;
                Handler = inHandler;
            }

            public void Register(Messenger inMessenger)
            {
                inMessenger.Register(Type, Handler);
            }

            public void Deregister(Messenger inMessenger)
            {
                inMessenger.Deregister(Type, Handler);
            }
        }

        private List<IEntry> m_Entries;

        public MsgTable()
        {
        }

        public MsgTable(int inCapacity)
        {
            m_Entries = new List<IEntry>(inCapacity);
        }

        /// <summary>
        /// Applies the table to the given messenger.
        /// </summary>
        public void Apply(Messenger inMessenger)
        {
            if (m_Entries != null)
            {
                for (int i = 0; i < m_Entries.Count; ++i)
                    m_Entries[i].Register(inMessenger);
            }
        }

        /// <summary>
        /// Removes the table from the given messenger.
        /// </summary>
        public void Remove(Messenger inMessenger)
        {
            if (m_Entries != null)
            {
                for (int i = 0; i < m_Entries.Count; ++i)
                    m_Entries[i].Deregister(inMessenger);
            }
        }

        /// <summary>
        /// Registers a handler for the given message type.
        /// </summary>
        public MsgTable Batch(MsgType inType, MsgHandler inHandler)
        {
            if (m_Entries == null)
                m_Entries = new List<IEntry>();
            m_Entries.Add(new HandlerEntry(inType, inHandler));
            return this;
        }

        /// <summary>
        /// Registers a handler for the given message type.
        /// </summary>
        public MsgTable Batch(MsgType inType, MsgActionHandler inHandler)
        {
            if (m_Entries == null)
                m_Entries = new List<IEntry>();
            m_Entries.Add(new ActionHandlerEntry(inType, inHandler));
            return this;
        }

        /// <summary>
        /// Registers a handler for the given message type.
        /// </summary>
        public MsgTable Batch<T>(MsgType<T> inType, MsgTypedHandler<T> inHandler)
        {
            if (m_Entries == null)
                m_Entries = new List<IEntry>();
            m_Entries.Add(new TypedHandlerEntry<T>(inType, inHandler));
            return this;
        }

        /// <summary>
        /// Clears the table.
        /// </summary>
        public void Clear()
        {
            if (m_Entries != null)
                m_Entries.Clear();
            m_Entries = null;
        }
    }
}
