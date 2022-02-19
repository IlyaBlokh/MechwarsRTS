using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Buildings
{
    public class UnitSpawnerBuilding : NetworkBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private GameObject _laserTankUnitPrefab;
        [SerializeField]
        private Transform _spawnPoint;

        #region Server
        [Command]
        private void CmdSpawnLaserTankUnit()
        {
            var _laserTankUnit = Instantiate(_laserTankUnitPrefab,
               _spawnPoint.position,
               _spawnPoint.rotation);
            NetworkServer.Spawn(_laserTankUnit, connectionToClient);
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
