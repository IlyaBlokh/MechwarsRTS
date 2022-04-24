using Buildings;
using TMPro;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System;

namespace UI
{
    public class BuildButtonUI : MonoBehaviour
    {
        [SerializeField] private Button buildButton;
        [SerializeField] private TMP_Text buildCost;
        [SerializeField] private Building building;
        [SerializeField] private LayerMask floorMask;

        private Camera mainCamera;
        private RTSPlayer ownerPlayer = null;
        private GameObject buildingPreviewInstance;
        private Renderer buildingPreviewRenderer;
        private Color rendererColor;
        private Ray ray;
        private bool isPlacing = false;

        private void Start()
        {
            mainCamera = Camera.main;
            buildButton.image.sprite = building.Icon;
            buildCost.text = building.Price.ToString();
            buildButton.onClick.AddListener(OnBuildClick);
            RTSPlayer.OnAuthorityStarted += InitNetworkClient;
        }

        private void InitNetworkClient()
        {
            NetworkClient.connection.identity.TryGetComponent(out ownerPlayer);
        }

        private void Update()
        {
            if (isPlacing)
                UpdatePlacing();
        }

        private void OnDestroy()
        {
            buildButton.onClick.RemoveAllListeners();
            RTSPlayer.OnAuthorityStarted -= InitNetworkClient;
        }

        private void OnBuildClick()
        {
            if (isPlacing) StopPlacing();
            StartPlacing();
        }

        private void StartPlacing()
        {
            if (ownerPlayer.PlayerResources.Credits < building.Price) return;
            isPlacing = true;
            buildingPreviewInstance = Instantiate(building.PreviewGameobject);
            buildingPreviewRenderer = buildingPreviewInstance.GetComponent<Renderer>();
            buildingPreviewInstance.SetActive(false);
        }

        private void UpdatePlacing()
        {
            ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, floorMask))
            {
                buildingPreviewInstance.transform.position = 
                    new Vector3(hitInfo.point.x, buildingPreviewInstance.transform.position.y, hitInfo.point.z);
                if (!buildingPreviewInstance.activeSelf)
                    buildingPreviewInstance.SetActive(true);

                rendererColor = ownerPlayer.PlayerBuildingPlacer.IsPlacingAllowed(building.GetComponent<BoxCollider>(), hitInfo.point) ?
                    Color.green : Color.red;
                buildingPreviewRenderer.material.SetColor("_BaseColor", rendererColor);

                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    if (ownerPlayer == null)
                    {
                        Debug.LogError("Can't retrieve RTSPlayer");
                    }
                    else
                    {
                        ownerPlayer.PlayerBuildingPlacer.CmdTryPlaceBuilding(building.Id, hitInfo.point);
                    }
                    StopPlacing();
                }else if (Mouse.current.rightButton.wasPressedThisFrame)
                {
                    StopPlacing();
                }
            }
        }

        private void StopPlacing()
        {
            Destroy(buildingPreviewInstance);
            isPlacing = false;
        }
    }
}