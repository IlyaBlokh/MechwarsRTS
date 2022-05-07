using Mirror;
using UnityEngine;

public class TeamColorSetter : NetworkBehaviour
{
    [SerializeField] private Renderer[] renderers;

    [SyncVar(hook = nameof(ClientHandleColorUpdate))]
    private Color teamColor;

    private void Start()
    {
        RTSPlayer.OnPlayerInitialized += InitNetworkClient;
    }

    private void OnDestroy()
    {
        RTSPlayer.OnPlayerInitialized -= InitNetworkClient;
    }

    #region Client
    private void ClientHandleColorUpdate(Color oldColor, Color newColor)
    {
        foreach (Renderer renderer in renderers)
        {
            renderer.material.SetColor("_BaseColor", newColor);
        }
    }

    private void InitNetworkClient()
    {
        NetworkClient.connection.identity.TryGetComponent(out RTSPlayer ownerPlayer);
        teamColor = ownerPlayer.TeamColor;
    }
    #endregion
}
