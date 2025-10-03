using UnityEngine;

public class CamRotate : MonoBehaviour
{
    [SerializeField] private float sensitivity = 5f;
    [SerializeField] private float maxAngle = 80f;
    [SerializeField] private float minAngle = -80f;

    private float xRotation = 0f;

    void Update()
    {
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;
        float rotationAmount = -mouseY;

        xRotation += rotationAmount;
        xRotation = Mathf.Clamp(xRotation, minAngle, maxAngle);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}