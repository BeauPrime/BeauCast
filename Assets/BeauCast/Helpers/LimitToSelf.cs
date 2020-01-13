/*
 * Copyright (C) 2017-2020. Filament Games, LLC. All rights reserved.
 * Author:  Autumn Beauchesne
 * Date:    5 Jan 2017
 * 
 * File:    LimitToSelf.cs
 * Purpose: Attribute attached to MsgType field. Filters the
 *          list of displayed message types to only include
 *          those associated with the current object.
*/

using System;

namespace BeauCast
{
    /// <summary>
    /// Limits the selectable message types to those associated
    /// with the current GameObject.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class LimitToSelf : Attribute { }
}