using Mirror;
using System;
using Networking;
using UI;
using Units;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Buildings
{
    public class UnitSpawnerBehaviour : NetworkBehaviour, IPointerClickHandler
    {
        [SerializeField] private UnitQueueUI unitQueueUI;
        [SerializeField] private Unit unitToSpawnPrefab;
        [SerializeField] private Transform spawnPoint;
        [SerializeField, Min(0)] private int maxQueueLength;
        [SerializeField, Min(0)] private float spawnDuration;
        [SerializeField, Min(0)] private float spawnMoveRange;

        [SyncVar]
        private float spawnTimer;
        [SyncVar(hook = nameof(ClientHandleUnitsInQueue))]
        private int currentUnitsInQueue;

        private RTSPlayer player;

        private void Start()
        {
            player = connectionToClient.identity.GetComponent<RTSPlayer>();
            currentUnitsInQueue = 0;
            spawnTimer = .0f;
        }

        private void Update()
        {
            if (isServer)
            {
                UpdateUnitSpawning();
            }

            if (isClient)
            {
                UpdateSpawnTimerUI();
            }
        }

        #region Server
        [Server]
        private void UpdateUnitSpawning()
        {
            if (currentUnitsInQueue == 0) return;

            if (spawnTimer < spawnDuration)
            {
                spawnTimer += Time.deltaTime;
                return;
            }

            ServerSpawnUnit();
            spawnTimer = 0;
            currentUnitsInQueue--;
        }

        [Server]
        private void ServerSpawnUnit()
        {
            var unit = Instantiate(unitToSpawnPrefab, spawnPoint.position, unitToSpawnPrefab.transform.rotation);
            NetworkServer.Spawn(unit.gameObject, connectionToClient);
            var destinationPoint = unit.GetUnitMovement.ServerFindAvailableDestination(spawnPoint.position, spawnMoveRange);
            unit.GetUnitMovement.ServerTryMove(destinationPoint);
        }

        [Command]
        private void CmdTrySpawnUnit()
        {
            if (currentUnitsInQueue == maxQueueLength) return;
            if (player.PlayerResources.TrySubstractCredits(unitToSpawnPrefab.GetCreditsCostValue()))
            {
                currentUnitsInQueue++;
            }
        }
        #endregion

        #region Client
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            if (!hasAuthority) return;
            CmdTrySpawnUnit();
        }

        private void ClientHandleUnitsInQueue(int oldValue, int newValue)
        {
            unitQueueUI.SetUnitsInQueue(newValue);
        }

        private void UpdateSpawnTimerUI()
        {
            unitQueueUI.UpdateProgress(spawnTimer, spawnDuration);
        }
        #endregion
    }
}
