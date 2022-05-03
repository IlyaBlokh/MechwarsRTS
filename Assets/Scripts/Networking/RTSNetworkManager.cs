using Config;
using Mirror;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Networking
{
    public class RTSNetworkManager : NetworkManager
    {
        [SerializeField] private GameObject unitBasePrefab;
        [SerializeField] private GameLoopController gameLoopControllerPrefab;
        [SerializeField] PlayersConfig playersConfig;

        public static Action ClientConnected;
        public static Action ClientDisonnected;

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            ClientConnected?.Invoke();
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            ClientDisonnected?.Invoke();
        }

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);
            //set team color
            var nextColor = (NetworkServer.connections.Count - 1) % NetworkServer.connections.Count;
            conn.identity.GetComponent<RTSPlayer>().SetTeamColor(playersConfig.TeamColors[nextColor]);            
            //spawn base
/*            var unitBaseBuilding= Instantiate(
                        unitBasePrefab,
                        conn.identity.transform.position,
                        conn.identity.transform.rotation);
            NetworkServer.Spawn(unitBaseBuilding, conn);*/
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