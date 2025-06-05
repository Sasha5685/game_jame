using UnityEngine;
using System.Collections;

public class WellInteraction : MonoBehaviour, IInteractable
{
    [Header("Well Settings")]
    public Transform handle; // Крутящаяся ручка колодца
    public Transform bucket; // Ведро, которое будет двигаться
    public float animationDuration = 2f; // Длительность анимации
    public float bucketMoveDistance = 5f; // Дистанция движения ведра
    public float rotationSpeed = 360f; // Скорость вращения ручки (градусы в секунду)

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
        // Пустая реализация, так как подсветка не требуется
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
        float rotationDirection = isBucketDown ? -1f : 1f; // Направление вращения ручки

        while (elapsedTime < animationDuration)
        {
            // Вращаем ручку вокруг оси Z (изменили с Vector3.forward на Vector3.forward)
            // Vector3.forward уже соответствует оси Z, поэтому оставляем как есть
            handle.Rotate(Vector3.up, rotationDirection * rotationSpeed * Time.deltaTime);

            // Двигаем ведро
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