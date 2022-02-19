using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

namespace Units
{
    [RequireComponent(typeof(UnitSelectionHandler))]
    public class UnitCommandHandler : NetworkBehaviour
    {
        private UnitSelectionHandler _unitSelectionHandler;
        private Camera _mainCamera;
        private void Awake()
        {
            _unitSelectionHandler = GetComponent<UnitSelectionHandler>();
            _mainCamera = Camera.main;
        }

        #region Client

        private void Update()
        {
            if (!hasAuthority) return;

            if (!Mouse.current.rightButton.wasPressedThisFrame) return;

            Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _unitSelectionHandler.LayerMask)) return;

            _unitSelectionHandler.SelectedUnits.ForEach(unit =>
            {
                unit.GetUnitMovement.CmdMove(hit.point);
            });
        }

        #endregion
    }
}
