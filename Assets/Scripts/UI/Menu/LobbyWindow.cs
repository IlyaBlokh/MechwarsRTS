using Mirror;
using Networking;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.Menu
{
    public class LobbyWindow : MonoBehaviour
    {
        [SerializeField] private GameObject lobbyUI;
        [SerializeField] private Button startGameBtn;

        private void Start()
        {
            RTSNetworkManager.ClientConnected += HandleClientConnected;
            RTSPlayer.OnPartyOwnerStateUpdated += HandlePartyOwnerStateUpdated;
        }

        private void OnDestroy()
        {
            RTSNetworkManager.ClientConnected -= HandleClientConnected;
            RTSPlayer.OnPartyOwnerStateUpdated -= HandlePartyOwnerStateUpdated;
        }

        private void HandleClientConnected()
        {
            lobbyUI.SetActive(true);
        }

        private void HandlePartyOwnerStateUpdated(bool state)
        {
            startGameBtn.gameObject.SetActive(state);
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