/*
 * Copyright (C) 2017-2020. Filament Games, LLC. All rights reserved.
 * Author:  Autumn Beauchesne
 * Date:    10 Jan 2017
 * 
 * File:    MsgFlags.cs
 * Purpose: Enumerates special message flags.
*/

using System;

namespace BeauCast.Internal
{
    /// <summary>
    /// Flags determining how a message is interpreted.
    /// </summary>
    [Flags]
    internal enum MsgFlags : uint
    {
        /// <summary>
        /// No special flags.
        /// </summary>
        None                = 0,

        /// <summary>
        /// Message is a input or command.
        /// </summary>
        Input               = 1 << 0,

        /// <summary>
        /// Message is an output or event.
        /// </summary>
        Output              = 1 << 1,

        /// <summary>
        /// Message tends to be internal.
        /// </summary>
        Local               = 1 << 2,

        /// <summary>
        /// Message tends to be broadcasted.
        /// </summary>
        Global              = 1 << 3,

        /// <summary>
        /// Message requires at least one handler.
        /// </summary>
        RequireHandler     = 1 << 4,

        /// <summary>
        /// Message does not require a non-null argument.
        /// </summary>
        OptionalArgs        = 1 << 5,

        /// <summary>
        /// Message has no arguments.
        /// </summary>
        NoArgs              = 1 << 6,

        /// <summary>
        /// Message requires a non-null argument.
        /// </summary>
        RequiresArgs        = 1 << 7,

        /// <summary>
        /// Arguments are serializable in editor.
        /// </summary>
        ArgsAreSerializable = 1 << 8,

        /// <summary>
        /// Message type is visible in editor.
        /// </summary>
        VisibleInEditor     = 1 << 9,

        /// <summary>
        /// Arguments can be shown in the inspector.
        /// </summary>
        InspectorArgs       = NoArgs | ArgsAreSerializable
    }
}