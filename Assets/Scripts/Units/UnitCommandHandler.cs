using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using Combat;

namespace Units
{
    [RequireComponent(typeof(UnitSelectionHandler))]
    public class UnitCommandHandler : NetworkBehaviour
    {
        private UnitSelectionHandler unitSelectionHandler;
        private Camera mainCamera;
        private void Awake()
        {
            unitSelectionHandler = GetComponent<UnitSelectionHandler>();
            mainCamera = Camera.main;
        }

        #region Client

        private void Update()
        {
            if (!hasAuthority) return;

            if (!Mouse.current.rightButton.wasPressedThisFrame) return;

            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, unitSelectionHandler.LayerMask)) return;

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
                unit.GetUnitTargeter.ClearTarget();
                unit.GetUnitMovement.CmdTryMove(hit.point);
            });
        }

        #endregion
    }
}
