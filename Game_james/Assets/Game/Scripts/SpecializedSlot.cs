using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Slot))]
public class SpecializedSlot : MonoBehaviour, IPointerClickHandler
{
    [Header("Settings")]
    public Item allowedItem;
    public bool canPutItems = true;
    public bool canTakeItems = true;

    private Slot slot;

    private void Awake()
    {
        slot = GetComponent<Slot>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Полная блокировка если клик не левой кнопкой
        if (eventData.button != PointerEventData.InputButton.Left) return;

        // Если в курсоре есть предмет
        if (Inventory.Instance.CursorSlot.IsUseSlot())
        {
            if (!CanPutItem(Inventory.Instance.CursorSlot.ItemType))
            {
                Debug.LogWarning($"Нельзя положить {Inventory.Instance.CursorSlot.ItemType.itemName} в этот слот!");
                return; // Полная блокировка
            }

            // Если слот пустой
            if (!slot.IsUseSlot())
            {
                slot.AddItemToSlot(Inventory.Instance.CursorSlot.ItemType, Inventory.Instance.CursorSlot.Count);
                Inventory.Instance.CursorSlot.DestroySlot();
            }
            // Если слот занят таким же предметом
            else if (slot.ItemType == Inventory.Instance.CursorSlot.ItemType)
            {
                slot.Count += Inventory.Instance.CursorSlot.Count;
                slot.CountText.text = slot.Count.ToString();
                Inventory.Instance.CursorSlot.DestroySlot();
            }
        }
        // Если курсор пустой и пытаемся взять предмет
        else if (slot.IsUseSlot())
        {
            if (!canTakeItems)
            {
                Debug.LogWarning("Нельзя забирать предметы из этого слота!");
                return;
            }

            Inventory.Instance.CursorSlot.AddItemToSlot(slot.ItemType, slot.Count);
            slot.DestroySlot();
        }
    }

    private bool CanPutItem(Item item)
    {
        return canPutItems && (item == allowedItem);
    }
}