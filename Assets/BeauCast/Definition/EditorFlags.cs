/*
 * Copyright (C) 2017-2019. Filament Games, LLC. All rights reserved.
 * Author:  Alex Beauchesne
 * Date:    1 May 2017
 * 
 * File:    EditorFlags.cs
 * Purpose: Editor-specific flags for describing the intended usage
 *          and behavior of a MsgType.
*/

using System;

namespace BeauCast
{
    /// <summary>
    /// Editor flags to describe the intended usage and behavior of a MsgType.
    /// </summary>
    [Flags]
    public enum EditorFlags : uint
    {
        /// <summary>
        /// No special flags.
        /// </summary>
        None                = 0x000,

        /// <summary>
        /// Message is a command or input.
        /// </summary>
        Command             = 1 << 0,

        /// <summary>
        /// Message is a notification or output.
        /// </summary>
        Notification         = 1 << 1,

        /// <summary>
        /// Message tends to be internal.
        /// </summary>
        Local               = 1 << 2,

        /// <summary>
        /// Message tends to be broadcasted.
        /// </summary>
        Global              = 1 << 3,

        LocalCommand          = Local | Command,
        LocalNotification     = Local | Notification,

        GlobalCommand         = Global | Command,
        GlobalNotification    = Global | Notification,
    }
}