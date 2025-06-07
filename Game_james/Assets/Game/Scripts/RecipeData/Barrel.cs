using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractableObject))]
public class Barrel : MonoBehaviour
{
    public bool IsCraftingComplete { get; set; } // Устанавливайте в true, когда крафт завершен
    [System.Serializable]
    public class Inventory
    {
        public Item[] inputItems;
        public int[] inputCounts;
        public Item[] outputItems;
        public int[] outputCounts;

        public Inventory(int slotCount)
        {
            inputItems = new Item[slotCount];
            inputCounts = new int[slotCount];
            outputItems = new Item[slotCount];
            outputCounts = new int[slotCount];

            // Инициализация
            for (int i = 0; i < slotCount; i++)
            {
                inputCounts[i] = 0;
                outputCounts[i] = 0;
            }
        }
    }

    public Inventory inventory;


    public int YouBarrelId;
    private Coroutine craftingCoroutine;
    public void SaveInputItems(Slot[] slots)
    {
        for (int i = 0; i < Mathf.Min(inventory.inputItems.Length, slots.Length); i++)
        {
            inventory.inputItems[i] = slots[i].ItemType;
            inventory.inputCounts[i] = slots[i].Count;
        }
    }
    private void Start()
    {
        YouBarrelId = BarrelManager.Instance.RegisterBarrel(this);
        inventory = new Inventory(9);
    }
    public void StartCrafting(RecipeData.Recipe recipe)
    {
        if (craftingCoroutine != null)
            StopCoroutine(craftingCoroutine);

        craftingCoroutine = StartCoroutine(CraftingProcess(recipe));
    }
    private IEnumerator CraftingProcess(RecipeData.Recipe recipe)
    {
        BarrelManager.Instance.UpdateCraftingUI(YouBarrelId, true, recipe.craftTime, recipe.craftTime);

        for (int time = recipe.craftTime; time > 0; time--)
        {
            yield return new WaitForSeconds(1f);
            BarrelManager.Instance.UpdateCraftingUI(YouBarrelId, true, time, recipe.craftTime);
        }

        CompleteCrafting(recipe);
        BarrelManager.Instance.UpdateCraftingUI(YouBarrelId, false);
    }

    private void CompleteCrafting(RecipeData.Recipe recipe)
    {
        // Добавляем результат в первый свободный выходной слот
        for (int i = 0; i < 9; i++)
        {
            if (inventory.outputItems[i] == null)
            {
                inventory.outputItems[i] = recipe.resultItem;
                inventory.outputCounts[i] = recipe.resultCount;
                break;
            }
        }

        // Обновляем UI если эта бочка сейчас открыта
        if (BarrelManager.Instance.CurrentBarrelID == YouBarrelId)
        {
            BarrelManager.Instance.LoadBarrelUI(this);
        }
    }

    public void AddCraftingResult(Item resultItem, int resultCount)
    {
        if (resultItem == null) return;

        // Сначала пробуем добавить к существующим стакам
        for (int i = 0; i < inventory.outputItems.Length; i++)
        {
            if (inventory.outputItems[i] == resultItem &&
                inventory.outputCounts[i] + resultCount <= resultItem.maxStack)
            {
                inventory.outputCounts[i] += resultCount;

                // Обновляем UI если открыто
                if (BarrelManager.Instance.CurrentBarrelID == YouBarrelId)
                {
                    BarrelManager.Instance.LoadBarrelUI(this);
                }
                return;
            }
        }

        // Если не нашли подходящий стак, ищем пустой слот
        for (int i = 0; i < inventory.outputItems.Length; i++)
        {
            if (inventory.outputItems[i] == null)
            {
                inventory.outputItems[i] = resultItem;
                inventory.outputCounts[i] = resultCount;

                // Обновляем UI если открыто
                if (BarrelManager.Instance.CurrentBarrelID == YouBarrelId)
                {
                    BarrelManager.Instance.LoadBarrelUI(this);
                }
                return;
            }
        }

        Debug.LogWarning("Нет места для результата крафта!");
    }

}
