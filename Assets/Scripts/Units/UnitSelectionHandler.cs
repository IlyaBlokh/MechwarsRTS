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
        private RectTransform selectionFrame;
        [SerializeField]
        private LayerMask layerMask;
        
        private List<Unit> selectedUnits = new List<Unit>();
        private Camera mainCamera;
        private Vector2 startSelectionPosition;

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
                InitSelection(); 
            }
            else
            if (Mouse.current.leftButton.isPressed)
            {
                UpdateSelection();
            }
            else
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                ApplySelection();
            }
        }

        private void InitSelection()
        {
            selectedUnits.ForEach(unit => unit.Deselect());
            selectedUnits.Clear();
            selectionFrame.gameObject.SetActive(true);
            startSelectionPosition = Mouse.current.position.ReadValue();
        }

        private void UpdateSelection()
        {
            var mousePos = Mouse.current.position.ReadValue();
            var width = mousePos.x - startSelectionPosition.x;
            var height = mousePos.y - startSelectionPosition.y;
            selectionFrame.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
            selectionFrame.anchoredPosition = startSelectionPosition + new Vector2(width / 2, height / 2);
        }

        private void ApplySelection()
        {
            selectionFrame.gameObject.SetActive(false);
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