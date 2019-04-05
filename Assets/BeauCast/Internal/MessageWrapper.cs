/*
 * Copyright (C) 2017-2019. Filament Games, LLC. All rights reserved.
 * Author:  Alex Beauchesne
 * Date:    1 May 2017
 * 
 * File:    MessageWrapper.cs
 * Purpose: Wrapper for a message. Contains metadata for how to dispatch
 *          the message.
*/

using System;
using System.Diagnostics;

namespace BeauCast.Internal
{
    /// <summary>
    /// Function for internally handling messages.
    /// </summary>
    internal delegate void MsgWrapperHandler(ref MessageWrapper inMessage);

    /// <summary>
    /// Wraps Message along with metadata so it can be processed appropriately.
    /// </summary>
    [DebuggerDisplay("{DebuggerString}")]
    internal struct MessageWrapper : IComparable<MessageWrapper>
    {
        static private uint s_GlobalID = 0;

        private readonly uint m_ID;

        public readonly MessengerImpl Target;
        public readonly Message Message;
        public readonly Metadata Metadata;

        private int m_RequeuesLeft;

        public MessageWrapper(MessageWrapper inBase)
        {
            Target = inBase.Target;
            Message = inBase.Message;
            Metadata = inBase.Metadata;
            m_RequeuesLeft = inBase.m_RequeuesLeft;

            m_ID = s_GlobalID++;
        }

        public MessageWrapper(MessageWrapper inBase, MessengerImpl inTarget)
        {
            Target = inTarget;
            Message = inBase.Message;
            Metadata = inBase.Metadata;
            m_RequeuesLeft = inBase.m_RequeuesLeft;

            m_ID = s_GlobalID++;
        }

        public MessageWrapper(MessengerImpl inTarget, Message inMessage)
        {
            Target = inTarget;
            Message = inMessage;

            Manager manager = Manager.Get();
            Metadata = manager.Database.Find(inMessage.Type);

            // Argument validation
            if (manager.DebugMode)
                Metadata.Validate(inMessage);
            m_RequeuesLeft = Metadata.RequeueLimit;

            m_ID = s_GlobalID++;
        }

        public void ProcessFromQueue()
        {
            if (Target == null || !Target.IsValid)
                return;

            Target.TryDispatch(ref this);
        }

        public bool CanRequeue()
        {
            if (m_RequeuesLeft > 0 && --m_RequeuesLeft <= 0)
                return false;

            return true;
        }

        public int CompareTo(MessageWrapper other)
        {
            int priorityCompare = -Metadata.Priority.CompareTo(other.Metadata.Priority);
            if (priorityCompare == 0)
                return m_ID.CompareTo(other.m_ID);
            return priorityCompare;
        }

        private string DebuggerString
        {
            get { return Message.Type.ToString() + " -> " + (Target.IsValid ? Target.Owner.name : "[null]"); }
        }

        public override string ToString()
        {
            return DebuggerString;
        }

        static public void ResetIDs()
        {
            s_GlobalID = 0;
        }
    }
}
