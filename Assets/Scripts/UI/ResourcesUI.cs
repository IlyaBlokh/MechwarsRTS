using Mirror;
using Resources;
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
            RTSPlayer.OnAuthorityStarted += InitPlayer;
        }

        private void OnDestroy()
        {
            RTSPlayer.OnAuthorityStarted -= InitPlayer;
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