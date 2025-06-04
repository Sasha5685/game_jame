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
        // ������ ���������� ���� ���� �� ����� �������
        if (eventData.button != PointerEventData.InputButton.Left) return;

        // ���� � ������� ���� �������
        if (Inventory.Instance.CursorSlot.IsUseSlot())
        {
            if (!CanPutItem(Inventory.Instance.CursorSlot.ItemType))
            {
                Debug.LogWarning($"������ �������� {Inventory.Instance.CursorSlot.ItemType.itemName} � ���� ����!");
                return; // ������ ����������
            }

            // ���� ���� ������
            if (!slot.IsUseSlot())
            {
                slot.AddItemToSlot(Inventory.Instance.CursorSlot.ItemType, Inventory.Instance.CursorSlot.Count);
                Inventory.Instance.CursorSlot.DestroySlot();
            }
            // ���� ���� ����� ����� �� ���������
            else if (slot.ItemType == Inventory.Instance.CursorSlot.ItemType)
            {
                slot.Count += Inventory.Instance.CursorSlot.Count;
                slot.CountText.text = slot.Count.ToString();
                Inventory.Instance.CursorSlot.DestroySlot();
            }
        }
        // ���� ������ ������ � �������� ����� �������
        else if (slot.IsUseSlot())
        {
            if (!canTakeItems)
            {
                Debug.LogWarning("������ �������� �������� �� ����� �����!");
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