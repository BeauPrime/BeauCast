/*
 * Copyright (C) 2017-2019. Filament Games, LLC. All rights reserved.
 * Author:  Alex Beauchesne
 * Date:    5 Jan 2017
 * 
 * File:    MsgDispatch.cs
 * Purpose: Enumerates message dispatch timing values.
*/

using System;

namespace BeauCast.Internal
{
    /// <summary>
    /// Flags determining how a message is dispatched.
    /// </summary>
    [Flags]
    internal enum MsgDispatch : byte
    {
        /// <summary>
        /// Message will queue to execute at the end of the frame.
        /// </summary>
        Queue               = 0x000,

        /// <summary>
        /// Message will be discarded if not processed.
        /// </summary>
        Discard             = 0x001,

        /// <summary>
        /// Message will be forced to be processed.
        /// </summary>
        Force               = 0x002,

        /// <summary>
        /// Message will be dispatched immediately.
        /// </summary>
        Immediate           = 0x004,

        Default             = Queue,
    }
}