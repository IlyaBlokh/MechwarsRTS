using Mirror;
using UnityEngine;

public class TeamColorSetter : NetworkBehaviour
{
    [SerializeField] private Renderer[] renderers;

    [SyncVar(hook = nameof(ClientHandleColorUpdate))]
    private Color teamColor;

    #region Server
    public override void OnStartServer()
    {
        RTSPlayer ownerPlayer = connectionToClient.identity.GetComponent<RTSPlayer>();
        teamColor = ownerPlayer.TeamColor;
    }
    #endregion

    #region Client
    private void ClientHandleColorUpdate(Color oldColor, Color newColor)
    {
        foreach (Renderer renderer in renderers)
        {
            renderer.material.SetColor("_BaseColor", newColor);
        }
    }
    #endregion
}
