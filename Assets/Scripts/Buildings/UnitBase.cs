using Combat;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Buildings
{
    [RequireComponent(typeof(Damageable))]
    [RequireComponent(typeof(UnitSpawnerBuilding))]
    public class UnitBase : NetworkBehaviour, IDestructible
    {
        private Damageable damageable;

        public static Action<UnitBase> OnServerBaseSpawned;
        public static Action<UnitBase> OnServerBaseDrop;

        private void Awake()
        {
            damageable = GetComponent<Damageable>();
        }
        #region Server
        public override void OnStartServer()
        {
            damageable.OnServerDestruct += HandleDestruction;
            OnServerBaseSpawned?.Invoke(this);
        }

        public override void OnStopServer()
        {
            damageable.OnServerDestruct -= HandleDestruction;
            OnServerBaseDrop?.Invoke(this);
        }

        [Server]
        public void HandleDestruction()
        {
            NetworkServer.Destroy(gameObject);
        }
        #endregion

        #region Client
        #endregion
    }
}