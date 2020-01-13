/*
 * Copyright (C) 2017-2020. Filament Games, LLC. All rights reserved.
 * Author:  Autumn Beauchesne
 * Date:    5 Jan 2017
 * 
 * File:    Manager.cs
 * Purpose: Manages BeauCast state and dispatches queued messages.
*/

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    #define DEVELOPMENT
#endif

using System;
using System.Collections.Generic;
using UnityEngine;

namespace BeauCast.Internal
{
    internal sealed class Manager
    {
        #region Singleton

        static private Manager s_Instance = new Manager();
        static private Database s_Database = new Database();
        static private bool s_AppQuitting = false;

        /// <summary>
        /// Ensures the Manager exists and is initialized.
        /// </summary>
        static public Manager Create()
        {
            if (s_Instance == null)
                s_Instance = new Manager();
            s_Instance.Initialize();
            return s_Instance;
        }

        /// <summary>
        /// Shuts down the Manager and nulls out the reference.
        /// </summary>
        static public void Destroy()
        {
            if (s_Instance != null)
            {
                if (!s_Instance.IsUpdating)
                {
                    s_Instance.Shutdown();
                    s_Instance = null;
                    return;
                }

                s_Instance.QueueShutdown();
            }
        }

        /// <summary>
        /// Returns the Manager singleton.
        /// </summary>
        static public Manager Get()
        {
#if UNITY_EDITOR
            if (s_Instance == null && !UnityEditor.EditorApplication.isPlaying)
                s_Instance = new Manager();
#endif
            if (s_Instance == null && !s_AppQuitting)
                throw new InvalidOperationException("BeauCast has been shutdown. Please call Initialize() before anything else.");
            return s_Instance;
        }

        /// <summary>
        /// Returns if the Manager singleton exists.
        /// </summary>
        static public bool Exists()
        {
            return s_Instance != null;
        }

        /// <summary>
        /// Returns the message type database.
        /// </summary>
        static public Database GetDatabase()
        {
            return s_Database;
        }

        #endregion

        // Current state
        private bool m_Initialized = false;
        private bool m_Updating = false;
        private bool m_Destroying = false;

        // Registry
        private Dictionary<int, MessengerImpl> m_MessengerRegistry;

        // Message Queue
        private List<MessageWrapper> m_MessageQueue;

        /// <summary>
        /// If updates should be run or not.
        /// </summary>
        public bool Paused = false;

        /// <summary>
        /// Additional checking and profiling.
        /// </summary>
        public bool DebugMode = false;

        /// <summary>
        /// Registry of existing message types.
        /// </summary>
        public Database Database
        {
            get { return s_Database; }
        }

        /// <summary>
        /// Unity host object.
        /// </summary>
        public MessengerUnityHost Host;

        /// <summary>
        /// Root messenger.
        /// </summary>
        public MessengerImpl Root;

        /// <summary>
        /// Is the manager in the middle of processing the queue right now?
        /// </summary>
        public bool IsUpdating
        {
            get { return m_Updating; }
        }

        #region Lifecycle

        public Manager()
        {
            s_Instance = this;

            m_MessengerRegistry = new Dictionary<int, MessengerImpl>();
            m_MessageQueue = new List<MessageWrapper>();

            Root = new MessengerImpl(this);

#if DEVELOPMENT
            DebugMode = true;
#endif
        }

        public void Initialize()
        {
            if (m_Initialized)
                return;

#if DEVELOPMENT
            DebugMode = true;
#else
            DebugMode = UnityEngine.Debug.isDebugBuild;
#endif

            GameObject hostGO = new GameObject("Messenger::Manager");
            Host = hostGO.AddComponent<MessengerUnityHost>();
            Host.Initialize(this);
            //hostGO.hideFlags = HideFlags.HideAndDontSave;
            GameObject.DontDestroyOnLoad(hostGO);

            Log("Initialize()");

            m_Initialized = true;
        }

        public void Update()
        {
            if (Paused)
                return;

            int numToProcess = m_MessageQueue.Count;
            if (numToProcess == 0)
                return;

            if (m_MessageQueue.Count > 256)
            {
                Debug.LogWarning("More than 256 messages this frame!");
            }

            if (s_Database.RequiresSorting)
                m_MessageQueue.Sort();
            MessageWrapper.ResetIDs();

            m_Updating = true;
            {
                for(int i = 0; i < numToProcess; ++i)
                {
                    MessageWrapper msg = m_MessageQueue[i];
                    msg.ProcessFromQueue();
                }

                if (m_MessageQueue.Count == numToProcess)
                    m_MessageQueue.Clear();
                else
                    m_MessageQueue.RemoveRange(0, numToProcess);
            }
            m_Updating = false;

            if (m_Destroying)
            {
                m_Destroying = false;
                Destroy();
            }
        }

        /// <summary>
        /// Shuts down the manager.
        /// </summary>
        public void Shutdown()
        {
            if (!m_Initialized)
                return;

            m_MessageQueue.Clear();

            foreach (MessengerImpl impl in m_MessengerRegistry.Values)
                impl.Destroy(false);
            m_MessengerRegistry.Clear();

            if (Host != null)
            {
                Host.Shutdown();
                GameObject.Destroy(Host.gameObject);
                Host = null;
            }

            Log("Shutdown()");

            m_Initialized = false;
            s_Instance = null;
        }

        /// <summary>
        /// Queues a shutdown at the end of an update.
        /// </summary>
        public void QueueShutdown()
        {
            m_Destroying = true;
        }

        /// <summary>
        /// Called when the application is quitting.
        /// </summary>
        public void OnApplicationQuit()
        {
            s_AppQuitting = true;
        }

        #endregion

        #region Registry

        /// <summary>
        /// Registers a messenger to the registry.
        /// </summary>
        public void RegisterMessenger(MessengerImpl inImpl)
        {
            m_MessengerRegistry[inImpl.ID] = inImpl;
        }

        /// <summary>
        /// Removes a messenger from the registry.
        /// </summary>
        public void DeregisterMessenger(MessengerImpl inImpl)
        {
            m_MessengerRegistry.Remove(inImpl.ID);
        }

        /// <summary>
        /// Returns the messenger for the given GameObject.
        /// </summary>
        public MessengerImpl Find(GameObject inGameObject)
        {
            MessengerImpl m;
            m_MessengerRegistry.TryGetValue(inGameObject.GetInstanceID(), out m);
            return m;
        }

        /// <summary>
        /// Returns the messenger with the given ID.
        /// </summary>
        public MessengerImpl Find(int inID)
        {
            MessengerImpl m;
            m_MessengerRegistry.TryGetValue(inID, out m);
            return m;
        }

        /// <summary>
        /// Finds or creates a messenger for the given GameObject.
        /// </summary>
        public MessengerImpl Require(GameObject inGameObject)
        {
            if (!m_Initialized)
                Initialize();

            MessengerImpl m = Find(inGameObject);
            if (m == null)
                m = new MessengerImpl(this, inGameObject);
            return m;
        }

        #endregion

        /// <summary>
        /// Queues a message to be executed at the end of the frame.
        /// </summary>
        public void QueueMessage(ref MessageWrapper inWrapper)
        {
            m_MessageQueue.Add(inWrapper);
        }

        /// <summary>
        /// Requeues a message to be executed at the end of the frame.
        /// </summary>
        public void RequeueMessage(ref MessageWrapper inWrapper, MessengerImpl inTarget)
        {
            m_MessageQueue.Add(new MessageWrapper(inWrapper, inTarget));
        }

        /// <summary>
        /// Logs a message to the console in Debug Mode.
        /// </summary>
        public void Log(string inMessage)
        {
            if (DebugMode)
            {
                UnityEngine.Debug.Log("[BeauCast] " + inMessage);
            }
        }
    }
}
