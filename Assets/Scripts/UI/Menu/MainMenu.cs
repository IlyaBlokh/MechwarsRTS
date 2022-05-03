using Mirror;
using UnityEngine;

namespace UI.Menu
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private WelcomeWindow welcomeWindow;
        [SerializeField] private JoinLobbyWindow joinLobbyWindow;
        public void HostLobby()
        {
            NetworkManager.singleton.StartHost();
            welcomeWindow.gameObject.SetActive(false);
        }

        public void JoinLobby()
        {
            welcomeWindow.gameObject.SetActive(false);
            joinLobbyWindow.gameObject.SetActive(true);
        }
    }
}