/*
 * Copyright (C) 2017-2020. Filament Games, LLC. All rights reserved.
 * Author:  Autumn Beauchesne
 * Date:    10 Jan 2017
 * 
 * File:    SetMessengerParent.cs
 * Purpose: Sets the parent of the Messenger for the GameObject.
 *          Use this to receive events sent to other objects.
*/

using UnityEngine;

namespace BeauCast
{
    /// <summary>
    /// Automatically sets the parent of this GameObject's Messenger
    /// on creation.
    /// </summary>
    [AddComponentMenu("BeauCast/Set Messenger Parent")]
    public class SetMessengerParent : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_Parent = null;

        public GameObject Parent
        {
            get { return m_Parent; }
        }

        private void Awake()
        {
            Messenger messenger = Messenger.Require(this);
            if (m_Parent != null)
                messenger.SetParent(Messenger.Require(m_Parent));
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!UnityEditor.EditorApplication.isPlaying)
                return;

            Messenger.Require(this).SetParent(m_Parent == null ? Messenger.Root : Messenger.Require(m_Parent));
        }
#endif
    }
}
