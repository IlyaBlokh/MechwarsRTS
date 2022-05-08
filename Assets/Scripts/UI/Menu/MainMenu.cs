using Config;
using Mirror;
using Steamworks;
using System;
using UnityEngine;

namespace UI.Menu
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private WelcomeWindow welcomeWindow;
        [SerializeField] private JoinLobbyWindow joinLobbyWindow;
        [SerializeField] NetworkConfig networkConfig;

        protected Callback<LobbyCreated_t> lobbyCreated;
        protected Callback<GameLobbyJoinRequested_t> lobbyJoinRequested;
        protected Callback<LobbyEnter_t> lobbyEnter;

        private void Start()
        {
            if (!networkConfig.UseSteam) return;
            lobbyCreated = new Callback<LobbyCreated_t>(OnLobbyCreated);
            lobbyJoinRequested = new Callback<GameLobbyJoinRequested_t>(OnLobbyJoinRequested);
            lobbyEnter = new Callback<LobbyEnter_t>(OnLobbyEnter);
        }

        private void OnLobbyCreated(LobbyCreated_t callback)
        {
            if (callback.m_eResult != EResult.k_EResultOK)
            {
                welcomeWindow.gameObject.SetActive(true);
                return;
            }
            NetworkManager.singleton.StartHost();
            SteamMatchmaking.SetLobbyData(
                new CSteamID(callback.m_ulSteamIDLobby),
                "HostAddress",
                SteamUser.GetSteamID().ToString());
        }

        private void OnLobbyJoinRequested(GameLobbyJoinRequested_t callback)
        {
            SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        }

        private void OnLobbyEnter(LobbyEnter_t callback)
        {
            if (NetworkServer.active) return;

            var hostAddress = SteamMatchmaking.GetLobbyData(
                new CSteamID(callback.m_ulSteamIDLobby),
                "HostAddress");
            NetworkManager.singleton.networkAddress = hostAddress;
            NetworkManager.singleton.StartClient();
            welcomeWindow.gameObject.SetActive(false);
        }

        public void HostLobby()
        {
            welcomeWindow.gameObject.SetActive(false);
            if (networkConfig.UseSteam)
            {
                SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
                return;
            }
            NetworkManager.singleton.StartHost();
        }

        public void JoinLobby()
        {
            welcomeWindow.gameObject.SetActive(false);
            joinLobbyWindow.gameObject.SetActive(true);
        }
    }
}