using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    [SerializeField] private float sensitivity = 5f;
    //private float yRotation = 0f;

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        transform.Rotate(Vector3.up, mouseX); // 제자리에서 Y축 기준 회전
    }
}