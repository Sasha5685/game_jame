using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WellInteraction : MonoBehaviour, IInteractable
{
    [Header("Well Settings")]
    public Transform handle;         // Крутящаяся ручка колодца
    public Transform bucket;        // Ведро
    public Transform bucketSet;        // Ведро
    public Transform ropeAnchor;    // Точка крепления верёвки на колодце
    public float animationDuration = 2f;
    public float bucketMoveDistance = 5f;
    public float rotationSpeed = 360f;

    [Header("Rope Settings")]
    public LineRenderer ropeRenderer;
    public int ropeSegments = 5;    // Количество сегментов верёвки
    public float ropeWidth = 0.05f;

    private bool isAnimating = false;
    private bool isBucketDown = false;
    private Vector3 bucketStartPosition;
    private Vector3 bucketTargetPosition;

    [Header("Outline Settings")]
    public Color outlineColor = Color.yellow;      // Цвет контура
    public float defaultOutlineWidth = 3f;        // Ширина контура без наведения
    public float hoverOutlineWidth = 8f;          // Ширина контура при наведении
    public float pulseSpeed = 2.5f;               // Скорость пульсации
    [Range(0f, 1f)] public float pulseIntensity = 0.85f; // Насколько сильно пульсирует (0 = нет пульсации, 1 = сильная)

    private bool isHighlighted;
    private Outline outline;
    private float currentPulseValue;
    private void Start()
    {
        outline = gameObject.GetComponent<Outline>();
        bucketStartPosition = bucket.position;
        bucketTargetPosition = bucketStartPosition + Vector3.down * bucketMoveDistance;

        InitializeRope();
    }

    private void InitializeRope()
    {
        if (ropeRenderer == null)
        {
            ropeRenderer = gameObject.AddComponent<LineRenderer>();
        }

        ropeRenderer.startWidth = ropeWidth;
        ropeRenderer.endWidth = ropeWidth;
        ropeRenderer.positionCount = ropeSegments;
        ropeRenderer.material = new Material(Shader.Find("Unlit/Color")) { color = Color.gray };

        UpdateRope();
    }

    private void UpdateRope()
    {
        if (ropeRenderer == null || ropeAnchor == null) return;

        for (int i = 0; i < ropeSegments; i++)
        {
            float t = i / (float)(ropeSegments - 1);
            Vector3 point = Vector3.Lerp(ropeAnchor.position, bucketSet.position, t);

            // Добавляем небольшую кривизну верёвке
            if (i > 0 && i < ropeSegments - 1)
            {
                float curveAmount = Mathf.Sin(t * Mathf.PI) * 0.2f;
                point += transform.right * curveAmount;
            }

            ropeRenderer.SetPosition(i, point);
        }
    }

    public void SetHighlight(bool state)
    {
        isHighlighted = state;


        if (state)
        {
            // При наведении включаем пульсацию (она работает в Update)
            outline.OutlineWidth = hoverOutlineWidth;
        }
        else
        {
            // Без наведения — просто статичная обводка
            outline.OutlineWidth = defaultOutlineWidth;
        }
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
        float rotationDirection = isBucketDown ? -1f : 1f;

        while (elapsedTime < animationDuration)
        {
            handle.Rotate(Vector3.up, rotationDirection * rotationSpeed * Time.deltaTime);

            float progress = elapsedTime / animationDuration;
            bucket.position = Vector3.Lerp(startPos, endPos, progress);

            UpdateRope(); // Обновляем верёвку каждый кадр

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        bucket.position = endPos;
        UpdateRope();
        isBucketDown = !isBucketDown;
        if(isBucketDown == true)
        {
            bucket.gameObject.GetComponent<Bucket>().IsWoater = true;
        }
        isAnimating = false;
    }
    private void Update()
    {
        if (isHighlighted)
        {


            // Плавная пульсация контура (используем синусоиду для плавности)
            currentPulseValue = Mathf.Sin(Time.time * pulseSpeed) * pulseIntensity;
            float pulseWidth = hoverOutlineWidth * (1f + currentPulseValue);
            outline.OutlineWidth = pulseWidth;
        }
    }
    private void LateUpdate()
    {
        if (!isAnimating)
        {
            UpdateRope();
        }
    }
}