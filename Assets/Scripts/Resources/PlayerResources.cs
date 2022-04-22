using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Resources
{
    public class PlayerResources : NetworkBehaviour
    {
        [SerializeField] private int defaultCreditsAmount;

        [SyncVar(hook = nameof(ClientHandleCreditsUpdated))] 
        private int credits;

        public event Action<int> OnClientCreditsUpdated;
        public int Credits { get => credits; }

        private void Start()
        {
            credits = defaultCreditsAmount;
        }

        #region Server
        [Server]
        public void AddCredits(int addValue)
        {
            credits += addValue;
        }

        [Server]
        public bool TrySubstractCredits(int subValue)
        {
            if (credits < subValue) return false;
            credits -= subValue;
            return true;
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