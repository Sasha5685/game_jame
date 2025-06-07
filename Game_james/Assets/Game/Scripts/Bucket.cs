using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bucket : MonoBehaviour, IInteractable
{
    public bool IsWoater;
    public GameObject Water;
    [Header("Outline Settings")]
    public Color outlineColor = Color.yellow;      // Цвет контура
    public float defaultOutlineWidth = 3f;        // Ширина контура без наведения
    public float hoverOutlineWidth = 8f;          // Ширина контура при наведении
    public float pulseSpeed = 2.5f;               // Скорость пульсации
    [Range(0f, 1f)] public float pulseIntensity = 0.85f; // Насколько сильно пульсирует (0 = нет пульсации, 1 = сильная)

    private bool isHighlighted;
    private Outline outline;
    private float currentPulseValue;

    public Item ItemData;
    public int ItemIntAdd;
    public void Start()
    {
        outline = gameObject.GetComponent<Outline>();
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
    private void Update()
    {
        if(IsWoater == true)
        {
            Water.SetActive(true);
        }
        else
        {
            Water.SetActive(false);
        }
        if (isHighlighted && IsWoater == true)
        {


            // Плавная пульсация контура (используем синусоиду для плавности)
            currentPulseValue = Mathf.Sin(Time.time * pulseSpeed) * pulseIntensity;
            float pulseWidth = hoverOutlineWidth * (1f + currentPulseValue);
            outline.OutlineWidth = pulseWidth;
        }
    }
    public void Interact()
    {
        if (IsWoater == true)
        {
            Inventory playerInventory = FindObjectOfType<Inventory>();
            if (playerInventory != null && ItemData != null && playerInventory.FoundFreeSlot() > -1)
            {
                playerInventory.AddItemToSlot(playerInventory.FoundFreeSlot(), ItemData, ItemIntAdd);
                IsWoater = false;
            }
        }
    }
}
