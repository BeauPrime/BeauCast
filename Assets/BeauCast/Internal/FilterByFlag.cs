/*
 * Copyright (C) 2017-2019. Filament Games, LLC. All rights reserved.
 * Author:  Alex Beauchesne
 * Date:    5 Jan 2017
 * 
 * File:    FilterByFlag.cs
 * Purpose: Attribute attached to MsgType field. Filters the
 *          list of displayed message types.
*/

using System;
using UnityEngine;

namespace BeauCast.Internal
{
    /// <summary>
    /// Filters the list of message types.
    /// </summary>
    internal class FilterByFlag : PropertyAttribute, IEquatable<FilterByFlag>
    {
        public MsgFlags FilterAll;
        public MsgFlags FilterAny;

        public FilterByFlag(MsgFlags inFilterAll, MsgFlags inFilterAny = MsgFlags.None)
        {
            FilterAll = inFilterAll;
            FilterAny = inFilterAny;
        }

        public override int GetHashCode()
        {
            return (int)FilterAll;
        }

        public override bool Equals(object obj)
        {
            if (obj is FilterByFlag)
                return Equals((FilterByFlag)obj);
            return false;
        }

        public bool Equals(FilterByFlag other)
        {
            return other != null && other.FilterAll == FilterAll && other.FilterAny == FilterAny;
        }
    }
}