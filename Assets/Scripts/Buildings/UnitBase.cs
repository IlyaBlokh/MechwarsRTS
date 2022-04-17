using Mirror;
using System;
using UnityEngine;

namespace Buildings
{
    public class UnitBase : Building, IDestructible
    { 
        public static Action<UnitBase> OnServerBaseSpawned;
        public static Action<UnitBase> OnServerBaseDrop;
        public static Action<int> OnServerPlayerLost;

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
            OnServerPlayerLost?.Invoke(connectionToClient.connectionId);
        }
        #endregion

        #region Client
        #endregion
    }
}