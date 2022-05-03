using Mirror;
using Networking;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.Menu
{
    public class LobbyWindow : MonoBehaviour
    {
        [SerializeField] private GameObject lobbyUI;

        private void Start()
        {
            RTSNetworkManager.ClientConnected += HandleClientConnected;
        }

        private void OnDestroy()
        {
            RTSNetworkManager.ClientConnected -= HandleClientConnected;
        }

        private void HandleClientConnected()
        {
            lobbyUI.SetActive(true);
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