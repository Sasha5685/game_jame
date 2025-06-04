using System;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Item[] hotbarItems = new Item[6];
    public int[] itemCounts = new int[6];
    public int selectedSlot = 0;
    public bool isInContainerUI = false;

    // Текущий предмет для переноса
    public Item itemToTransfer;
    private int transferCount;

    public event Action OnInventoryChanged;
    public event Action<Item, int> OnStartItemTransfer;
    public event Action OnEndItemTransfer;

    public void StartItemTransfer(Item item, int count)
    {
        itemToTransfer = item;
        transferCount = count;
        OnStartItemTransfer?.Invoke(item, count);
    }

    public void EndItemTransfer()
    {
        itemToTransfer = null;
        transferCount = 0;
        OnEndItemTransfer?.Invoke();
    }

    public bool TryTransferItem(Inventory targetInventory, bool removeAfterTransfer = true)
    {
        if (itemToTransfer == null) return false;

        if (targetInventory.AddItem(itemToTransfer, transferCount))
        {
            if (removeAfterTransfer)
            {
                RemoveItem(selectedSlot, transferCount);
            }
            EndItemTransfer();
            return true;
        }
        return false;
    }

    private void Update()
    {
        if (!isInContainerUI)
        {
            // Переключение слотов только вне интерфейса
            for (int i = 0; i < 6; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    ChangeSelectedSlot(i);
                }
            }

            // Использование предмета по Q
            if (Input.GetKeyDown(KeyCode.Q))
            {
                UseSelectedItem();
            }
        }
    }
    public bool AddItem(Item item, int count = 1)
    {
        // Проверяем есть ли такой предмет в хотбаре и можно ли добавить в стек
        for (int i = 0; i < hotbarItems.Length; i++)
        {
            if (hotbarItems[i] == item && itemCounts[i] < item.maxStack)
            {
                itemCounts[i] += count;
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        // Ищем пустой слот
        for (int i = 0; i < hotbarItems.Length; i++)
        {
            if (hotbarItems[i] == null)
            {
                hotbarItems[i] = item;
                itemCounts[i] = count;
                OnInventoryChanged?.Invoke();
                return true;
            }
        }

        return false; // Нет места
    }

    public void RemoveItem(int slotIndex, int count = 1)
    {
        if (hotbarItems[slotIndex] != null)
        {
            itemCounts[slotIndex] -= count;

            if (itemCounts[slotIndex] <= 0)
            {
                hotbarItems[slotIndex] = null;
            }

            OnInventoryChanged?.Invoke();
        }
    }

    public void UseSelectedItem()
    {
        if (hotbarItems[selectedSlot] != null && !isInContainerUI)
        {
            hotbarItems[selectedSlot].Use();
            RemoveItem(selectedSlot, 1);
        }
    }

    public void ChangeSelectedSlot(int newSlot)
    {
        selectedSlot = Mathf.Clamp(newSlot, 0, hotbarItems.Length - 1);
        OnInventoryChanged?.Invoke();
    }

    public bool TransferItemToContainer(Inventory container, int slotIndex)
    {
        if (hotbarItems[slotIndex] == null) return false;

        if (container.AddItem(hotbarItems[slotIndex], itemCounts[slotIndex]))
        {
            RemoveItem(slotIndex, itemCounts[slotIndex]);
            return true;
        }
        return false;
    }
}