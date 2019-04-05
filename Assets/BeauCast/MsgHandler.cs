/*
 * Copyright (C) 2017-2019. Filament Games, LLC. All rights reserved.
 * Author:  Alex Beauchesne
 * Date:    5 Jan 2017
 * 
 * File:    MsgHandler.cs
 * Purpose: Function signatures for message handlers.
*/

namespace BeauCast
{
    /// <summary>
    /// Function for handling messages.
    /// </summary>
    public delegate void MsgHandler(Message inMessage);

    /// <summary>
    /// Function for handling messages with no arguments.
    /// </summary>
    public delegate void MsgActionHandler();

    /// <summary>
    /// Function for handling messages with one argument.
    /// </summary>
    public delegate void MsgTypedHandler<T>(T inArgument);
}