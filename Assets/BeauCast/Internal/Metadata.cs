/*
 * Copyright (C) 2017-2020. Filament Games, LLC. All rights reserved.
 * Author:  Autumn Beauchesne
 * Date:    1 May 2017
 * 
 * File:    Metadata.cs
 * Purpose: Maintains information about messages and how they are dispatched.
*/

using System;
using System.Text;

namespace BeauCast.Internal
{
    /// <summary>
    /// Metadata describing MsgType.
    /// </summary>
    internal sealed class Metadata : IComparable<Metadata>
    {
        static private readonly IntPtr TYPE_PTR_INT = typeof(int).TypeHandle.Value;
        static private readonly IntPtr TYPE_PTR_BOOL = typeof(bool).TypeHandle.Value;
        static private readonly IntPtr TYPE_PTR_STRING = typeof(string).TypeHandle.Value;
        static private readonly IntPtr TYPE_PTR_FLOAT = typeof(float).TypeHandle.Value;
        static private readonly Type TYPE_UNITYENGINE_OBJECT = typeof(UnityEngine.Object);

        public MsgType Type;
        public string Name;
        public Type ArgType = null;

        public MsgDispatch Dispatch = MsgDispatch.Default;
        public MsgFlags Flags = MsgFlags.None;
        public int Priority = 0;
        public string DefaultMethodName = string.Empty;
        public int RequeueLimit = 0;

        public LogFlags LogFlags = LogFlags.None;

        public string DisplayName = string.Empty;
        public string Description = string.Empty;

        public string Tooltip = string.Empty;

        private Database m_Database;

        public Metadata(Database inDatabase, MsgType inType, string inID, Type inArgType)
        {
            m_Database = inDatabase;

            Type = inType;
            Name = inID;
            ArgType = inArgType;

            Priority = 0;
            Dispatch = MsgDispatch.Default;
        }

        public void Process()
        {
            Flags = MakeFlags();

#if UNITY_EDITOR
            StringBuilder tooltipText = new StringBuilder();
            tooltipText.Append(Name).Append("\n(ID ").Append((int)Type).Append(")");
            if (Description.Length > 0)
                tooltipText.Append("\n - ").Append(Description);
            if (ArgType == null)
            {
                tooltipText.Append("\n - No arguments.");
            }
            else
            {
                tooltipText.Append("\n - Arg: ").Append(ArgType.FullName);
                if (HasFlags(MsgFlags.OptionalArgs))
                    tooltipText.Append(" (Optional)");
            }
            if (HasAnyFlags(MsgFlags.Local | MsgFlags.Global | MsgFlags.Input | MsgFlags.Output))
            {
                tooltipText.Append("\n -");

                if (HasFlags(MsgFlags.Local))
                    tooltipText.Append(" Local");
                else if (HasFlags(MsgFlags.Global))
                    tooltipText.Append(" Global");
                if (HasFlags(MsgFlags.Input))
                    tooltipText.Append(" Input");
                else if (HasFlags(MsgFlags.Output))
                    tooltipText.Append(" Output");

                if (tooltipText[tooltipText.Length - 1] == ' ')
                    --tooltipText.Length;
            }

            Tooltip = tooltipText.ToString();
#endif

            m_Database.Process(this);
        }

        public int CompareTo(Metadata other)
        {
            return DisplayName.CompareTo(other.DisplayName);
        }

        public bool HasFlags(MsgFlags inFlags)
        {
            return (Flags & inFlags) == inFlags;
        }

        public bool HasAnyFlags(MsgFlags inFlags)
        {
            return (Flags & inFlags) != 0;
        }

        public bool CanLog(LogFlags inLogFlags)
        {
            return (LogFlags & inLogFlags) != 0;
        }

        public void Validate(Message inMessage)
        {
            // No args provided but we need arguments
            if (inMessage.Arg == null)
            {
                if (ArgType != null && !HasFlags(MsgFlags.OptionalArgs))
                    throw new ArgumentException("Message type '" + Name + "' requires argument of type '" + ArgType.Name + "', but none provided!");
            }

            // We have an argument
            else
            {
                // No arguments accepted.
                if (ArgType == null)
                    throw new ArgumentException("Message type '" + Name + "' does not accept arguments, but argument of type '" + inMessage.Arg.GetType().Name + "' was provided!");

                // The argument is not of the required type
                else if (!ArgType.IsAssignableFrom(inMessage.Arg.GetType()))
                    throw new ArgumentException("Message type '" + Name + "' requires an argument of type '" + ArgType.Name + "', but argument of type '" + inMessage.Arg.GetType().Name + " was provided!");
            }
        }

        private MsgFlags MakeFlags()
        {
            MsgFlags flags = Flags;

            // Don't allow users to determine if the arguments
            // are serializable
            flags &= ~(MsgFlags.ArgsAreSerializable);

            // Auto-populate argument flags.
            if (ArgType == null)
            {
                // Remove optional and requires flags
                flags &= ~(MsgFlags.OptionalArgs | MsgFlags.RequiresArgs);

                // No argument required
                flags |= MsgFlags.NoArgs;
            }
            else
            {
                flags &= ~(MsgFlags.NoArgs);

                // If the argument isn't optional, set the required flag
                if ((flags & MsgFlags.OptionalArgs) != 0)
                    flags |= MsgFlags.RequiresArgs;

                // Check if the argument is serializable in the editor.
                IntPtr typeHandle = ArgType.TypeHandle.Value;
                if (typeHandle == TYPE_PTR_BOOL || typeHandle == TYPE_PTR_FLOAT || typeHandle == TYPE_PTR_INT || typeHandle == TYPE_PTR_STRING
                    || ArgType.IsEnum || TYPE_UNITYENGINE_OBJECT.IsAssignableFrom(ArgType))
                {
                    flags |= MsgFlags.ArgsAreSerializable;
                }
            }

            // Should this be shown in the editor?
            if (string.IsNullOrEmpty(DisplayName))
                flags &= ~(MsgFlags.VisibleInEditor);
            else
                flags |= (MsgFlags.VisibleInEditor);

            return flags;
        }
    }
}
