using Mirror;
using Networking;
using TMPro;
using UnityEngine;

namespace UI
{
    public class ResourcesUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text creditsAmountText;
        private RTSPlayer player;
        private void Start()
        {
            InitPlayer();
        }

        private void OnDestroy()
        {
            player.PlayerResources.OnClientCreditsUpdated -= DisplayCreditsAmount;
        }

        private void InitPlayer()
        {
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
            player.PlayerResources.OnClientCreditsUpdated += DisplayCreditsAmount;
            DisplayCreditsAmount(player.PlayerResources.Credits);
        }

        private void DisplayCreditsAmount(int newAmount)
        {
            creditsAmountText.text = newAmount.ToString();
        }
    }
}