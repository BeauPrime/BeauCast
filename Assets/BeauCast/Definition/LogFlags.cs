/*
 * Copyright (C) 2017-2020. Filament Games, LLC. All rights reserved.
 * Author:  Autumn Beauchesne
 * Date:    24 Nov 2017
 * 
 * File:    LogFlags.cs
 * Purpose: Flags to determine how a MsgType gets logged.
*/

using System;

namespace BeauCast
{
    /// <summary>
    /// Flags determing how a MsgType will get logged.
    /// </summary>
    [Flags]
    public enum LogFlags : uint
    {
        /// <summary>
        /// No special flags.
        /// </summary>
        None                = 0x000,

        /// <summary>
        /// Logged when a listener is registered.
        /// </summary>
        Register            = 0x001,

        /// <summary>
        /// Logged when a listener is deregistered.
        /// </summary>
        Deregister          = 0x002,

        /// <summary>
        /// Logged when the message is dispatched.
        /// </summary>
        Dispatch            = 0x004,

        /// <summary>
        /// Logged when a handler is either registered or deregistered.
        /// </summary>
        Handlers            = Register | Deregister,

        /// <summary>
        /// Logged when a handler is registered or deregistered, or when a message is dispatched.
        /// </summary>
        Full                = Handlers | Dispatch
    }
}