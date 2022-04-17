using Buildings;
using Config;
using Mirror;
using Resources;
using System;
using System.Collections.Generic;
using Units;
using UnityEngine;

[RequireComponent(typeof(PlayerResources))]
public class RTSPlayer : NetworkBehaviour
{
    [SerializeField] BuildingsConfig buildingsConfig;

    private PlayerResources playerResources;
    private List<Unit> units = new List<Unit>();
    private List<Building> buildings = new List<Building>();

    public static event Action OnAuthorityStarted; 
    public List<Unit> Units { get => units;}
    public List<Building> Buildings { get => buildings; }
    public PlayerResources PlayerResources { get => playerResources; }

    private void Awake()
    {
        playerResources = GetComponent<PlayerResources>();
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

    [Command]
    public void CmdTryPlaceBuilding(int buildingId, Vector3 location)
    {
        var buildingToPlace = buildingsConfig.Buildings.Find(b => b.Id == buildingId);
        if (buildingToPlace == null) return;
        var buildingInstance = Instantiate(buildingToPlace.gameObject, location, buildingToPlace.transform.rotation);
        NetworkServer.Spawn(buildingInstance, connectionToClient);
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
        buildings.Add(building);
    }

    private void ServerHandleBuildingDrop(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) return;
        buildings.Remove(building);
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
        buildings.Add(building);
    }

    private void AuthorityHandleBuildingDrop(Building building)
    {
        buildings.Remove(building);
    }
    #endregion
}
