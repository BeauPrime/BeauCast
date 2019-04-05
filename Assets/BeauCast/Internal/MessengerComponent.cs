/*
 * Copyright (C) 2017-2019. Filament Games, LLC. All rights reserved.
 * Author:  Alex Beauchesne
 * Date:    1 May 2017
 * 
 * File:    MessengerComponent.cs
 * Purpose: Controls lifetime of a Messenger object.
*/

using UnityEngine;

namespace BeauCast.Internal
{
    [AddComponentMenu("")]
    internal sealed class MessengerComponent : MonoBehaviour
    {
        public MessengerImpl Messenger { get; private set; }

        public void Initialize(MessengerImpl inMessenger)
        {
            Messenger = inMessenger;
        }

        private void OnEnable()
        {
            if (OwnsMessenger())
            {
                Messenger.SetUnityActive(true);
            }
        }

        private void OnDisable()
        {
            if (OwnsMessenger())
            {
                Messenger.SetUnityActive(false);
            }
        }

        private void OnDestroy()
        {
            if (OwnsMessenger())
            {
                Messenger.Destroy();
                Messenger = null;
            }
        }

        private bool OwnsMessenger()
        {
            return Messenger != null && Messenger.ID == gameObject.GetInstanceID();
        }
    }
}
