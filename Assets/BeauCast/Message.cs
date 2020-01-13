/*
 * Copyright (C) 2017-2020. Filament Games, LLC. All rights reserved.
 * Author:  Autumn Beauchesne
 * Date:    29 Apr 2017
 * 
 * File:    Message.cs
 * Purpose: Small info packet passed to message handlers.
*/

namespace BeauCast
{
    /// <summary>
    /// Info packet passed to message handlers.
    /// </summary>
    public struct Message
    {
        /// <summary>
        /// The source of the message.
        /// </summary>
        public readonly Messenger Source;

        /// <summary>
        /// Type of message dispatched.
        /// </summary>
        public readonly MsgType Type;

        /// <summary>
        /// Message argument.
        /// </summary>
        public readonly object Arg;

        public Message(Messenger inSource, MsgType inType, object inArg)
        {
            Source = inSource;
            Type = inType;
            Arg = inArg;
        }

        public override string ToString()
        {
            return string.Format("[{0} > {1}({2})]", Source.GetOwner() == null ? "Root" : Source.GetOwner().name, Type.ToString(), Arg == null ? "" : Arg.ToString());
        }
    }
}