using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName = "New Item";
    public Sprite icon = null;
    public int maxStack = 1;
    public GameObject prefab; // Для физического представления
    public float throwForce = 10f; // Добавляем силу броска
    public virtual void Use()
    {
        Debug.Log("Using " + itemName);
    }
}