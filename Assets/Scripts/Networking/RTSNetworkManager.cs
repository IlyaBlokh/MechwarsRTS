using Config;
using Mirror;
using Steamworks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Networking
{
    public class RTSNetworkManager : NetworkManager
    {
        [SerializeField] private GameObject unitBasePrefab;
        [SerializeField] private GameLoopController gameLoopControllerPrefab;
        [SerializeField] PlayersConfig playersConfig;
        [SerializeField] NetworkConfig networkConfig;

        private bool isGameInProgress = false;
        private List<RTSPlayer> players = new List<RTSPlayer>();

        public static Action ClientConnected;
        public static Action ClientDisonnected;

        public List<RTSPlayer> Players { get => players; }

        #region Server
        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            if (isGameInProgress)
            {
                conn.Disconnect();
            }
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();
            players.Remove(player);
            ClientDisonnected?.Invoke();
            base.OnServerDisconnect(conn);
        }

        public override void OnStopServer()
        {
            players.Clear();
            isGameInProgress = false;
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);

            RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();
            players.Add(player);

            string playerName;
            if (networkConfig.UseSteam)
            {
                playerName = SteamFriends.GetPersonaName();
            }
            else
            {
                playerName = $"Player {players.Count}";
            }
            player.SetDisplayName(playerName);
            var nextColor = (players.Count - 1) % players.Count;
            player.SetTeamColor(playersConfig.TeamColors[nextColor]);
            
            player.SetIsPartyOwner(players.Count == 1);
        }

        public override void OnServerSceneChanged(string newSceneName)
        {
            if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
            {
                var gameLoopController = Instantiate(gameLoopControllerPrefab);
                NetworkServer.Spawn(gameLoopController.gameObject);

                //spawn bases
                players.ForEach(player => {
                    var unitBaseBuilding = Instantiate(
                        unitBasePrefab,
                        GetStartPosition().position,
                        Quaternion.identity);
                    NetworkServer.Spawn(unitBaseBuilding, player.connectionToClient);
                });
            }
        }

        public void StartGame()
        {
            if (players.Count < 2) return;
            isGameInProgress = true;
            ServerChangeScene("Scene_Map_01");
        }
        #endregion

        #region Client
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

        public override void OnStopClient()
        {
            players.Clear();
        }
        #endregion
    }
}