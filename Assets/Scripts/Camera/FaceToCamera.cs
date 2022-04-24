using UnityEngine;

namespace CameraControl
{
    public class FaceToCamera : MonoBehaviour
    {
        private Camera mainCamera;
        private Quaternion targetRotation;

        private void Start()
        {
            mainCamera = Camera.main;
        }

        void LateUpdate()
        {
            targetRotation = Quaternion.LookRotation(mainCamera.transform.forward);
            transform.rotation = targetRotation;
        }
    }
}