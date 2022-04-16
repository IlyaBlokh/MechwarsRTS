using Buildings;
using Mirror;
using System.Collections.Generic;
using Units;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
    private List<Unit> units = new List<Unit>();
    private List<Building> buildings = new List<Building>();

    public List<Unit> Units { get => units;}
    public List<Building> Buildings { get => buildings; }

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
