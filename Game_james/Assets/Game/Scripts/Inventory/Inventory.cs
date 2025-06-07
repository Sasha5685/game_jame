using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int SelectedSlot;
    public Slot[] Slots;
    public PointCoursorSlot CursorSlot;

    public static Inventory Instance;

    [Header("Throwing Settings")]
    public Transform throwPoint; // Сюда перетащите ваш ThrowPoint из сцены
    public float baseThrowForce = 10f;
    public float throwUpwardForce = 2f;
    private void Awake ()
    {
        Instance = this;
        for (int i = 0; i < Slots.Length; i++)
        {
            Slots[i].DestroySlot();
        }
        if (Slots.Length > 0)
        {
            UpdateSelection(SelectedSlot);
        }
    }

    private void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            int direction = scroll > 0 ? -1 : 1;
            int newSlot = (SelectedSlot + direction + Slots.Length) % Slots.Length;
            ChangeSelectedSlot(newSlot);
        }

        if (Input.GetKeyDown(KeyCode.Escape) && CursorSlot.IsUseSlot())
        {
            ReturnCursorItemToInventory();
        }

        if (Input.GetKeyDown(KeyCode.F) && Slots[SelectedSlot].IsUseSlot())
        {
            ThrowItemFromSelectedSlot();
        }
    }

    public void ChangeSelectedSlot(int newSlot)
    {
        Slots[SelectedSlot].SelectedSlot(false);
        SelectedSlot = newSlot;
        Slots[SelectedSlot].SelectedSlot(true);
    }

    public void AddItemToSlot(int Slot, Item item, int Count)
    {
        Slots[Slot].AddItemToSlot(item, Count);
    }

    private void UpdateSelection(int slotIndex)
    {
        for (int i = 0; i < Slots.Length; i++)
        {
            Slots[i].SelectedSlot(i == slotIndex);
        }
    }
    public int FoundFreeSlot()
    {
        for(int i = 0;i < Slots.Length;i++)
        {
            if (Slots[i].IsUseSlot() == false) { return i; }
        }
        return -1;
    }

    public void ThrowItemFromSelectedSlot()
    {
        if (!Slots[SelectedSlot].IsUseSlot()) return;

        Slot selectedSlot = Slots[SelectedSlot];

        if (selectedSlot.ItemType.prefab == null)
        {
            Debug.LogWarning("This item doesn't have a physical representation!");
            return;
        }

        // Создаем объект в точке броска
        GameObject thrownItem = Instantiate(
            selectedSlot.ItemType.prefab,
            throwPoint.position,
            throwPoint.rotation
        );

        // Добавляем физику
        Rigidbody rb = thrownItem.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 forceDirection = throwPoint.forward;

            // Комбинируем силу: вперед + немного вверх
            Vector3 forceToAdd = forceDirection * baseThrowForce +
                                throwPoint.up * throwUpwardForce;

            rb.AddForce(forceToAdd, ForceMode.Impulse);

            // Добавляем случайное вращение для реалистичности
            rb.AddTorque(new Vector3(
                Random.Range(-5f, 5f),
                Random.Range(-5f, 5f),
                Random.Range(-5f, 5f)
            ), ForceMode.Impulse);
        }

        // Уменьшаем количество предметов
        selectedSlot.Count--;
        selectedSlot.CountText.text = selectedSlot.Count.ToString();

        if (selectedSlot.Count <= 0)
        {
            selectedSlot.DestroySlot();
        }
    }
    public void HandleLeftClick(Slot clickedSlot)
    {
        // Для специализированных слотов ничего не делаем - вся логика в SpecializedSlot
        if (clickedSlot.GetComponent<SpecializedSlot>() != null) return;

        // Стандартная логика только для обычных слотов
        if (CursorSlot.IsUseSlot())
        {
            if (!clickedSlot.IsUseSlot())
            {
                clickedSlot.AddItemToSlot(CursorSlot.ItemType, CursorSlot.Count);
                CursorSlot.DestroySlot();
            }
            else if (clickedSlot.ItemType == CursorSlot.ItemType)
            {
                clickedSlot.Count += CursorSlot.Count;
                clickedSlot.CountText.text = clickedSlot.Count.ToString();
                CursorSlot.DestroySlot();
            }
        }
        else if (clickedSlot.IsUseSlot())
        {
            CursorSlot.AddItemToSlot(clickedSlot.ItemType, clickedSlot.Count);
            clickedSlot.DestroySlot();
        }
    }

    public void HandleRightClick(Slot clickedSlot)
    {
        var specialized = clickedSlot.GetComponent<SpecializedSlot>();
        if (specialized != null && !specialized.canTakeItems)
        {
            Debug.Log("Из этого слота нельзя забирать предметы");
            return;
        }

        if (CursorSlot.IsUseSlot())
        {
            ReturnCursorItemToInventory();
        }
        else if (clickedSlot.IsUseSlot())
        {
            int halfCount = Mathf.CeilToInt(clickedSlot.Count / 2f);
            int remainingCount = clickedSlot.Count - halfCount;

            CursorSlot.AddItemToSlot(clickedSlot.ItemType, halfCount);

            if (remainingCount <= 0)
            {
                clickedSlot.DestroySlot();
            }
            else
            {
                clickedSlot.Count = remainingCount;
                clickedSlot.CountText.text = remainingCount.ToString();
            }
        }
    }

    private void ReturnCursorItemToInventory()
    {
        int freeSlot = FoundFreeSlot();
        if (freeSlot != -1)
        {
            Slots[freeSlot].AddItemToSlot(CursorSlot.ItemType, CursorSlot.Count);
            CursorSlot.DestroySlot();
        }
        else
        {
            Debug.Log("No free slots available!");
        }
    }

    //public Item[] hotbarItems = new Item[6];
    //public int[] itemCounts = new int[6];
    //public int selectedSlot = 0;
    //public bool isInContainerUI = false;

    //// Текущий предмет для переноса
    //public Item itemToTransfer;
    //private int transferCount;

    //public event Action OnInventoryChanged;
    //public event Action<Item, int> OnStartItemTransfer;
    //public event Action OnEndItemTransfer;

    //public void StartItemTransfer(Item item, int count)
    //{
    //    itemToTransfer = item;
    //    transferCount = count;
    //    OnStartItemTransfer?.Invoke(item, count);
    //}

    //public void EndItemTransfer()
    //{
    //    itemToTransfer = null;
    //    transferCount = 0;
    //    OnEndItemTransfer?.Invoke();
    //}

    //public bool TryTransferItem(Inventory targetInventory, bool removeAfterTransfer = true)
    //{
    //    if (itemToTransfer == null) return false;

    //    if (targetInventory.AddItem(itemToTransfer, transferCount))
    //    {
    //        if (removeAfterTransfer)
    //        {
    //            RemoveItem(selectedSlot, transferCount);
    //        }
    //        EndItemTransfer();
    //        return true;
    //    }
    //    return false;
    //}

    //private void Update()
    //{
    //    if (isInContainerUI) return; // Не обрабатываем управление инвентарем, если открыт UI контейнера

    //    // Переключение слотов только вне интерфейса
    //    for (int i = 0; i < 6; i++)
    //    {
    //        if (Input.GetKeyDown(KeyCode.Alpha1 + i))
    //        {
    //            ChangeSelectedSlot(i);
    //        }
    //    }

    //    // Использование предмета по Q
    //    if (Input.GetKeyDown(KeyCode.Q))
    //    {
    //        UseSelectedItem();
    //    }
    //}
    //public bool AddItem(Item item, int count = 1)
    //{
    //    // Проверяем есть ли такой предмет в хотбаре и можно ли добавить в стек
    //    for (int i = 0; i < hotbarItems.Length; i++)
    //    {
    //        if (hotbarItems[i] == item && itemCounts[i] < item.maxStack)
    //        {
    //            itemCounts[i] += count;
    //            OnInventoryChanged?.Invoke();
    //            return true;
    //        }
    //    }

    //    // Ищем пустой слот
    //    for (int i = 0; i < hotbarItems.Length; i++)
    //    {
    //        if (hotbarItems[i] == null)
    //        {
    //            hotbarItems[i] = item;
    //            itemCounts[i] = count;
    //            OnInventoryChanged?.Invoke();
    //            return true;
    //        }
    //    }

    //    return false; // Нет места
    //}

    //public void RemoveItem(int slotIndex, int count = 1)
    //{
    //    if (hotbarItems[slotIndex] != null)
    //    {
    //        itemCounts[slotIndex] -= count;

    //        if (itemCounts[slotIndex] <= 0)
    //        {
    //            hotbarItems[slotIndex] = null;
    //        }

    //        OnInventoryChanged?.Invoke();
    //    }
    //}

    //public void UseSelectedItem()
    //{
    //    if (hotbarItems[selectedSlot] != null && !isInContainerUI)
    //    {
    //        hotbarItems[selectedSlot].Use();
    //        RemoveItem(selectedSlot, 1);
    //    }
    //}

    //public void ChangeSelectedSlot(int newSlot)
    //{
    //    selectedSlot = Mathf.Clamp(newSlot, 0, hotbarItems.Length - 1);
    //    OnInventoryChanged?.Invoke();
    //}

    //public bool TransferItemToContainer(Inventory container, int slotIndex)
    //{
    //    if (hotbarItems[slotIndex] == null) return false;

    //    if (container.AddItem(hotbarItems[slotIndex], itemCounts[slotIndex]))
    //    {
    //        RemoveItem(slotIndex, itemCounts[slotIndex]);
    //        return true;
    //    }
    //    return false;
    //}
}