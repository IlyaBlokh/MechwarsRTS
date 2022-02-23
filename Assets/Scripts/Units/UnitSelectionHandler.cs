using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Units
{
    public class UnitSelectionHandler : NetworkBehaviour
    {
        [SerializeField]
        private LayerMask layerMask;
        [SerializeField]
        private List<Unit> selectedUnits = new List<Unit>();
        private Camera mainCamera;

        public LayerMask LayerMask { get => layerMask; }
        public List<Unit> SelectedUnits { get => selectedUnits; }

        #region Client
        private void Awake()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                selectedUnits.ForEach(unit => unit.Deselect());
                selectedUnits.Clear();
            }
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                ApplySelection();
            }
        }

        private void ApplySelection()
        {
            if (!hasAuthority) return;

            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) return;

            if (!hit.collider.TryGetComponent(out Unit unit)) return;

            selectedUnits.Add(unit);
            selectedUnits.ForEach(unit => unit.Select());
        }
        #endregion
    }
}