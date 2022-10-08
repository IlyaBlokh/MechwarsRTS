using System.Linq;
using Data;
using Data.Config;
using Mirror;
using Networking;
using UnityEngine;

namespace Utils
{
    public class TeamColorSetter : NetworkBehaviour
    {
        [SerializeField] private Renderer[] renderers;
        [SerializeField] private PlayersColors config;

        [SyncVar(hook = nameof(ClientHandleColorUpdate))]
        private ColorId playerColor;
        
        #region Server
        public override void OnStartServer()
        {
            RTSPlayer ownerPlayer = connectionToClient.identity.GetComponent<RTSPlayer>();
            playerColor = ownerPlayer.DisplayColor;
            Debug.Log($"setter color: {playerColor}");
        }
        #endregion

        #region Client
        private void ClientHandleColorUpdate(ColorId oldColor, ColorId newColor)
        {
            Debug.Log($"ClientHandleColorUpdate. new color: {newColor}");
            foreach (Renderer renderer in renderers)
            {
                renderer.material = config.TeamColors.First(x => x.ColorId == newColor).Material;
            }
        }

        #endregion
    }
}
