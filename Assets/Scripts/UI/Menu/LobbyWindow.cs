using Mirror;
using Networking;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.Menu
{
    public class LobbyWindow : MonoBehaviour
    {
        [SerializeField] private GameObject lobbyUI;
        [SerializeField] private GameObject startGameBtn;
        [SerializeField] private TMP_Text[] playerNames;

        private List<RTSPlayer> connectedPlayers = new();

        private void Start()
        {
            RTSNetworkManager.ClientConnected += HandleClientConnected;
            RTSPlayer.OnPartyOwnerStateUpdated += HandlePartyOwnerStateUpdated;
            RTSPlayer.OnClientInfoUpdated += HandleClientInfoUpdated;
            RTSNetworkManager.ClientDisonnected += HandleClientInfoUpdated;
        }

        private void OnDestroy()
        {
            RTSNetworkManager.ClientConnected -= HandleClientConnected;
            RTSPlayer.OnPartyOwnerStateUpdated -= HandlePartyOwnerStateUpdated;
            RTSPlayer.OnClientInfoUpdated -= HandleClientInfoUpdated;
            RTSNetworkManager.ClientDisonnected -= HandleClientInfoUpdated;
        }

        private void HandleClientConnected()
        {
            lobbyUI.SetActive(true);
        }

        private void HandlePartyOwnerStateUpdated(bool state)
        {
            startGameBtn.gameObject.SetActive(state);
        }

        private void HandleClientInfoUpdated()
        {
            connectedPlayers = ((RTSNetworkManager)NetworkManager.singleton).Players;
            if (playerNames.Length < connectedPlayers.Count)
            {
                Debug.LogError("Not enough text elements for players!");
            }

            for (int i = 0; i < connectedPlayers.Count; i++)
            {
                playerNames[i].text = connectedPlayers[i].DisplayName;
            }
            for (int i = connectedPlayers.Count; i < playerNames.Length; i++)
            {
                playerNames[i].text = "Waiting for player...";
            }
            startGameBtn.SetActive(connectedPlayers.Count >= 2);
        }

        public void StartGame()
        {
            NetworkClient.connection.identity.GetComponent<RTSPlayer>().CmdStartGame();
        }

        public void LeaveLobby()
        {
            if (NetworkServer.active && NetworkClient.isConnected) 
            {
                NetworkManager.singleton.StopHost();
            }
            else
            {
                NetworkManager.singleton.StopClient();
                SceneManager.LoadScene(0);
            }
        }
    }
}