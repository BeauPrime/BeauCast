/*
 * Copyright (C) 2017-2019. Filament Games, LLC. All rights reserved.
 * Author:  Alex Beauchesne
 * Date:    5 Jan 2017
 * 
 * File:    Messenger.Utils.cs
 * Purpose: Utility functions for retrieving Messengers.
*/

using BeauCast.Internal;
using UnityEngine;

namespace BeauCast
{
    public partial class Messenger
    {
        #region Registry

        /// <summary>
        /// Finds the Messenger for the given GameObject.
        /// </summary>
        static public Messenger Find(GameObject inGameObject)
        {
            MessengerImpl impl = Manager.Get().Find(inGameObject);
            return impl == null ? null : impl.Interface;
        }

        /// <summary>
        /// Finds the Messenger for the given Component's GameObject.
        /// </summary>
        static public Messenger Find(Component inComponent)
        {
            MessengerImpl impl = Manager.Get().Find(inComponent.gameObject);
            return impl == null ? null : impl.Interface;
        }

        /// <summary>
        /// Gets or creates a Messenger for the given Component's GameObject.
        /// </summary>
        static public Messenger Require(Component inComponent)
        {
            MessengerImpl impl = Manager.Get().Require(inComponent.gameObject);
			return impl == null ? null : impl.Interface;
        }

        /// <summary>
        /// Gets or creates a Messenger for the given GameObject.
        /// </summary>
        static public Messenger Require(GameObject inGameObject)
        {
            MessengerImpl impl = Manager.Get().Require(inGameObject);
            return impl == null ? null : impl.Interface;
        }

        #endregion

        /// <summary>
        /// Root of the Messenger graph.
        /// </summary>
        static public Messenger Root
        {
            get
            {
                if (!Manager.Exists())
                    return null;
                return Manager.Create().Root.Interface;
            }
        }

        #region Initialization

        /// <summary>
        /// Initializes the Messaging system.
        /// </summary>
        static public void Initialize()
        {
            Manager.Create();
        }

        /// <summary>
        /// Returns if the Messaging system is initialized.
        /// </summary>
        static public bool IsInitialized()
        {
            return Manager.Exists();
        }

        /// <summary>
        /// Shuts down the Messaging system.
        /// </summary>
        static public void Shutdown()
        {
            Manager.Destroy();
        }

        #endregion

        #region Global Settings

        /// <summary>
        /// Global settings.
        /// </summary>
        static public class Settings
        {
            /// <summary>
            /// Pauses/resumes the system.
            /// </summary>
            static public bool Paused
            {
                get { return Manager.Get().Paused; }
                set { Manager.Get().Paused = value; }
            }

            /// <summary>
            /// Indicates if debug mode is enabled.
            /// </summary>
            static public bool IsDebugMode
            {
                get { return Manager.Get().DebugMode; }
            }
        }

        #endregion
    }
}
