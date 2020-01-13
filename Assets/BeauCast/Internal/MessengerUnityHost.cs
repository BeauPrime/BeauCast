/*
 * Copyright (C) 2016-2020. Filament Games, LLC. All rights reserved.
 * Author:  Autumn Beauchesne
 * Date:    4 Apr 2017
 * 
 * File:    MessengerUnityHost.cs
 * Purpose: Host behavior. Contains hooks for executing BeauCast features.
*/

using UnityEngine;

namespace BeauCast.Internal
{
    [AddComponentMenu("")]
    internal sealed class MessengerUnityHost : MonoBehaviour
    {
        private Manager m_Manager;

        public void Initialize(Manager inManager)
        {
            m_Manager = inManager;
        }

        public Manager Manager
        {
            get { return m_Manager; }
        }

        public void Shutdown()
        {
            m_Manager = null;
        }

        private void OnApplicationQuit()
        {
            if (m_Manager != null)
            {
                m_Manager.OnApplicationQuit();
                m_Manager = null;
                Manager.Destroy();
            }
        }

        private void OnDestroy()
        {
            if (m_Manager != null)
            {
                m_Manager = null;
                Manager.Destroy();
            }
        }

        private void LateUpdate()
        {
            if (m_Manager != null)
            {
                m_Manager.Update();
            }
        }
    }
}
