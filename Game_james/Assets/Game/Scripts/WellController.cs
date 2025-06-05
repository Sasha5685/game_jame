using UnityEngine;
using System.Collections;

public class WellInteraction : MonoBehaviour, IInteractable
{
    [Header("Well Settings")]
    public Transform handle; // ���������� ����� �������
    public Transform bucket; // �����, ������� ����� ���������
    public float animationDuration = 2f; // ������������ ��������
    public float bucketMoveDistance = 5f; // ��������� �������� �����
    public float rotationSpeed = 360f; // �������� �������� ����� (������� � �������)

    private bool isAnimating = false;
    private bool isBucketDown = false;
    private Vector3 bucketStartPosition;
    private Vector3 bucketTargetPosition;

    private void Start()
    {
        bucketStartPosition = bucket.position;
        bucketTargetPosition = bucketStartPosition + Vector3.down * bucketMoveDistance;
    }

    public void SetHighlight(bool state)
    {
        // ������ ����������, ��� ��� ��������� �� ���������
    }

    public void Interact()
    {
        if (!isAnimating)
        {
            StartCoroutine(AnimateWell());
        }
    }

    private IEnumerator AnimateWell()
    {
        isAnimating = true;

        Vector3 startPos = isBucketDown ? bucketTargetPosition : bucketStartPosition;
        Vector3 endPos = isBucketDown ? bucketStartPosition : bucketTargetPosition;

        float elapsedTime = 0f;
        float rotationDirection = isBucketDown ? -1f : 1f; // ����������� �������� �����

        while (elapsedTime < animationDuration)
        {
            // ������� ����� ������ ��� Z (�������� � Vector3.forward �� Vector3.forward)
            // Vector3.forward ��� ������������� ��� Z, ������� ��������� ��� ����
            handle.Rotate(Vector3.up, rotationDirection * rotationSpeed * Time.deltaTime);

            // ������� �����
            float progress = elapsedTime / animationDuration;
            bucket.position = Vector3.Lerp(startPos, endPos, progress);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        bucket.position = endPos;
        isBucketDown = !isBucketDown;
        isAnimating = false;
    }
}