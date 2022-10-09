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
        }
        #endregion

        #region Client
        private void ClientHandleColorUpdate(ColorId oldColor, ColorId newColor)
        {
            foreach (Renderer rend in renderers)
            {
                TeamColor teamColor = config.TeamColors.First(x => x.ColorId == newColor);
                rend.material =
                    rend.gameObject.layer == LayerMask.NameToLayer("Minimap")
                        ? teamColor.MinimapMaterial
                        : teamColor.GameplayMaterial;

            }
        }

        #endregion
    }
}
