using Buildings;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI
{
    public class BuildButtonUI : MonoBehaviour
    {
        [SerializeField] private Button buildButton;
        [SerializeField] private TMP_Text buildCost;
        [SerializeField] private Building building;
        [SerializeField] private LayerMask floorMask;

        private Camera mainCamera;
        private GameObject buildingPreviewInstance;
        private Ray ray;
        private bool isPlacing = false;

        private void Start()
        {
            mainCamera = Camera.main;
            buildButton.image.sprite = building.Icon;
            buildCost.text = building.Price.ToString();
            buildButton.onClick.AddListener(OnBuildClick);
        }

        private void Update()
        {
            if (isPlacing)
                UpdatePlacing();
        }

        private void OnDestroy()
        {
            buildButton.onClick.RemoveAllListeners();
        }

        private void OnBuildClick()
        {
            if (isPlacing) StopPlacing();
            StartPlacing();
        }

        private void StartPlacing()
        {
            isPlacing = true;
            buildingPreviewInstance = Instantiate(building.PreviewGameobject);
            buildingPreviewInstance.SetActive(false);
        }

        private void UpdatePlacing()
        {
            ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hitInfo, floorMask))
            {
                buildingPreviewInstance.transform.position = hitInfo.point;
                if (!buildingPreviewInstance.activeSelf)
                    buildingPreviewInstance.SetActive(true);
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    Debug.Log("build here");
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