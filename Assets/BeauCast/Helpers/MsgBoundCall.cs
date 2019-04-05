/*
 * Copyright (C) 2017-2019. Filament Games, LLC. All rights reserved.
 * Author:  Alex Beauchesne
 * Date:    17 Jan 2017
 * 
 * File:    MsgBoundCall.cs
 * Purpose: Struct containing information about a target and
 *          a message to send to that target.
*/

using System;

namespace BeauCast
{
    /// <summary>
    /// Contains a target and a call.
    /// </summary>
    [Serializable]
    public struct MsgBoundCall
    {
        /// <summary>
        /// Target of the message.
        /// </summary>
        public MsgTarget Target;

        /// <summary>
        /// Call to make to the target.
        /// </summary>
        public MsgCall Call;

        static public implicit operator bool(MsgBoundCall inCall)
        {
            return inCall.Target && inCall.Call;
        }
    }
}
