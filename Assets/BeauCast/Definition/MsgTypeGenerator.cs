/*
 * Copyright (C) 2017-2020. Filament Games, LLC. All rights reserved.
 * Author:  Autumn Beauchesne
 * Date:    1 May 2017
 * 
 * File:    MsgTypeGenerator.cs
 * Purpose: Provides builder interface for defining a MsgType.
*/

using BeauCast.Internal;

namespace BeauCast
{
    /// <summary>
    /// Handle for a type of message.
    /// </summary>
    public class MsgTypeGenerator<T>
    {
        private Metadata m_Metadata;

        internal MsgTypeGenerator(Metadata inMetadata)
        {
            m_Metadata = inMetadata;
        }

        /// <summary>
        /// Messages will queue to dispatch at the end of the frame.
        /// This is the default behavior.
        /// </summary>
        public MsgTypeGenerator<T> Queue()
        {
            m_Metadata.Dispatch |= MsgDispatch.Queue;
            return this;
        }

        /// <summary>
        /// Messages will queue to dispatch at the end of the frame.
        /// If they skip more than the given number of frames, they will be discarded.
        /// </summary>
        public MsgTypeGenerator<T> Queue(int inRequeueLimit)
        {
            m_Metadata.Dispatch |= MsgDispatch.Queue;
            m_Metadata.RequeueLimit = inRequeueLimit;
            return this;
        }

        /// <summary>
        /// Messages will dispatch immediately.
        /// </summary>
        public MsgTypeGenerator<T> Immediate()
        {
            m_Metadata.Dispatch |= MsgDispatch.Immediate;
            return this;
        }

        /// <summary>
        /// Messages will be processed on disabled Messengers.
        /// </summary>
        public MsgTypeGenerator<T> Force()
        {
            m_Metadata.Dispatch |= MsgDispatch.Force;
            return this;
        }

        /// <summary>
        /// Messages will be discarded on disabled Messengers.
        /// </summary>
        public MsgTypeGenerator<T> Discard()
        {
            m_Metadata.Dispatch |= MsgDispatch.Discard;
            return this;
        }

        /// <summary>
        /// Type will be visible in the editor.
        /// </summary>
        /// <param name="inName">Name to display in the inspector dropdown.</param>
        /// <param name="inDescription">Description to display alongside messages.</param>
        /// <param name="inFlags">Optional flags to use for filtering by type.</param>
        public MsgTypeGenerator<T> Editor(string inName, string inDescription = null, EditorFlags inFlags = EditorFlags.None)
        {
            m_Metadata.DisplayName = string.IsNullOrEmpty(inName) ? string.Empty : inName;
            m_Metadata.Description = string.IsNullOrEmpty(inDescription) ? string.Empty : inDescription;
            m_Metadata.Flags |= (MsgFlags)inFlags;
            return this;
        }

        /// <summary>
        /// Argument is optional.
        /// </summary>
        public MsgTypeGenerator<T> OptionalArg()
        {
            m_Metadata.Flags |= MsgFlags.OptionalArgs;
            return this;
        }

        /// <summary>
        /// Will throw an exception if no Messengers are handling the the message type.
        /// </summary>
        public MsgTypeGenerator<T> RequireHandler()
        {
            m_Metadata.Flags |= MsgFlags.RequireHandler;
            return this;
        }

        /// <summary>
        /// Message will be given priority over other messages, if queued.
        /// </summary>
        /// <param name="inPriority">Messages with greater values are executed first.</param>
        public MsgTypeGenerator<T> Priority(int inPriority)
        {
            m_Metadata.Priority = inPriority;
            return this;
        }

        /// <summary>
        /// Debug messages will be logged under certain situations for the message type.
        /// </summary>
        /// <param name="inLogFlags">When to log messages for this message type.</param>
        public MsgTypeGenerator<T> Log(LogFlags inLogFlags)
        {
            m_Metadata.LogFlags = inLogFlags;
            return this;
        }

        static public implicit operator MsgType(MsgTypeGenerator<T> inTypeGenerator)
        {
            inTypeGenerator.m_Metadata.Process();
            return inTypeGenerator.m_Metadata.Type;
        }

        static public implicit operator MsgType<T>(MsgTypeGenerator<T> inTypeGenerator)
        {
            inTypeGenerator.m_Metadata.Process();
            return (MsgType<T>)inTypeGenerator.m_Metadata.Type;
        }
    }
}