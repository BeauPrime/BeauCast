/*
 * Copyright (C) 2017-2020. Filament Games, LLC. All rights reserved.
 * Author:  Autumn Beauchesne
 * Date:    10 Jan 2017
 * 
 * File:    IUsesMsgTypes.cs
 * Purpose: Interface associating an object with a set of message types.
*/

namespace BeauCast
{
    /// <summary>
    /// Identifies that an object interacts with a set of message types.
    /// </summary>
    public interface IUsesMsgTypes
    {
        MsgType[] GetMsgTypes();
    }
}