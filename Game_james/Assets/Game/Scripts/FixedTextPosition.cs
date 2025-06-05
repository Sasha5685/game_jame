using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshPro))]
public class FixedTextPosition : MonoBehaviour
{
    public Transform target; // ������, ��� ������� ������ ���� �����
    public Vector3 offset = new Vector3(0, 1.5f, 0); // �������� ������

    private void LateUpdate()
    {
        if (target != null)
        {
            // ��������� ������� ������ ��� ��������
            transform.position = target.position + offset;

            // ������������ ����� � ������ (�� ������ �� ��� Y)
            transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y + 180, 0);
        }
    }
}