using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BarrelManager : MonoBehaviour
{
    public static BarrelManager Instance;

    [Header("UI References")]
    public GameObject barrelUI;
    public Slot[] inputSlots;
    public Slot[] outputSlots;
    public Image progressBar;
    public Text timerText;
    public GameObject timerUI;
    public PlayerController PlayerController;
    [Header("Recipe Data")]
    public RecipeData recipeData;

    public Barrel[] Barrel;
    public int AllBarrel;
    public int CurrentBarrelID { get; private set; } = -1;

    private Dictionary<int, BarrelCraftingData> activeCraftings = new Dictionary<int, BarrelCraftingData>();

    private class BarrelCraftingData
    {
        public RecipeData.Recipe recipe;
        public float remainingTime;
    }

    private void Awake()
    {
        Instance = this;
    }

    public int RegisterBarrel(Barrel barrel)
    {
        Barrel[AllBarrel] = barrel;
        AllBarrel++;
        return AllBarrel - 1;
    }

    public void OpenBarrelUI(int barrelID)
    {
        CurrentBarrelID = barrelID;
        LoadBarrelUI(Barrel[barrelID]);
        barrelUI.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        PlayerController.UIOpen = true;
    }

    public void LoadBarrelUI(Barrel barrel)
    {
        // Очищаем слоты
        foreach (var slot in inputSlots) slot.DestroySlot();
        foreach (var slot in outputSlots) slot.DestroySlot();

        // Загружаем входные слоты
        for (int i = 0; i < barrel.inventory.inputItems.Length; i++)
        {
            if (barrel.inventory.inputItems[i] != null)
            {
                inputSlots[i].AddItemToSlot(barrel.inventory.inputItems[i], barrel.inventory.inputCounts[i]);
            }
        }

        // Загружаем выходные слоты
        for (int i = 0; i < barrel.inventory.outputItems.Length; i++)
        {
            if (barrel.inventory.outputItems[i] != null)
            {
                outputSlots[i].AddItemToSlot(barrel.inventory.outputItems[i], barrel.inventory.outputCounts[i]);
            }
        }
    }

    public void CloseBarrelUI()
    {
        if (CurrentBarrelID == -1) return;

        SaveBarrelUI(Barrel[CurrentBarrelID]);
        CurrentBarrelID = -1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PlayerController.UIOpen = false;
        barrelUI.SetActive(false);

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && PlayerController.UIOpen == true)
        {
            CloseBarrelUI();
        }
        // Копируем ключи для безопасной итерации
        var keys = activeCraftings.Keys.ToList();
        foreach (var barrelID in keys)
        {
            if (!activeCraftings.ContainsKey(barrelID)) continue;

            var craft = activeCraftings[barrelID];
            craft.remainingTime -= Time.deltaTime;

            // Обновляем UI для текущей бочки в реальном времени
            if (CurrentBarrelID == barrelID)
            {
                UpdateCraftingUI(barrelID, true, craft.remainingTime, craft.recipe.craftTime);
            }

            if (craft.remainingTime <= 0)
            {
                CompleteCrafting(barrelID);
                activeCraftings.Remove(barrelID);
            }
        }
    }
    private void SaveBarrelUI(Barrel barrel)
    {
        // Сохраняем входные слоты
        int slotsToSave = Mathf.Min(inputSlots.Length, barrel.inventory.inputItems.Length);
        for (int i = 0; i < slotsToSave; i++)
        {
            barrel.inventory.inputItems[i] = inputSlots[i].ItemType;
            barrel.inventory.inputCounts[i] = inputSlots[i].Count;
        }

        // Сохраняем выходные слоты
        slotsToSave = Mathf.Min(outputSlots.Length, barrel.inventory.outputItems.Length);
        for (int i = 0; i < slotsToSave; i++)
        {
            barrel.inventory.outputItems[i] = outputSlots[i].ItemType;
            barrel.inventory.outputCounts[i] = outputSlots[i].Count;
        }
    }

    private Dictionary<int, float> currentProgress = new Dictionary<int, float>();

    public void UpdateCraftingUI(int barrelID, bool isActive, float remainingTime = 0f, int totalTime = 1)
    {
        // Только для текущей открытой бочки
        if (CurrentBarrelID != barrelID)
        {
            if (timerUI.activeSelf)
                timerUI.SetActive(false);
            return;
        }

        timerUI.SetActive(isActive);
        if (isActive)
        {
            float progress = 1f - (remainingTime / totalTime);
            progressBar.fillAmount = progress;
            timerText.text = Mathf.CeilToInt(remainingTime).ToString();
        }
        else if (currentProgress.ContainsKey(barrelID))
        {
            currentProgress.Remove(barrelID);
        }
    }

    public void StartCrafting()
    {
        if (CurrentBarrelID == -1)
        {
            Debug.LogWarning("Не выбрана бочка для крафта");
            return;
        }

        foreach (var recipe in recipeData.recipes)
        {
            if (recipe == null) continue;

            if (CheckRecipe(recipe))
            {
                // Изменено на float
                activeCraftings[CurrentBarrelID] = new BarrelCraftingData
                {
                    recipe = recipe,
                    remainingTime = (float)recipe.craftTime
                };

                RemoveIngredients(recipe);
                UpdateCraftingUI(CurrentBarrelID, true, recipe.craftTime, recipe.craftTime);
                return;
            }
        }

        Debug.LogWarning("Не найдено подходящего рецепта для текущих ингредиентов");
    }

    private bool CheckRecipe(RecipeData.Recipe recipe)
    {
        // 1. Проверка на null рецепта и его ингредиентов
        if (recipe == null || recipe.ingredients == null)
        {
            Debug.LogWarning("Рецепт не задан или не содержит ингредиентов");
            return false;
        }

        // 2. Сначала проверим, нет ли в слотах ЛИШНИХ предметов (не из рецепта)
        foreach (var slot in inputSlots)
        {
            if (slot == null || slot.ItemType == null) continue; // Пустые слоты пропускаем

            bool isItemInRecipe = false;
            foreach (var ingredient in recipe.ingredients)
            {
                if (ingredient != null && ingredient.item == slot.ItemType)
                {
                    isItemInRecipe = true;
                    break;
                }
            }

            // Если в слоте предмет, которого нет в рецепте - рецепт невалиден
            if (!isItemInRecipe)
            {
                Debug.LogWarning($"В слоте найден лишний предмет: {slot.ItemType.itemName}");
                return false;
            }
        }

        // 3. Теперь проверяем каждый ингредиент рецепта
        foreach (var ingredient in recipe.ingredients)
        {
            if (ingredient == null || ingredient.item == null)
            {
                Debug.LogWarning("Рецепт содержит пустой ингредиент");
                return false;
            }

            int requiredAmount = ingredient.amount;
            int totalFound = 0;
            int slotsWithItem = 0;

            // 4. Считаем, сколько нужного предмета в слотах
            foreach (var slot in inputSlots)
            {
                if (slot == null || slot.ItemType == null) continue;

                if (slot.ItemType == ingredient.item)
                {
                    totalFound += slot.Count;
                    slotsWithItem++;
                }
            }

            // 5. Если общее количество меньше нужного - отмена
            if (totalFound < requiredAmount)
            {
                Debug.LogWarning($"Недостаточно {ingredient.item.itemName}. Нужно: {requiredAmount}, есть: {totalFound}");
                return false;
            }

            // 6. Если нужно ровно 1, но он разбросан по слотам - отмена
            if (requiredAmount == 1 && slotsWithItem > 1)
            {
                Debug.LogWarning($"Ингредиент {ingredient.item.itemName} должен быть в одном слоте, но разбросан по {slotsWithItem} слотам");
                return false;
            }

            // 7. Если нужно >1, но ни в одном слоте нет нужного количества - отмена
            if (requiredAmount > 1)
            {
                bool hasEnoughInSingleSlot = false;
                foreach (var slot in inputSlots)
                {
                    if (slot != null && slot.ItemType == ingredient.item && slot.Count >= requiredAmount)
                    {
                        hasEnoughInSingleSlot = true;
                        break;
                    }
                }

                if (!hasEnoughInSingleSlot)
                {
                    Debug.LogWarning($"Нужно {requiredAmount} {ingredient.item.itemName} в одном слоте, но они разбросаны");
                    return false;
                }
            }
        }

        // 8. Если все проверки пройдены - рецепт валиден
        return true;
    }


    private bool RemoveIngredients(RecipeData.Recipe recipe)
    {
        Dictionary<Item, int> itemsToRemove = new Dictionary<Item, int>();
        foreach (var ingredient in recipe.ingredients)
        {
            if (ingredient == null || ingredient.item == null) continue;

            if (itemsToRemove.ContainsKey(ingredient.item))
                itemsToRemove[ingredient.item] += ingredient.amount;
            else
                itemsToRemove[ingredient.item] = ingredient.amount;
        }

        foreach (var slot in inputSlots)
        {
            slot.DestroySlot();
        }

        return itemsToRemove.Count == 0;
    }

    private void CompleteCrafting(int barrelID)
    {
        if (!activeCraftings.TryGetValue(barrelID, out var craftingData)) return;

        SaveBarrelUI(Barrel[CurrentBarrelID]);
        // Добавляем результат в бочку
        Barrel[barrelID].AddCraftingResult(craftingData.recipe.resultItem, craftingData.recipe.resultCount);
        SaveBarrelUI(Barrel[CurrentBarrelID]);
        // Обновляем UI если эта бочка открыта
        if (CurrentBarrelID == barrelID)
        {
            LoadBarrelUI(Barrel[barrelID]);
        }

        UpdateCraftingUI(barrelID, false);
    }


}