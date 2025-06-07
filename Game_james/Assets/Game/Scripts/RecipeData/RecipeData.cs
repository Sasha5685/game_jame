using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipeData", menuName = "Inventory/RecipeData")]
public class RecipeData : ScriptableObject
{
    [System.Serializable]
    public class Recipe
    {
        public Item resultItem;
        public Ingredient[] ingredients;
        public int craftTime = 5;
        public int resultCount = 1;
    }

    [System.Serializable]
    public class Ingredient
    {
        public Item item;
        public int amount = 1;
    }

    public List<Recipe> recipes = new List<Recipe>();
}