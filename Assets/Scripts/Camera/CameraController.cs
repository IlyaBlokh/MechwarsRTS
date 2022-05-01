using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CameraControl
{
    public class CameraController : NetworkBehaviour
    {
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float movementSpeed;
        [SerializeField] private float borderOffset;
        [SerializeField] private Vector2 screenBoundsX;
        [SerializeField] private Vector2 screenBoundsZ;
        [SerializeField] private float offsetZ = -60f;

        private PlayerControls playerControls;
        private Vector2 lastInput = Vector2.zero;
        private Vector2 mousePosition;
        private Vector3 movementDirection;
        private Vector3 cameraPos;

        public Transform CameraTransform { get => cameraTransform; }
        public float OffsetZ { get => offsetZ; }

        public override void OnStartAuthority()
        {
            cameraTransform.gameObject.SetActive(true);
            cameraTransform.position = new Vector3(cameraTransform.position.x, cameraTransform.position.y, cameraTransform.position.z + offsetZ);

            playerControls = new PlayerControls();
            playerControls.Player.CameraMovement.performed += HandleCameraMovementInput;
            playerControls.Player.CameraMovement.canceled += HandleCameraMovementInput;
            playerControls.Enable();
        }

        [ClientCallback]
        private void Update()
        {
            if (!hasAuthority || !Application.isFocused) { return; }
            UpdateCameraPosition();
        }

        private void UpdateCameraPosition()
        {
            cameraPos = cameraTransform.position;
            if (lastInput == Vector2.zero)
            {
                movementDirection = Vector3.zero;
                mousePosition = Mouse.current.position.ReadValue();
                if (mousePosition.y > Screen.height - borderOffset)
                {
                    movementDirection.z += 1;
                }
                else if (mousePosition.y < borderOffset)
                {
                    movementDirection.z -= 1;
                }

                if (mousePosition.x > Screen.width - borderOffset)
                {
                    movementDirection.x += 1;
                }
                else if (mousePosition.x < borderOffset)
                {
                    movementDirection.x -= 1;
                }
                cameraPos += movementDirection.normalized * movementSpeed * Time.deltaTime;
            }
            else
            {
                cameraPos += new Vector3(lastInput.x, .0f, lastInput.y) * movementSpeed * Time.deltaTime;
            }
            cameraPos.x = Mathf.Clamp(cameraPos.x, screenBoundsX.x, screenBoundsX.y);
            cameraPos.z = Mathf.Clamp(cameraPos.z, screenBoundsZ.x, screenBoundsZ.y);
            cameraTransform.position = cameraPos;
        }

        private void HandleCameraMovementInput(InputAction.CallbackContext ctx)
        {
            lastInput = ctx.ReadValue<Vector2>();
        }
    }
}