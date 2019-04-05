/*
 * Copyright (C) 2017-2019. Filament Games, LLC. All rights reserved.
 * Author:  Alex Beauchesne
 * Date:    12 Jan 2017
 * 
 * File:    MsgTarget.cs
 * Purpose: Struct containing information about the target
 *          of a message dispatch.
*/

using System;
using UnityEngine;

namespace BeauCast
{
    /// <summary>
    /// Struct representing the target of a Messenger.Dispatch call.
    /// </summary>
    [Serializable]
    public struct MsgTarget : IEquatable<MsgTarget>
    {
        /// <summary>
        /// How a message should be dispatched.
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// Dispatch on Messenger and children.
            /// </summary>
            Self,
            
            /// <summary>
            /// Dispatch to the entire hierarchy.
            /// </summary>
            All,

            /// <summary>
            /// Dispatch on another Messenger and chidren.
            /// </summary>
            Other
        }

        /// <summary>
        /// How this message should be dispatched.
        /// </summary>
        public Mode Target;

        /// <summary>
        /// Object this should be sent to.
        /// </summary>
        public GameObject Object;

        private MsgTarget(Mode inMode)
        {
            Target = inMode;
            Object = null;
        }

        private MsgTarget(GameObject inTarget)
        {
            Target = Mode.Other;
            Object = inTarget;
        }

        /// <summary>
        /// Returns a MsgTarget that will dispatch
        /// to the Messenger and its children.
        /// </summary>
        static public MsgTarget Self()
        {
            return new MsgTarget(Mode.Self);
        }

        /// <summary>
        /// Returns a MsgTarget that will dispatch
        /// to the entire Messenger hierarcny.
        /// </summary>
        static public MsgTarget All()
        {
            return new MsgTarget(Mode.All);
        }

        /// <summary>
        /// Returns a MsgTarget that will dispatch
        /// to another Messenger and its children.
        /// </summary>
        static public MsgTarget Other(GameObject inGameObject)
        {
            return new MsgTarget(inGameObject);
        }

        /// <summary>
        /// Returns a MsgTarget that will dispatch
        /// to another Messenger and its children.
        /// </summary>
        static public MsgTarget Other(Messenger inMessenger)
        {
            return new MsgTarget(inMessenger.GetOwner());
        }

        #region Overrides

        static public implicit operator bool(MsgTarget inTarget)
        {
            return inTarget.Target != Mode.Other || inTarget.Object != null;
        }

        public bool Equals(MsgTarget inTarget)
        {
            if (Target != inTarget.Target)
                return false;
            if (Target == Mode.Other && Object != inTarget.Object)
                return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj is MsgTarget)
                return Equals((MsgTarget)obj);
            return false;
        }

        static public bool operator ==(MsgTarget inA, MsgTarget inB)
        {
            return inA.Equals(inB);
        }

        static public bool operator !=(MsgTarget inA, MsgTarget inB)
        {
            return !inA.Equals(inB);
        }

        public override int GetHashCode()
        {
            if (Object == null)
                return Target.GetHashCode();
            return Object.GetHashCode() ^ Target.GetHashCode();
        }

        #endregion
    }
}
