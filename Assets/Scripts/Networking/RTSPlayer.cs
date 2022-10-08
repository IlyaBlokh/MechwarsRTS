using System;
using System.Collections.Generic;
using Buildings;
using CameraControl;
using Data;
using GameResources;
using Mirror;
using Units;
using UnityEngine;

namespace Networking
{
    [RequireComponent(typeof(PlayerResources))]
    [RequireComponent(typeof(PlayerBuildingPlacer))]
    [RequireComponent(typeof(CameraController))]
    [RequireComponent(typeof(UnitCommandHandler))]
    public class RTSPlayer : NetworkBehaviour
    {
        private PlayerResources playerResources;
        private PlayerBuildingPlacer playerBuildingPlacer;
        private CameraController cameraController;
        private List<Unit> units = new();
        private ColorId displayColor;
        [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))]
        private bool isPartyOwner;
        [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))]
        private string displayName;

        public static event Action<bool> OnPartyOwnerStateUpdated;
        public static event Action OnClientInfoUpdated;

        public List<Unit> Units => units;
        public PlayerResources PlayerResources => playerResources;
        public PlayerBuildingPlacer PlayerBuildingPlacer => playerBuildingPlacer;
        public ColorId DisplayColor => displayColor;
        public CameraController CameraController => cameraController;
        public bool IsPartyOwner => isPartyOwner;
        public string DisplayName => displayName;

        private void Awake()
        {
            playerResources = GetComponent<PlayerResources>();
            playerBuildingPlacer = GetComponent<PlayerBuildingPlacer>();
            cameraController = GetComponent<CameraController>();
            GetComponent<UnitCommandHandler>();
        }

        #region Server
        public override void OnStartServer()
        {
            base.OnStartServer();
            DontDestroyOnLoad(gameObject);
            Unit.OnServerUnitSpawned += ServerHandleUnitSpawn;
            Unit.OnServerUnitDrop += ServerHandleUnitDrop;
            Building.OnServerBuildingSpawned += ServerHandleBuildingSpawn;
            Building.OnServerBuildingDrop += ServerHandleBuildingDrop;
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            Unit.OnServerUnitSpawned -= ServerHandleUnitSpawn;
            Unit.OnServerUnitDrop -= ServerHandleUnitDrop;
            Building.OnServerBuildingSpawned -= ServerHandleBuildingSpawn;
            Building.OnServerBuildingDrop -= ServerHandleBuildingDrop;
        }

        private void ServerHandleUnitSpawn(Unit unit)
        {
            if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;
            units.Add(unit);
        }

        private void ServerHandleUnitDrop(Unit unit)
        {
            if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;
            units.Remove(unit);
        }

        private void ServerHandleBuildingSpawn(Building building)
        {
            if (building.connectionToClient.connectionId != connectionToClient.connectionId) return;
            playerBuildingPlacer.Buildings.Add(building);
        }

        private void ServerHandleBuildingDrop(Building building)
        {
            if (building.connectionToClient.connectionId != connectionToClient.connectionId) return;
            playerBuildingPlacer.Buildings.Remove(building);
        }

        [Server]
        public void SetDisplayName(string newName) => 
            displayName = newName;

        [Server]
        public void SetDisplayColor(ColorId newColor) => 
            displayColor = newColor;

        [Server]
        public void SetIsPartyOwner(bool newValue) => 
            isPartyOwner = newValue;

        [Command]
        public void CmdStartGame()
        {
            if (!IsPartyOwner) return;
            ((RTSNetworkManager)NetworkManager.singleton).StartGame();
        }
        #endregion

        #region Client

        public override void OnStartAuthority()
        {
            if (NetworkServer.active) return;
            Unit.OnAuthorityUnitSpawned += AuthorityHandleUnitSpawn;
            Unit.OnAuthorityUnitDrop += AuthorityHandleUnitDrop;
            Building.OnAuthorityBuildingSpawned += AuthorityHandleBuildingSpawn;
            Building.OnAuthorityBuildingDrop += AuthorityHandleBuildingDrop;
        }

        private void AuthorityHandlePartyOwnerStateUpdated(bool oldValue, bool newValue)
        {
            if (!hasAuthority) return;
            OnPartyOwnerStateUpdated?.Invoke(newValue);
        }

        private void ClientHandleDisplayNameUpdated(string oldName, string newName) => 
            OnClientInfoUpdated?.Invoke();

        public override void OnStartClient()
        {
            if (isClientOnly)
            {
                DontDestroyOnLoad(gameObject);
                ((RTSNetworkManager)NetworkManager.singleton).Players.Add(this);
            }
        }

        public override void OnStopClient()
        {
            if (isClientOnly)
            {
                ((RTSNetworkManager)NetworkManager.singleton).Players.Remove(this);
                OnClientInfoUpdated?.Invoke();
                if (hasAuthority)
                {
                    Unit.OnAuthorityUnitSpawned -= AuthorityHandleUnitSpawn;
                    Unit.OnAuthorityUnitDrop -= AuthorityHandleUnitDrop;
                    Building.OnAuthorityBuildingSpawned -= AuthorityHandleBuildingSpawn;
                    Building.OnAuthorityBuildingDrop -= AuthorityHandleBuildingDrop;
                }
            }
        }

        private void AuthorityHandleUnitSpawn(Unit unit) => 
            units.Add(unit);

        private void AuthorityHandleUnitDrop(Unit unit) => 
            units.Remove(unit);

        private void AuthorityHandleBuildingSpawn(Building building) => 
            playerBuildingPlacer.Buildings.Add(building);

        private void AuthorityHandleBuildingDrop(Building building) => 
            playerBuildingPlacer.Buildings.Remove(building);

        #endregion
    }
}
