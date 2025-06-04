using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HotbarUI : MonoBehaviour
{
/*    public Inventory inventory;
    public Image[] slotIcons;
    public Text[] slotCounts;
    public Image[] slotHighlights;

    private bool isDragging = false;
    private int dragStartSlot = -1;

    private void Start()
    {
        inventory.OnInventoryChanged += UpdateUI;
        inventory.OnStartItemTransfer += OnStartTransfer;
        inventory.OnEndItemTransfer += OnEndTransfer;
        UpdateUI();
    }

    private void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            int direction = scroll > 0 ? -1 : 1;
            int newSlot = (inventory.selectedSlot + direction + 6) % 6;
            inventory.ChangeSelectedSlot(newSlot);
        }

        // Переключение слотов цифрами
        for (int i = 0; i < 6; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                inventory.ChangeSelectedSlot(i);
            }
        }

        if (inventory.isInContainerUI) return;
        // Использование предмета по Q
        if (Input.GetKeyDown(KeyCode.Q))
        {
            inventory.UseSelectedItem();
        }

        // Перенос предмета правой кнопкой мыши
        if (isDragging && Input.GetMouseButtonUp(1))
        {
            inventory.EndItemTransfer();
            isDragging = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            int slotIndex = GetSlotIndex(eventData.position);
            if (slotIndex >= 0 && inventory.hotbarItems[slotIndex] != null)
            {
                dragStartSlot = slotIndex;
                inventory.StartItemTransfer(
                    inventory.hotbarItems[slotIndex],
                    inventory.itemCounts[slotIndex]
                );
                isDragging = true;
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDragging && eventData.button == PointerEventData.InputButton.Right)
        {
            isDragging = false;
            inventory.EndItemTransfer();
        }
    }

    private int GetSlotIndex(Vector2 position)
    {
        for (int i = 0; i < slotIcons.Length; i++)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(
                slotIcons[i].rectTransform, position))
            {
                return i;
            }
        }
        return -1;
    }

    private void OnStartTransfer(Item item, int count)
    {
        // Можно добавить визуальный эффект переноса
        Debug.Log($"Started transfer: {item.itemName} x{count}");
    }

    private void OnEndTransfer()
    {
        // Сброс визуальных эффектов переноса
    }

    private void UpdateUI()
    {
        for (int i = 0; i < inventory.hotbarItems.Length; i++)
        {
            if (inventory.hotbarItems[i] != null)
            {
                slotIcons[i].sprite = inventory.hotbarItems[i].icon;
                slotIcons[i].enabled = true;
                slotCounts[i].text = inventory.itemCounts[i] > 1 ? inventory.itemCounts[i].ToString() : "";
            }
            else
            {
                slotIcons[i].enabled = false;
                slotCounts[i].text = "";
            }

            slotHighlights[i].enabled = i == inventory.selectedSlot;
        }
    }

    private void OnDestroy()
    {
        inventory.OnInventoryChanged -= UpdateUI;
        inventory.OnStartItemTransfer -= OnStartTransfer;
        inventory.OnEndItemTransfer -= OnEndTransfer;
    }*/
}