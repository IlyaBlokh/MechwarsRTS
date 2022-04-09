using Buildings;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Networking
{
    public class GameLoopController : NetworkBehaviour
    {
        private List<UnitBase> bases = new List<UnitBase>();

        public static Action OnServerGameOver;
        public static Action<string> OnClientGameOver;
        #region Server

        public override void OnStartServer()
        {
            UnitBase.OnServerBaseSpawned += ServerHandleAddBase;
            UnitBase.OnServerBaseDrop += ServerHandleRemoveBase;
        }

        public override void OnStopServer()
        {
            UnitBase.OnServerBaseSpawned -= ServerHandleAddBase;
            UnitBase.OnServerBaseDrop -= ServerHandleRemoveBase;
        }

        [Server]
        private void ServerHandleAddBase(UnitBase newBase)
        {
            bases.Add(newBase);
        }

        [Server]
        private void ServerHandleRemoveBase(UnitBase baseToRemove)
        {
            bases.Remove(baseToRemove);
            if (bases.Count > 1) return;
            if (bases.Count < 1)
            {
                Debug.LogError("No bases left, can't define a winner");
                return;
            }
            var winnerId = bases[0].connectionToClient.connectionId;
            RPCGameOver($"Player {winnerId}");
            OnServerGameOver?.Invoke();
        }

        #endregion

        #region Client
        [ClientRpc]
        private void RPCGameOver(string winnerName)
        {
            OnClientGameOver?.Invoke(winnerName);
        }
        #endregion
    }
}