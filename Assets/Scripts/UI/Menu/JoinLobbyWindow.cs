using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;
using Networking;
using System;

namespace UI.Menu
{
    public class JoinLobbyWindow : MonoBehaviour
    {
        [SerializeField] private WelcomeWindow welcomeWindow;
        [SerializeField] private TMP_InputField inputAddress;
        [SerializeField] private Button joinBtn;

        private void OnEnable()
        {
            RTSNetworkManager.ClientConnected += HandleClientConnected;
            RTSNetworkManager.ClientDisonnected += HandleClientDisonnected;
        }

        private void OnDisable()
        {
            RTSNetworkManager.ClientConnected -= HandleClientConnected;
            RTSNetworkManager.ClientDisonnected -= HandleClientDisonnected;
        }

        public void JoinLobby()
        {
            NetworkManager.singleton.networkAddress = inputAddress.text;
            NetworkManager.singleton.StartClient();
            joinBtn.interactable = false;
        }

        public void Exit()
        {
            joinBtn.interactable = true;
            gameObject.SetActive(false);
            welcomeWindow.gameObject.SetActive(true);
        }

        private void HandleClientConnected()
        {
            joinBtn.interactable = true;
            gameObject.SetActive(false);
            welcomeWindow.gameObject.SetActive(false);
        }

        private void HandleClientDisonnected()
        {
            joinBtn.interactable = true;
        }
    }
}