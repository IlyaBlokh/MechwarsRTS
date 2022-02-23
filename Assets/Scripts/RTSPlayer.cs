using Mirror;
using System.Collections.Generic;
using Units;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField]
    private GameObject unitCommandHandlerPrefab;

    [SerializeField]
    private List<Unit> units = new List<Unit>();

    private UnitCommandHandler unitCommandHandler;

    public List<Unit> Units { get => units;}
    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();
        Unit.OnServerUnitSpawned += ServerHandleUnitSpawn;
        Unit.OnServerUnitDrop += ServerHandleUnitDrop;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        Unit.OnServerUnitSpawned -= ServerHandleUnitSpawn;
        Unit.OnServerUnitDrop -= ServerHandleUnitDrop;

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
    #endregion

    #region Client

    public override void OnStartClient()
    {
        base.OnStartClient();
        var unitCommandHandlerObject = Instantiate(unitCommandHandlerPrefab, transform);
        unitCommandHandler = unitCommandHandlerObject.GetComponent<UnitCommandHandler>();
        NetworkServer.Spawn(unitCommandHandlerObject, connectionToClient);
        if (isClientOnly)
        {
            Unit.OnAuthorityUnitSpawned += AuthorityHandleUnitSpawn;
            Unit.OnAuthorityUnitDrop += AuthorityHandleUnitDrop;
        }
    }

    public override void OnStopClient()
    {
        if (isClientOnly)
        {
            Unit.OnAuthorityUnitSpawned -= AuthorityHandleUnitSpawn;
            Unit.OnAuthorityUnitDrop -= AuthorityHandleUnitDrop;
        }
        base.OnStopClient();
    }

    private void AuthorityHandleUnitSpawn(Unit unit)
    {
        if (hasAuthority)
            units.Add(unit);
    }

    private void AuthorityHandleUnitDrop(Unit unit)
    {
        if (hasAuthority)
            units.Remove(unit);
    }
    #endregion
}
