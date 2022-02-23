using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Units
{
    public class UnitSelectionHandler : NetworkBehaviour
    {
        [SerializeField]
        private RectTransform selectionFrame;
        [SerializeField]
        private LayerMask layerMask;

        private RTSPlayer player;
        private List<Unit> selectedUnits = new List<Unit>();
        private Camera mainCamera;
        private Vector2 startSelectionPosition;
        private Vector2 mousePos;
        private float selectionWidth;
        private float selectionHeight;

        public LayerMask LayerMask { get => layerMask; }
        public List<Unit> SelectedUnits { get => selectedUnits; }

        #region Client
        private void Awake()
        {
            mainCamera = Camera.main;
        }

        private void Start()
        {
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
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
            UpdateSelection();
        }

        private void UpdateSelection()
        {
            mousePos = Mouse.current.position.ReadValue();
            selectionWidth = mousePos.x - startSelectionPosition.x;
            selectionHeight = mousePos.y - startSelectionPosition.y;
            selectionFrame.sizeDelta = new Vector2(Mathf.Abs(selectionWidth), Mathf.Abs(selectionHeight));
            selectionFrame.anchoredPosition = startSelectionPosition + new Vector2(selectionWidth / 2, selectionHeight / 2);
        }

        private void ApplySelection()
        {
            selectionFrame.gameObject.SetActive(false);

            if (selectionFrame.sizeDelta.magnitude < float.Epsilon)
            {
                if (!hasAuthority) return;

                Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) return;

                if (!hit.collider.TryGetComponent(out Unit unit)) return;

                selectedUnits.Add(unit);
                selectedUnits.ForEach(unit => unit.Select());
            }
            else
            {
                Vector2 leftBottomCorner = selectionFrame.anchoredPosition - selectionFrame.sizeDelta / 2;
                Vector2 rightTopCorner = selectionFrame.anchoredPosition + selectionFrame.sizeDelta / 2;
                player.Units.ForEach(unit => {
                    var screenPosition = mainCamera.WorldToScreenPoint(unit.transform.position);
                    if (screenPosition.x > leftBottomCorner.x &&
                        screenPosition.x < rightTopCorner.x &&
                        screenPosition.y > leftBottomCorner.y &&
                        screenPosition.y < rightTopCorner.y)
                    {
                        selectedUnits.Add(unit);
                        unit.Select();
                        //selectedUnits.ForEach(unit => unit.Select());
                    }
                });
            }
        }

        #endregion
    }
}