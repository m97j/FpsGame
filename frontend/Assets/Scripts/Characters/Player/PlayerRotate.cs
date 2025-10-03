using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    [SerializeField] private float sensitivity = 5f;
    //private float yRotation = 0f;

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        transform.Rotate(Vector3.up, mouseX); // ���ڸ����� Y�� ���� ȸ��
    }
}