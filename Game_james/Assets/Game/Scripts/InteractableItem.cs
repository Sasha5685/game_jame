using TMPro;
using UnityEngine;


public class InteractableItem : MonoBehaviour, IInteractable
{
    [Header("Interaction Settings")]
    public KeyCode interactionKey = KeyCode.E;

    [Header("Outline Settings")]
    public Color outlineColor = Color.yellow;      // Цвет контура
    public float defaultOutlineWidth = 3f;        // Ширина контура без наведения
    public float hoverOutlineWidth = 8f;          // Ширина контура при наведении
    public float pulseSpeed = 2.5f;               // Скорость пульсации
    [Range(0f, 1f)] public float pulseIntensity = 0.85f; // Насколько сильно пульсирует (0 = нет пульсации, 1 = сильная)

    public Item ItemData;
    public int ItemIntAdd;

    private bool isHighlighted;
    private Outline outline;
    private float currentPulseValue;

    private void Start()
    {
        InitializeOutline();
    }

    private void InitializeOutline()
    {
        outline = gameObject.GetComponent<Outline>();
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
        Inventory playerInventory = FindObjectOfType<Inventory>();
        if (playerInventory != null && ItemData != null && playerInventory.FoundFreeSlot() > -1)
        {
            playerInventory.AddItemToSlot(playerInventory.FoundFreeSlot(), ItemData, ItemIntAdd);

            Destroy(gameObject);
        }
    }


}