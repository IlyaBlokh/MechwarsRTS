using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Buildings
{
    public class UnitSpawnerBehaviour : NetworkBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private GameObject unitToSpawnPrefab;
        [SerializeField]
        private Transform spawnPoint;

        #region Server
        [Command]
        private void CmdSpawnLaserTankUnit()
        {
            var laserTankUnit = Instantiate(unitToSpawnPrefab,
               spawnPoint.position,
               spawnPoint.rotation);
            NetworkServer.Spawn(laserTankUnit, connectionToClient);
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
