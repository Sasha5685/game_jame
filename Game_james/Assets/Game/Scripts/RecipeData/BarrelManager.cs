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
        // ������� �����
        foreach (var slot in inputSlots) slot.DestroySlot();
        foreach (var slot in outputSlots) slot.DestroySlot();

        // ��������� ������� �����
        for (int i = 0; i < barrel.inventory.inputItems.Length; i++)
        {
            if (barrel.inventory.inputItems[i] != null)
            {
                inputSlots[i].AddItemToSlot(barrel.inventory.inputItems[i], barrel.inventory.inputCounts[i]);
            }
        }

        // ��������� �������� �����
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
        // �������� ����� ��� ���������� ��������
        var keys = activeCraftings.Keys.ToList();
        foreach (var barrelID in keys)
        {
            if (!activeCraftings.ContainsKey(barrelID)) continue;

            var craft = activeCraftings[barrelID];
            craft.remainingTime -= Time.deltaTime;

            // ��������� UI ��� ������� ����� � �������� �������
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
        // ��������� ������� �����
        int slotsToSave = Mathf.Min(inputSlots.Length, barrel.inventory.inputItems.Length);
        for (int i = 0; i < slotsToSave; i++)
        {
            barrel.inventory.inputItems[i] = inputSlots[i].ItemType;
            barrel.inventory.inputCounts[i] = inputSlots[i].Count;
        }

        // ��������� �������� �����
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
        // ������ ��� ������� �������� �����
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
            Debug.LogWarning("�� ������� ����� ��� ������");
            return;
        }

        foreach (var recipe in recipeData.recipes)
        {
            if (recipe == null) continue;

            if (CheckRecipe(recipe))
            {
                // �������� �� float
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

        Debug.LogWarning("�� ������� ����������� ������� ��� ������� ������������");
    }

    private bool CheckRecipe(RecipeData.Recipe recipe)
    {
        // 1. �������� �� null ������� � ��� ������������
        if (recipe == null || recipe.ingredients == null)
        {
            Debug.LogWarning("������ �� ����� ��� �� �������� ������������");
            return false;
        }

        // 2. ������� ��������, ��� �� � ������ ������ ��������� (�� �� �������)
        foreach (var slot in inputSlots)
        {
            if (slot == null || slot.ItemType == null) continue; // ������ ����� ����������

            bool isItemInRecipe = false;
            foreach (var ingredient in recipe.ingredients)
            {
                if (ingredient != null && ingredient.item == slot.ItemType)
                {
                    isItemInRecipe = true;
                    break;
                }
            }

            // ���� � ����� �������, �������� ��� � ������� - ������ ���������
            if (!isItemInRecipe)
            {
                Debug.LogWarning($"� ����� ������ ������ �������: {slot.ItemType.itemName}");
                return false;
            }
        }

        // 3. ������ ��������� ������ ���������� �������
        foreach (var ingredient in recipe.ingredients)
        {
            if (ingredient == null || ingredient.item == null)
            {
                Debug.LogWarning("������ �������� ������ ����������");
                return false;
            }

            int requiredAmount = ingredient.amount;
            int totalFound = 0;
            int slotsWithItem = 0;

            // 4. �������, ������� ������� �������� � ������
            foreach (var slot in inputSlots)
            {
                if (slot == null || slot.ItemType == null) continue;

                if (slot.ItemType == ingredient.item)
                {
                    totalFound += slot.Count;
                    slotsWithItem++;
                }
            }

            // 5. ���� ����� ���������� ������ ������� - ������
            if (totalFound < requiredAmount)
            {
                Debug.LogWarning($"������������ {ingredient.item.itemName}. �����: {requiredAmount}, ����: {totalFound}");
                return false;
            }

            // 6. ���� ����� ����� 1, �� �� ��������� �� ������ - ������
            if (requiredAmount == 1 && slotsWithItem > 1)
            {
                Debug.LogWarning($"���������� {ingredient.item.itemName} ������ ���� � ����� �����, �� ��������� �� {slotsWithItem} ������");
                return false;
            }

            // 7. ���� ����� >1, �� �� � ����� ����� ��� ������� ���������� - ������
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
                    Debug.LogWarning($"����� {requiredAmount} {ingredient.item.itemName} � ����� �����, �� ��� ����������");
                    return false;
                }
            }
        }

        // 8. ���� ��� �������� �������� - ������ �������
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
        // ��������� ��������� � �����
        Barrel[barrelID].AddCraftingResult(craftingData.recipe.resultItem, craftingData.recipe.resultCount);
        SaveBarrelUI(Barrel[CurrentBarrelID]);
        // ��������� UI ���� ��� ����� �������
        if (CurrentBarrelID == barrelID)
        {
            LoadBarrelUI(Barrel[barrelID]);
        }

        UpdateCraftingUI(barrelID, false);
    }


}