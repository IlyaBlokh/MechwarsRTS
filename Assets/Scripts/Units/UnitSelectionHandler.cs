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
        private LayerMask _layerMask;

        private List<Unit> _selectedUnits = new List<Unit>();
        private Camera _mainCamera;

        public LayerMask LayerMask { get => _layerMask; }
        public List<Unit> SelectedUnits { get => _selectedUnits; }

        #region Client
        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                _selectedUnits.ForEach(unit => unit.Deselect());
                _selectedUnits.Clear();
            }
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                ApplySelection();
            }
        }

        private void ApplySelection()
        {
            if (!hasAuthority) return;

            Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _layerMask)) return;

            if (!hit.collider.TryGetComponent(out Unit unit)) return;

            _selectedUnits.Add(unit);
            _selectedUnits.ForEach(unit => unit.Select());
        }
        #endregion
    }
}