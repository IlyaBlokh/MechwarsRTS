using Buildings;
using CameraControl;
using Mirror;
using Networking;
using Resources;
using System;
using System.Collections.Generic;
using Units;
using UnityEngine;

[RequireComponent(typeof(PlayerResources))]
[RequireComponent(typeof(PlayerBuildingPlacer))]
[RequireComponent(typeof(CameraController))]
[RequireComponent(typeof(UnitCommandHandler))]
public class RTSPlayer : NetworkBehaviour
{
    private PlayerResources playerResources;
    private PlayerBuildingPlacer playerBuildingPlacer;
    private CameraController cameraController;
    private List<Unit> units = new List<Unit>();
    private Color teamColor;
    [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))]
    private bool isPartyOwner;

    public static event Action OnPlayerInitialized;
    public static event Action<bool> OnPartyOwnerStateUpdated;

    public List<Unit> Units { get => units;}
    public PlayerResources PlayerResources { get => playerResources; }
    public PlayerBuildingPlacer PlayerBuildingPlacer { get => playerBuildingPlacer;  }
    public Color TeamColor { get => teamColor; }
    public CameraController CameraController { get => cameraController; }
    public bool IsPartyOwner { get => isPartyOwner; }

    private void Awake()
    {
        playerResources = GetComponent<PlayerResources>();
        playerBuildingPlacer = GetComponent<PlayerBuildingPlacer>();
        cameraController = GetComponent<CameraController>();
    }

    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();
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
    public void SetTeamColor(Color newColor)
    {
        teamColor = newColor;
    }

    [Server]
    public void SetIsPartyOwner(bool newValue)
    {
        isPartyOwner = newValue;
    }

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

    public void InitPlayer()
    {
        OnPlayerInitialized?.Invoke();
    }

    private void AuthorityHandlePartyOwnerStateUpdated(bool oldValue, bool newValue)
    {
        if (!hasAuthority) return;
        OnPartyOwnerStateUpdated?.Invoke(newValue);
    }

    public override void OnStartClient()
    {
        if (isClientOnly)
        {
            ((RTSNetworkManager)NetworkManager.singleton).Players.Add(this);
        }
    }

    public override void OnStopClient()
    {
        if (isClientOnly)
        {
            ((RTSNetworkManager)NetworkManager.singleton).Players.Remove(this);
            if (hasAuthority)
            {
                Unit.OnAuthorityUnitSpawned -= AuthorityHandleUnitSpawn;
                Unit.OnAuthorityUnitDrop -= AuthorityHandleUnitDrop;
                Building.OnAuthorityBuildingSpawned -= AuthorityHandleBuildingSpawn;
                Building.OnAuthorityBuildingDrop -= AuthorityHandleBuildingDrop;
            }
        }
    }

    private void AuthorityHandleUnitSpawn(Unit unit)
    {
        units.Add(unit);
    }

    private void AuthorityHandleUnitDrop(Unit unit)
    {
        units.Remove(unit);
    }

    private void AuthorityHandleBuildingSpawn(Building building)
    {
        playerBuildingPlacer.Buildings.Add(building);
    }

    private void AuthorityHandleBuildingDrop(Building building)
    {
        playerBuildingPlacer.Buildings.Remove(building);
    }
    #endregion
}
