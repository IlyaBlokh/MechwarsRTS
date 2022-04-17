using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Resources
{
    public class PlayerResources : NetworkBehaviour
    {
        [SyncVar(hook = nameof(ClientHandleCreditsUpdated))] 
        private int credits;

        public static event Action<int> OnClientCreditsUpdated;
        public int Credits { get => credits; }

        #region Server
        [Server]
        public void AddCredits(int addValue)
        {
            credits += addValue;
        }
        #endregion

        #region Client
        private void ClientHandleCreditsUpdated(int oldValue, int newValue)
        {
            OnClientCreditsUpdated?.Invoke(newValue);
        }
        #endregion
    }
}