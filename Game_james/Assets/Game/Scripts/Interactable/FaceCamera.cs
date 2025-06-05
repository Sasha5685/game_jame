using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    public Transform parentObject;
    public Vector3 localOffset;

    void Update()
    {
        if (parentObject != null)
        {
            // ������� ������ ������� �� ��������, �� �� ���������
            transform.position = parentObject.position + parentObject.TransformDirection(localOffset);

            // ����� ������ ������� �� ������ (��� � Billboard)
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        }
    }
}