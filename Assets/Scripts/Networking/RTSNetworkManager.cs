using Mirror;
using Steamworks;
using System;
using System.Collections.Generic;
using Data.Config;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Networking
{
    public class RTSNetworkManager : NetworkManager
    {
        [SerializeField] private GameObject unitBasePrefab;
        [SerializeField] private GameLoopController gameLoopControllerPrefab;
        [SerializeField] private PlayersConfig playersConfig;
        [SerializeField] private NetworkConfig networkConfig;

        private bool isGameInProgress;
        private List<RTSPlayer> players = new();

        public static Action ClientConnected;
        public static Action ClientDisonnected;

        public List<RTSPlayer> Players => players;

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

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);

            RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();
            players.Add(player);

            SetPlayerName(player);
            SetPlayerColor(player);
            player.SetIsPartyOwner(players.Count == 1);
        }

        private void SetPlayerName(RTSPlayer player)
        {
            string playerName = networkConfig.UseSteam
                ? SteamFriends.GetPersonaName()
                : $"Player {players.Count}";
            player.SetDisplayName(playerName);
        }

        private void SetPlayerColor(RTSPlayer player)
        {
            int nextColor = (players.Count - 1) % players.Count;
            player.SetDisplayColor(playersConfig.PlayersColors[nextColor]);
            Debug.Log($"player {player.name} has color {player.DisplayColor}");
        }

        #endregion

        #region Client
        public override void OnClientConnect()
        {
            base.OnClientConnect();
            ClientConnected?.Invoke();
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            ClientDisonnected?.Invoke();
        }

        public override void OnStopClient()
        {
            players.Clear();
        }
        #endregion
    }
}