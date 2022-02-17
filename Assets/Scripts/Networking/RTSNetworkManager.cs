using Mirror;
using UnityEngine;

namespace Networking
{
    public class RTSNetworkManager : NetworkManager
    {
        [SerializeField]
        private GameObject _unitSpawnerBuildingPrefab;

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);
            var _playerUnitSpawner = Instantiate(
                        _unitSpawnerBuildingPrefab,
                        conn.identity.transform.position,
                        conn.identity.transform.rotation);
            NetworkServer.Spawn(_playerUnitSpawner, conn);
        }
    }
}