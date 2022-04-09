using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Networking
{
    public class RTSNetworkManager : NetworkManager
    {
        [SerializeField]
        private GameObject unitBasePrefab;
        [SerializeField]
        private GameLoopController gameLoopControllerPrefab;

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);
            var unitBaseBuilding= Instantiate(
                        unitBasePrefab,
                        conn.identity.transform.position,
                        conn.identity.transform.rotation);
            NetworkServer.Spawn(unitBaseBuilding, conn);
        }

        public override void OnServerSceneChanged(string newSceneName)
        {
            if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
            {
                var gameLoopController = Instantiate(gameLoopControllerPrefab);
                NetworkServer.Spawn(gameLoopController.gameObject);
            }
        }
    }
}