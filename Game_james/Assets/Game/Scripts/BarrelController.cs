using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic; // Для использования LINQ

public class BarrelController : MonoBehaviour
{
    [Header("Defould")]
    public GameObject UIBarrel;
    private int Barrel;
    public PlayerController PlayerController;

    [Header("Setings")]
    public Slot[] SlotInput;
    public Slot[] SlotOutput;
    public Item[] ItemsCreate;

    [Header("Recipes")]
    public Recipe[] recipes; // Массив всех рецептов

    [Header("Crafting Timer Settings")]
    public Image progressBar;
    public Text timerText;
    public GameObject timerUI;

    private int remainingTime;
    private bool isCrafting;
    private Recipe currentRecipe; // Текущий рецепт, который крафтится

    [System.Serializable]
    public class Recipe
    {
        public Item resultItem; // Прямая ссылка на предмет
        public Ingredient[] ingredients;
        public int craftTime;
        public int resultCount = 1;
    }

    [System.Serializable]
    public class Ingredient
    {
        public string itemName; // Название предмета
        public int amount; // Необходимое количество
    }

    public void OpenUIBarrel(int barrel)
    {
        Barrel = barrel;
        UIBarrel.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        PlayerController.UIOpen = true;
    }

    public void ClouseUIBarrel()
    {
        UIBarrel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PlayerController.UIOpen = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && PlayerController.UIOpen == true)
        {
            ClouseUIBarrel();
        }
    }

    public void StarCreate()
    {
        CheckRecipes();
    }

    public void CheckRecipes()
    {
        // Проверяем все рецепты
        foreach (Recipe recipe in recipes)
        {
            if (CanCraftRecipe(recipe))
            {
                CraftRecipe(recipe);
                return; // Крафтим только первый подходящий рецепт
            }
        }

        Debug.Log("Нет подходящих рецептов для имеющихся ингредиентов");
    }

    private bool CanCraftRecipe(Recipe recipe)
    {
        // Создаем словарь для подсчета имеющихся ингредиентов
        Dictionary<string, int> availableIngredients = new Dictionary<string, int>();

        // Считаем все предметы в слотах ввода
        foreach (Slot slot in SlotInput)
        {
            if (slot.ItemType != null && !string.IsNullOrEmpty(slot.NameText.text))
            {
                string itemName = slot.NameText.text;
                if (availableIngredients.ContainsKey(itemName))
                {
                    availableIngredients[itemName] += slot.Count; // Изменено с ItemCount на Count
                }
                else
                {
                    availableIngredients[itemName] = slot.Count; // Изменено с ItemCount на Count
                }
            }
        }

        // Проверяем, хватает ли всех ингредиентов для рецепта
        foreach (Ingredient ingredient in recipe.ingredients)
        {
            if (!availableIngredients.ContainsKey(ingredient.itemName)
                || availableIngredients[ingredient.itemName] < ingredient.amount)
            {
                return false;
            }
        }

        return true;
    }

    private void CraftRecipe(Recipe recipe)
    {
        // Проверяем есть ли место в выходных слотах
        int emptySlots = SlotOutput.Count(s => s.ItemType == null);
        if (emptySlots < recipe.resultCount)
        {
            Debug.Log("Недостаточно свободных слотов для результата");
            return;
        }

        // Удаляем ингредиенты
        Dictionary<string, int> ingredientsToRemove = new Dictionary<string, int>();
        foreach (Ingredient ingredient in recipe.ingredients)
        {
            ingredientsToRemove[ingredient.itemName] = ingredient.amount;
        }

        // Проходим по всем слотам и удаляем нужное количество предметов
        foreach (Slot slot in SlotInput)
        {
            if (slot.ItemType != null && ingredientsToRemove.Count > 0)
            {
                string itemName = slot.NameText.text;
                if (ingredientsToRemove.ContainsKey(itemName))
                {
                    int removeAmount = Mathf.Min(slot.Count, ingredientsToRemove[itemName]); // Изменено с ItemCount на Count
                    slot.Count -= removeAmount; // Изменено с ItemCount на Count
                    ingredientsToRemove[itemName] -= removeAmount;

                    if (slot.Count <= 0)
                    {
                        slot.DestroySlot();
                    }
                    else
                    {
                        slot.CountText.text = slot.Count.ToString(); // Обновляем текст количества
                    }

                    if (ingredientsToRemove[itemName] <= 0)
                    {
                        ingredientsToRemove.Remove(itemName);
                    }
                }
            }
        }

        currentRecipe = recipe;
        StartCoroutine(TimeForCreateCraft(recipe.craftTime));
    }
    public IEnumerator TimeForCreateCraft(int craftTime)
    {
        if (isCrafting) yield break;

        isCrafting = true;
        remainingTime = craftTime;

        if (timerUI != null) timerUI.SetActive(true);

        while (remainingTime > 0)
        {
            if (progressBar != null)
                progressBar.fillAmount = (float)remainingTime / craftTime;

            if (timerText != null)
                timerText.text = remainingTime.ToString();

            yield return new WaitForSeconds(1f);
            remainingTime--;
        }

        CraftComplite();
        if (timerUI != null) timerUI.SetActive(false);
        isCrafting = false;
    }

    public void CraftComplite()
    {
        if (currentRecipe == null) return;

        // Находим все пустые слоты
        var emptySlots = new List<Slot>();
        foreach (var slot in SlotOutput)
        {
            if (slot.ItemType == null && emptySlots.Count < currentRecipe.resultCount)
            {
                emptySlots.Add(slot);
            }
        }

        // Добавляем предметы в слоты
        foreach (var slot in emptySlots)
        {
            slot.AddItemToSlot(currentRecipe.resultItem, 1);
        }

        currentRecipe = null;
    }
}