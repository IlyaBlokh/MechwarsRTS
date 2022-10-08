using UnityEngine;

namespace CameraControl
{
    public class FaceToCamera : MonoBehaviour
    {
        private UnityEngine.Camera mainCamera;
        private Quaternion targetRotation;

        private void Start()
        {
            mainCamera = UnityEngine.Camera.main;
        }

        void LateUpdate()
        {
            targetRotation = Quaternion.LookRotation(mainCamera.transform.forward);
            transform.rotation = targetRotation;
        }
    }
}