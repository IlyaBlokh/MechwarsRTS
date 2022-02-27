using UnityEngine;

public class FaceToCamera : MonoBehaviour
{
    private Quaternion targetRotation;

    void Update()
    {
        targetRotation = Quaternion.LookRotation(Camera.main.transform.forward);
        transform.rotation = targetRotation;
    }
}
