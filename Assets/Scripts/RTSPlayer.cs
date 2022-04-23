using Buildings;
using Config;
using Mirror;
using Resources;
using System;
using System.Collections.Generic;
using Units;
using UnityEngine;

[RequireComponent(typeof(PlayerResources))]
[RequireComponent(typeof(PlayerBuildingPlacer))]
public class RTSPlayer : NetworkBehaviour
{
    private PlayerResources playerResources;
    private PlayerBuildingPlacer playerBuildingPlacer;
    private List<Unit> units = new List<Unit>();
    private Color teamColor;

    public static event Action OnAuthorityStarted; 
    public List<Unit> Units { get => units;}
    public PlayerResources PlayerResources { get => playerResources; }
    public PlayerBuildingPlacer PlayerBuildingPlacer { get => playerBuildingPlacer;  }
    public Color TeamColor { get => teamColor; }

    private void Awake()
    {
        playerResources = GetComponent<PlayerResources>();
        playerBuildingPlacer = GetComponent<PlayerBuildingPlacer>();
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
    #endregion

    #region Client

    public override void OnStartAuthority()
    {
        OnAuthorityStarted?.Invoke();
        if (NetworkServer.active) return;
        Unit.OnAuthorityUnitSpawned += AuthorityHandleUnitSpawn;
        Unit.OnAuthorityUnitDrop += AuthorityHandleUnitDrop;
        Building.OnAuthorityBuildingSpawned += AuthorityHandleBuildingSpawn;
        Building.OnAuthorityBuildingDrop += AuthorityHandleBuildingDrop;
    }

    public override void OnStopClient()
    {
        if (isClientOnly && hasAuthority)
        {
            Unit.OnAuthorityUnitSpawned -= AuthorityHandleUnitSpawn;
            Unit.OnAuthorityUnitDrop -= AuthorityHandleUnitDrop;
            Building.OnAuthorityBuildingSpawned -= AuthorityHandleBuildingSpawn;
            Building.OnAuthorityBuildingDrop -= AuthorityHandleBuildingDrop;
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
