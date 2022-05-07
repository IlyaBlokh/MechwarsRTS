using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using Combat;
using Networking;

namespace Units
{
    [RequireComponent(typeof(UnitSelectionHandler))]
    public class UnitCommandHandler : NetworkBehaviour
    {
        [SerializeField]
        private LayerMask layerMask;
        private UnitSelectionHandler unitSelectionHandler;
        private Camera mainCamera;

        private void Awake()
        {
            unitSelectionHandler = GetComponent<UnitSelectionHandler>();
        }

        private void Start()
        {
            GameLoopController.OnClientGameOver += ClientHandleGameOver;
        }

        private void OnDestroy()
        {
            GameLoopController.OnClientGameOver -= ClientHandleGameOver;
        }
        
        #region Client

        private void Update()
        {
            if (!hasAuthority) return;

            if (mainCamera == null) mainCamera = Camera.main;

            if (!Mouse.current.rightButton.wasPressedThisFrame) return;

            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) return;

            unitSelectionHandler.SelectedUnits.ForEach(unit =>
            {
                if (hit.collider.TryGetComponent(out Targetable targetable))
                {
                    if (!targetable.hasAuthority)
                    {
                        unit.GetUnitTargeter.CmdSetTarget(targetable);
                        return;
                    }
                }
                unit.GetUnitMovement.CmdTryMove(hit.point);
            });
        }

        private void ClientHandleGameOver(string winnerName)
        {
            enabled = false;
        }

        #endregion
    }
}
