using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CameraControl
{
    public class Minimap : MonoBehaviour, IDragHandler, IPointerDownHandler
    {
        [SerializeField] private RectTransform minimapRect;
        [SerializeField] private float mapScale = 15f;

        private CameraController playerCamera = null;
        private Vector2 minimapPoint;
        private Vector2 lerpPosition;
        private Vector3 cameraPosition;

        private void Start()
        {
            InitNetworkClient();
        }

        private void InitNetworkClient()
        {
            NetworkClient.connection.identity.TryGetComponent(out RTSPlayer ownerPlayer);
            playerCamera = ownerPlayer.CameraController;
        }

        public void OnDrag(PointerEventData eventData)
        {
            MoveMinimapCamera(eventData.position);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            MoveMinimapCamera(eventData.position);
        }

        private void MoveMinimapCamera(Vector2 pointerPosition)
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(minimapRect, pointerPosition, null, out minimapPoint))
            {
                lerpPosition = new Vector2(
                    (minimapPoint.x - minimapRect.rect.x) / minimapRect.rect.width,
                    (minimapPoint.y - minimapRect.rect.y) / minimapRect.rect.height);
                cameraPosition = new Vector3(
                    Mathf.Lerp(-mapScale, mapScale, lerpPosition.x),
                    playerCamera.CameraTransform.position.y,
                    Mathf.Lerp(-mapScale, mapScale, lerpPosition.y));
                playerCamera.CameraTransform.position = cameraPosition + new Vector3(.0f, .0f, playerCamera.OffsetZ);
            }
        }
    }
}