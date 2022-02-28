using Combat;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Buildings
{
    [RequireComponent(typeof(Damageable))]
    public class UnitSpawnerBuilding : NetworkBehaviour, IPointerClickHandler, IDestructible
    {
        [SerializeField]
        private GameObject laserTankUnitPrefab;
        [SerializeField]
        private Transform spawnPoint;

        private Damageable damageable;

        private void Awake()
        {
            damageable = GetComponent<Damageable>();
        }

        #region Server

        public override void OnStartServer()
        {
            damageable.OnServerDestruct += HandleDestruction;
        }

        public override void OnStopServer()
        {
            damageable.OnServerDestruct -= HandleDestruction;
        }

        [Command]
        private void CmdSpawnLaserTankUnit()
        {
            var laserTankUnit = Instantiate(laserTankUnitPrefab,
               spawnPoint.position,
               spawnPoint.rotation);
            NetworkServer.Spawn(laserTankUnit, connectionToClient);
        }

        [Server]
        public void HandleDestruction()
        {
            NetworkServer.Destroy(gameObject);
        }
        #endregion

        #region Client
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            if (!hasAuthority) return;
            CmdSpawnLaserTankUnit();
        }
        #endregion
    }
}
