using Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject parent;
        [SerializeField]
        private TMP_Text winnerText;
        [SerializeField]
        private Button exitBtn;
        void Start()
        {
            GameLoopController.ClientGameOver += ShowGameOverUI;
            exitBtn.onClick.AddListener(ExitToLobby);
            parent.SetActive(false);
        }

        private void OnDestroy()
        {
            GameLoopController.ClientGameOver -= ShowGameOverUI;
            exitBtn.onClick.RemoveListener(ExitToLobby);
        }

        private void ShowGameOverUI(string winnerName)
        {
            parent.SetActive(true);
            winnerText.text = $"{winnerName} has won the match";
        }

        private void ExitToLobby()
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopServer();
            }
            else
            {
                NetworkManager.singleton.StopClient();
            }
        }
    }
}